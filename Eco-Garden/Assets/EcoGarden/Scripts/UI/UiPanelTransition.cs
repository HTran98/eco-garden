using UnityEngine;

namespace EcoGarden.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UiPanelTransition : MonoBehaviour
    {
        [SerializeField] private float duration = 0.16f;
        [SerializeField] private float startScale = 0.965f;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector3 targetScale;
        private float elapsed;

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            CacheReferences();
            targetScale = rectTransform.localScale;
            elapsed = 0f;
            canvasGroup.alpha = 0f;
            rectTransform.localScale = targetScale * startScale;
        }

        private void Update()
        {
            if (elapsed >= duration)
            {
                return;
            }

            elapsed += Time.unscaledDeltaTime;
            float t = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            canvasGroup.alpha = eased;
            rectTransform.localScale = Vector3.LerpUnclamped(targetScale * startScale, targetScale, eased);
        }

        private void OnDisable()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = targetScale == Vector3.zero ? Vector3.one : targetScale;
            }
        }

        private void CacheReferences()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }
    }
}
