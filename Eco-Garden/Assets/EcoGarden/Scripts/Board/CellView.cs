using UnityEngine;

namespace EcoGarden.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer shadowRenderer;
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
            RefreshShadow(sprite);
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
            RefreshShadow(sprite);
            name = "Cell_" + boardCell.Position.X + "_" + boardCell.Position.Y + "_" + boardCell.Kind;
        }

        private void RefreshShadow(Sprite sprite)
        {
            EnsureShadowRenderer();
            shadowRenderer.sprite = sprite;
            shadowRenderer.color = new Color(0.05f, 0.12f, 0.10f, 0.18f);
            shadowRenderer.sortingOrder = -1;
        }

        private void EnsureShadowRenderer()
        {
            if (shadowRenderer != null)
            {
                return;
            }

            Transform existing = transform.Find("CellShadow");
            GameObject shadowObject = existing != null
                ? existing.gameObject
                : new GameObject("CellShadow");
            shadowObject.transform.SetParent(transform, false);
            shadowObject.transform.localPosition = new Vector3(0.035f, -0.045f, 0.02f);
            shadowObject.transform.localScale = new Vector3(1.04f, 1.04f, 1f);

            shadowRenderer = shadowObject.GetComponent<SpriteRenderer>();
            if (shadowRenderer == null)
            {
                shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
            }
        }
    }
}
