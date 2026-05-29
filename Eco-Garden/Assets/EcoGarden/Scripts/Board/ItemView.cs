using EcoGarden.Items;
using UnityEngine;

namespace EcoGarden.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ItemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer shadowRenderer;
        [SerializeField] private TextMesh label;
        [SerializeField] private BoardItem item;
        [SerializeField] private GridPosition position;
        [SerializeField] private bool showLevelLabel;

        public BoardItem Item { get { return item; } }
        public GridPosition Position { get { return position; } }
        public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(BoardItem boardItem, GridPosition gridPosition, Sprite sprite, Color color, Vector3 worldPosition, float size)
        {
            item = boardItem;
            position = gridPosition;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = 10;
            RefreshShadow(sprite, 8, 0.30f);
            transform.position = worldPosition + new Vector3(0f, 0f, -0.1f);
            transform.localScale = new Vector3(size, size, 1f);
            name = "Item_" + boardItem.ItemId + "_" + gridPosition.X + "_" + gridPosition.Y;

            RefreshLabel(boardItem);
        }

        public void Refresh(BoardItem boardItem, GridPosition gridPosition, Sprite sprite, Color color, Vector3 worldPosition, float size)
        {
            item = boardItem;
            position = gridPosition;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            RefreshShadow(sprite, 8, color.a * 0.30f);
            transform.position = worldPosition + new Vector3(0f, 0f, -0.1f);
            transform.localScale = new Vector3(size, size, 1f);
            name = "Item_" + boardItem.ItemId + "_" + gridPosition.X + "_" + gridPosition.Y;

            RefreshLabel(boardItem);
        }

        public void BeginDrag()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 100;
            }

            if (shadowRenderer != null)
            {
                shadowRenderer.sortingOrder = 90;
            }

            if (showLevelLabel && label != null)
            {
                MeshRenderer labelRenderer = label.GetComponent<MeshRenderer>();
                if (labelRenderer != null)
                {
                    labelRenderer.sortingOrder = 110;
                }
            }

            transform.localScale *= 1.12f;
        }

        public void SetDragWorldPosition(Vector3 worldPosition)
        {
            transform.position = new Vector3(worldPosition.x, worldPosition.y, -0.25f);
        }

        public void SetDragWorldPosition(Vector3 worldPosition, float z)
        {
            transform.position = new Vector3(worldPosition.x, worldPosition.y, z);
        }

        public void EndDrag(Vector3 worldPosition, float size)
        {
            transform.position = worldPosition + new Vector3(0f, 0f, -0.1f);
            transform.localScale = new Vector3(size, size, 1f);
            SetAlpha(1f);

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 10;
            }

            if (shadowRenderer != null)
            {
                shadowRenderer.sortingOrder = 8;
            }

            if (showLevelLabel && label != null)
            {
                MeshRenderer labelRenderer = label.GetComponent<MeshRenderer>();
                if (labelRenderer != null)
                {
                    labelRenderer.sortingOrder = 20;
                }
            }
        }

        private void EnsureLabel()
        {
            if (label != null)
            {
                return;
            }

            GameObject labelObject = new GameObject("LevelLabel");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 0f, -0.05f);

            label = labelObject.AddComponent<TextMesh>();
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.18f;
            label.fontSize = 64;
            label.color = Color.white;
            label.GetComponent<MeshRenderer>().sortingOrder = 20;
        }

        private void RefreshLabel(BoardItem boardItem)
        {
            if (!showLevelLabel)
            {
                if (label != null)
                {
                    label.gameObject.SetActive(false);
                }

                return;
            }

            EnsureLabel();
            label.gameObject.SetActive(true);
            label.text = boardItem.Level.ToString();
        }

        public void SetAlpha(float alpha)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            if (shadowRenderer != null)
            {
                Color shadowColor = shadowRenderer.color;
                shadowColor.a = alpha * 0.30f;
                shadowRenderer.color = shadowColor;
            }

            if (showLevelLabel && label != null)
            {
                Color color = label.color;
                color.a = alpha;
                label.color = color;
            }
        }

        private void RefreshShadow(Sprite sprite, int sortingOrder, float alpha)
        {
            EnsureShadowRenderer();
            shadowRenderer.sprite = sprite;
            shadowRenderer.color = new Color(0.04f, 0.09f, 0.08f, Mathf.Clamp01(alpha));
            shadowRenderer.sortingOrder = sortingOrder;
        }

        private void EnsureShadowRenderer()
        {
            if (shadowRenderer != null)
            {
                return;
            }

            Transform existing = transform.Find("ItemShadow");
            GameObject shadowObject = existing != null
                ? existing.gameObject
                : new GameObject("ItemShadow");
            shadowObject.transform.SetParent(transform, false);
            shadowObject.transform.localPosition = new Vector3(0.055f, -0.07f, 0.035f);
            shadowObject.transform.localScale = new Vector3(1.05f, 1.05f, 1f);

            shadowRenderer = shadowObject.GetComponent<SpriteRenderer>();
            if (shadowRenderer == null)
            {
                shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
            }
        }
    }
}
