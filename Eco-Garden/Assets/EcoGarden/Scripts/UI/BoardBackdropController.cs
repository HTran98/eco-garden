using EcoGarden.Board;
using EcoGarden.Utilities;
using UnityEngine;

namespace EcoGarden.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BoardBackdropController : MonoBehaviour
    {
        private const string BackdropSpritePath = "UiSkins/ui_board_backdrop";

        [SerializeField] private BoardController boardController;
        [SerializeField] private float horizontalPadding = 0.42f;
        [SerializeField] private float verticalPadding = 0.44f;
        [SerializeField] private int sortingOrder = -900;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            EnsureRenderer();
        }

        private void LateUpdate()
        {
            Apply();
        }

        public void Configure(BoardController controller)
        {
            boardController = controller;
            Apply();
        }

        private void Apply()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (boardController == null ||
                boardController.BoardView == null ||
                boardController.BoardState == null)
            {
                return;
            }

            EnsureRenderer();

            float width = boardController.BoardView.GetBoardWorldWidth(boardController.BoardState) + horizontalPadding * 2f;
            float height = boardController.BoardView.GetBoardWorldHeight(boardController.BoardState) + verticalPadding * 2f;
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            float scaleX = spriteSize.x > 0f ? width / spriteSize.x : 1f;
            float scaleY = spriteSize.y > 0f ? height / spriteSize.y : 1f;

            Transform boardTransform = boardController.BoardView.transform;
            transform.position = new Vector3(boardTransform.position.x, boardTransform.position.y, 4.5f);
            transform.localScale = new Vector3(scaleX, scaleY, 1f);
            spriteRenderer.sortingOrder = sortingOrder;
        }

        private void EnsureRenderer()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>(BackdropSpritePath);
            }

            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = PlaceholderSpriteFactory.EmptyTileSprite;
            }

            spriteRenderer.color = Color.white;
        }
    }
}
