using UnityEngine;

namespace EcoGarden.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class EcoGardenBackgroundController : MonoBehaviour
    {
        private const string DefaultBackgroundPath = "Backgrounds/bg_pond_foggy_01";

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private string backgroundResourcePath = DefaultBackgroundPath;
        [SerializeField] private float extraScale = 1.08f;
        [SerializeField] private Vector2 worldOffset;

        private void Awake()
        {
            EnsureRenderer();
            LoadBackgroundSprite();
            FitToCamera();
        }

        private void LateUpdate()
        {
            FitToCamera();
        }

        public void Configure(Camera camera)
        {
            targetCamera = camera;
            FitToCamera();
        }

        private void EnsureRenderer()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sortingOrder = -1000;
            spriteRenderer.color = Color.white;
        }

        private void LoadBackgroundSprite()
        {
            if (spriteRenderer.sprite != null)
            {
                return;
            }

            Sprite sprite = Resources.Load<Sprite>(backgroundResourcePath);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        private void FitToCamera()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (targetCamera == null || !targetCamera.orthographic || spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return;
            }

            float worldHeight = targetCamera.orthographicSize * 2f * extraScale;
            float worldWidth = worldHeight * targetCamera.aspect;
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            float scale = Mathf.Max(worldWidth / spriteSize.x, worldHeight / spriteSize.y);
            transform.localScale = new Vector3(scale, scale, 1f);
            transform.position = new Vector3(
                targetCamera.transform.position.x + worldOffset.x,
                targetCamera.transform.position.y + worldOffset.y,
                5f);
        }

        public void SetCosmeticTint(Color tint)
        {
            EnsureRenderer();
            spriteRenderer.color = tint;
        }

        public void ResetCosmeticBackground()
        {
            EnsureRenderer();
            backgroundResourcePath = DefaultBackgroundPath;
            worldOffset = Vector2.zero;
            Sprite sprite = Resources.Load<Sprite>(backgroundResourcePath);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }

            spriteRenderer.color = Color.white;
            FitToCamera();
        }

        public void SetCosmeticBackground(string resourcePath, Color tint)
        {
            SetCosmeticBackground(resourcePath, tint, Vector2.zero);
        }

        public void SetCosmeticBackground(string resourcePath, Color tint, Vector2 offset)
        {
            EnsureRenderer();
            if (!string.IsNullOrWhiteSpace(resourcePath))
            {
                Sprite sprite = Resources.Load<Sprite>(resourcePath);
                if (sprite != null)
                {
                    backgroundResourcePath = resourcePath;
                    spriteRenderer.sprite = sprite;
                }
            }

            worldOffset = offset;
            spriteRenderer.color = tint;
            FitToCamera();
        }
    }
}
