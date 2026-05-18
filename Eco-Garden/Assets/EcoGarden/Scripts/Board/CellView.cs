using UnityEngine;

namespace EcoGarden.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoardCell cell;

        public BoardCell Cell { get { return cell; } }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(BoardCell boardCell, Sprite sprite, Color color, Vector3 worldPosition, Vector2 size)
        {
            cell = boardCell;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = 0;
            transform.position = worldPosition;
            transform.localScale = new Vector3(size.x, size.y, 1f);
            name = "Cell_" + boardCell.Position.X + "_" + boardCell.Position.Y + "_" + boardCell.Kind;
        }

        public void Refresh(BoardCell boardCell, Sprite sprite, Color color)
        {
            cell = boardCell;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            name = "Cell_" + boardCell.Position.X + "_" + boardCell.Position.Y + "_" + boardCell.Kind;
        }
    }
}
