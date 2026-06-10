using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class StartupSplashController : MonoBehaviour
    {
        public const string SplashResourcePath = "Splash/startup_splash_eco_garden_portrait";

        [SerializeField] private float holdSeconds = 2.2f;
        [SerializeField] private float fadeSeconds = 0.65f;
        [SerializeField] private bool pauseGameWhileVisible = true;

        private CanvasGroup canvasGroup;
        private RectTransform splashImageRect;
        private Coroutine splashRoutine;
        private float previousTimeScale = 1f;
        private bool restoredTimeScale;

        private void Start()
        {
            Show();
        }

        private void OnDisable()
        {
            RestoreTimeScale();
        }

        private void OnDestroy()
        {
            RestoreTimeScale();
        }

        public void Show()
        {
            if (canvasGroup != null)
            {
                return;
            }

            if (pauseGameWhileVisible)
            {
                previousTimeScale = Time.timeScale;
                restoredTimeScale = false;
                Time.timeScale = 0f;
            }

            CreateSplashCanvas();
            splashRoutine = StartCoroutine(HideAfterDelay());
        }

        public void HideNow()
        {
            if (splashRoutine != null)
            {
                StopCoroutine(splashRoutine);
                splashRoutine = null;
            }

            StartCoroutine(FadeOut(0.12f));
        }

        private void CreateSplashCanvas()
        {
            GameObject canvasObject = new GameObject("StartupSplashCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30000;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGroup = canvasObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
            canvasRect.anchorMin = Vector2.zero;
            canvasRect.anchorMax = Vector2.one;
            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;

            CreateImage("Backdrop", canvasObject.transform, null, new Color(0.36f, 0.64f, 0.55f, 1f), Vector2.zero, Vector2.one, false);

            Sprite splashSprite = Resources.Load<Sprite>(SplashResourcePath);
            Image splashImage = CreateImage("PortraitSplash", canvasObject.transform, splashSprite, Color.white, Vector2.zero, Vector2.one, false);
            splashImage.raycastTarget = false;
            splashImageRect = splashImage.GetComponent<RectTransform>();

            Button skipButton = canvasObject.AddComponent<Button>();
            skipButton.transition = Selectable.Transition.None;
            skipButton.onClick.AddListener(HideNow);
        }

        private static Image CreateImage(string name, Transform parent, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax, bool preserveAspect)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = imageObject.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = true;
            return image;
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0.05f, holdSeconds));
            yield return FadeOut(fadeSeconds);
        }

        private IEnumerator FadeOut(float duration)
        {
            if (canvasGroup == null)
            {
                RestoreTimeScale();
                yield break;
            }

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;
            Vector3 startScale = splashImageRect != null ? splashImageRect.localScale : Vector3.one;
            Vector3 endScale = startScale * 1.08f;
            float safeDuration = Mathf.Max(0.01f, duration);
            while (elapsed < safeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / safeDuration);
                float eased = 1f - ((1f - t) * (1f - t));
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, eased);
                if (splashImageRect != null)
                {
                    splashImageRect.localScale = Vector3.Lerp(startScale, endScale, eased);
                }

                yield return null;
            }

            RestoreTimeScale();
            Destroy(canvasGroup.gameObject);
            canvasGroup = null;
        }

        private void RestoreTimeScale()
        {
            if (!pauseGameWhileVisible || restoredTimeScale)
            {
                return;
            }

            Time.timeScale = previousTimeScale;
            restoredTimeScale = true;
        }
    }
}
