using System.Collections;
using EcoGarden.Board;
using EcoGarden.Input;
using EcoGarden.Utilities;
using UnityEngine;

namespace EcoGarden.AI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class NpcMovementController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float moveSpeed = 2.2f;
        [SerializeField] private float idleBobAmplitude = 0.08f;
        [SerializeField] private float idleBobSpeed = 2.8f;
        [SerializeField] private Color npcColor = new Color(0.72f, 0.44f, 0.82f, 1f);

        private Vector3 idleBasePosition;
        private Vector3 exitPosition;
        private Coroutine movementRoutine;
        private bool isIdle;
        private ExternalDropZone deliveryDropZone;

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boardController = FindAnyObjectByType<BoardController>();
        }

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = PlaceholderSpriteFactory.NpcSprite;
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 5;
            transform.localScale = new Vector3(0.82f, 1.08f, 1f);
        }

        private void OnEnable()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            deliveryDropZone = FindDeliveryDropZone();

            if (boardController != null)
            {
                boardController.ObjectiveCompleted += ExitAfterFulfillment;
            }
        }

        private void OnDisable()
        {
            if (boardController != null)
            {
                boardController.ObjectiveCompleted -= ExitAfterFulfillment;
            }
        }

        private void Start()
        {
            movementRoutine = StartCoroutine(EnterWhenBoardIsReady());
        }

        private void Update()
        {
            if (!isIdle)
            {
                return;
            }

            float offset = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmplitude;
            transform.position = idleBasePosition + new Vector3(0f, offset, 0f);
        }

        public void SetBoardController(BoardController controller)
        {
            if (boardController != null)
            {
                boardController.ObjectiveCompleted -= ExitAfterFulfillment;
            }

            boardController = controller;

            if (isActiveAndEnabled && boardController != null)
            {
                boardController.ObjectiveCompleted += ExitAfterFulfillment;
            }
        }

        private IEnumerator EnterWhenBoardIsReady()
        {
            while (boardController == null || boardController.BoardState == null || boardController.BoardView == null)
            {
                if (boardController == null)
                {
                    boardController = FindAnyObjectByType<BoardController>();
                }

                yield return null;
            }

            deliveryDropZone = FindDeliveryDropZone();
            if (deliveryDropZone != null)
            {
                ConfigureUiDeliveryMovement();
            }
            else if (!TryConfigureBoardFallbackMovement())
            {
                yield break;
            }

            yield return MoveTo(idleBasePosition);
            isIdle = true;
            movementRoutine = null;
        }

        private void ExitAfterFulfillment()
        {
            if (!isActiveAndEnabled || boardController == null || boardController.BoardState == null)
            {
                return;
            }

            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
            }

            isIdle = false;
            movementRoutine = StartCoroutine(ExitRoutine());
        }

        private IEnumerator ExitRoutine()
        {
            yield return MoveTo(exitPosition);
            gameObject.SetActive(false);
            movementRoutine = null;
        }

        private IEnumerator MoveTo(Vector3 target)
        {
            while ((transform.position - target).sqrMagnitude > 0.0025f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
        }

        private bool TryFindOrderPosition(out GridPosition position)
        {
            foreach (BoardCell cell in boardController.BoardState.GetCells())
            {
                if (cell != null && cell.Kind == CellKind.NpcOrderPoint)
                {
                    position = cell.Position;
                    return true;
                }
            }

            position = default;
            return false;
        }

        private void ConfigureUiDeliveryMovement()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                TryConfigureBoardFallbackMovement();
                return;
            }

            Vector2 centerScreen = deliveryDropZone.GetScreenCenter(null);
            Vector3 centerWorld = camera.ScreenToWorldPoint(new Vector3(centerScreen.x, centerScreen.y, -camera.transform.position.z));
            centerWorld.z = -0.15f;
            centerWorld += new Vector3(0f, 0.85f, 0f);

            float halfWidth = camera.orthographicSize * camera.aspect;
            Vector3 entryPosition = centerWorld + new Vector3(-halfWidth * 1.2f, 0f, 0f);
            exitPosition = centerWorld + new Vector3(halfWidth * 1.2f, 0f, 0f);
            idleBasePosition = centerWorld;
            transform.position = entryPosition;
        }

        private bool TryConfigureBoardFallbackMovement()
        {
            GridPosition orderPosition;
            if (!TryFindOrderPosition(out orderPosition))
            {
                return false;
            }

            Vector3 entryPosition = boardController.BoardView.GridToWorld(boardController.BoardState, new GridPosition(-1, orderPosition.Y));
            idleBasePosition = boardController.BoardView.GridToWorld(boardController.BoardState, orderPosition);
            exitPosition = boardController.BoardView.GridToWorld(boardController.BoardState, new GridPosition(boardController.BoardState.Width, orderPosition.Y));

            transform.position = entryPosition + new Vector3(0f, 0f, -0.15f);
            idleBasePosition += new Vector3(0f, 0f, -0.15f);
            exitPosition += new Vector3(0f, 0f, -0.15f);
            return true;
        }

        private static ExternalDropZone FindDeliveryDropZone()
        {
            return ExternalDropZone.TryGetFirst(ExternalDropZoneKind.Delivery, out ExternalDropZone zone)
                ? zone
                : null;
        }
    }
}
