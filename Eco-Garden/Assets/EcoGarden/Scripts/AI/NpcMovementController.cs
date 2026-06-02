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
        private const string CustomerSpriteResourcePath = "Characters/char_customer_01";

        [SerializeField] private BoardController boardController;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float moveSpeed = 2.2f;
        [SerializeField] private float idleBobAmplitude = 0.08f;
        [SerializeField] private float idleBobSpeed = 2.8f;
        [SerializeField] private Color npcColor = new Color(0.72f, 0.44f, 0.82f, 1f);

        private Vector3 idleBasePosition;
        private Vector3 checkoutPosition;
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

            Sprite customerSprite = Resources.Load<Sprite>(CustomerSpriteResourcePath);
            spriteRenderer.sprite = customerSprite ?? PlaceholderSpriteFactory.NpcSprite;
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 20;
            transform.localScale = customerSprite != null
                ? new Vector3(2.55f, 3.25f, 1f)
                : new Vector3(0.82f, 1.08f, 1f);
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
                boardController.OrderCompleted += CheckoutAfterFulfillment;
            }
        }

        private void OnDisable()
        {
            if (boardController != null)
            {
                boardController.OrderCompleted -= CheckoutAfterFulfillment;
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
                boardController.OrderCompleted -= CheckoutAfterFulfillment;
            }

            boardController = controller;

            if (isActiveAndEnabled && boardController != null)
            {
                boardController.OrderCompleted += CheckoutAfterFulfillment;
            }
        }

        public void SetCosmeticColor(Color color)
        {
            npcColor = color;
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
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
                SetWorldNpcVisible(true);
                ConfigureUiDeliveryMovement();
            }
            else if (!TryConfigureBoardFallbackMovement())
            {
                yield break;
            }
            else
            {
                SetWorldNpcVisible(true);
            }

            yield return MoveTo(idleBasePosition);
            isIdle = true;
            movementRoutine = null;
        }

        private void CheckoutAfterFulfillment()
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
            movementRoutine = StartCoroutine(CheckoutRoutine());
        }

        private IEnumerator CheckoutRoutine()
        {
            yield return MoveTo(checkoutPosition);
            yield return new WaitForSeconds(0.25f);
            yield return MoveTo(idleBasePosition);
            isIdle = true;
            boardController.StartNextOrder();
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
            checkoutPosition = ResolveCheckoutPosition(camera, centerWorld);
            idleBasePosition = centerWorld;
            transform.position = entryPosition;
        }

        private static Vector3 ResolveCheckoutPosition(Camera camera, Vector3 fallback)
        {
            if (ExternalDropZone.TryGetFirst(ExternalDropZoneKind.SellBasket, out ExternalDropZone sellBasket))
            {
                Vector2 centerScreen = sellBasket.GetScreenCenter(null);
                Vector3 centerWorld = camera.ScreenToWorldPoint(new Vector3(centerScreen.x, centerScreen.y, -camera.transform.position.z));
                centerWorld.z = -0.15f;
                return centerWorld + new Vector3(0f, 0.85f, 0f);
            }

            return fallback + new Vector3(1.5f, 0f, 0f);
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
            checkoutPosition = exitPosition + new Vector3(-0.8f, 0f, 0f);

            transform.position = entryPosition + new Vector3(0f, 0f, -0.15f);
            idleBasePosition += new Vector3(0f, 0f, -0.15f);
            exitPosition += new Vector3(0f, 0f, -0.15f);
            return true;
        }

        private void SetWorldNpcVisible(bool visible)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
            }
        }

        private static ExternalDropZone FindDeliveryDropZone()
        {
            return ExternalDropZone.TryGetFirst(ExternalDropZoneKind.Delivery, out ExternalDropZone zone)
                ? zone
                : null;
        }
    }
}
