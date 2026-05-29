using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class UiAmbientPulse : MonoBehaviour
    {
        private const string PulseChildName = "RuntimePulseGlow";

        [SerializeField] private Color pulseColor = Color.white;
        [SerializeField] private float minAlpha = 0.08f;
        [SerializeField] private float maxAlpha = 0.22f;
        [SerializeField] private float pulseScale = 1.08f;
        [SerializeField] private float pulseSpeed = 2.2f;

        private Image pulseImage;
        private RectTransform pulseRect;

        private void OnEnable()
        {
            EnsurePulseImage();
        }

        private void Update()
        {
            if (pulseImage == null)
            {
                EnsurePulseImage();
            }

            float phase = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
            Color color = pulseColor;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, phase);
            pulseImage.color = color;

            float scale = Mathf.Lerp(1f, pulseScale, phase);
            pulseRect.localScale = new Vector3(scale, scale, 1f);
        }

        public void Configure(Color color, float minimumAlpha, float maximumAlpha, float speed)
        {
            pulseColor = color;
            minAlpha = minimumAlpha;
            maxAlpha = maximumAlpha;
            pulseSpeed = speed;
            EnsurePulseImage();
        }

        private void EnsurePulseImage()
        {
            Transform existing = transform.Find(PulseChildName);
            GameObject pulseObject = existing != null
                ? existing.gameObject
                : new GameObject(PulseChildName, typeof(RectTransform), typeof(Image));
            pulseObject.transform.SetParent(transform, false);
            pulseObject.transform.SetAsFirstSibling();

            pulseRect = pulseObject.GetComponent<RectTransform>();
            pulseRect.anchorMin = Vector2.zero;
            pulseRect.anchorMax = Vector2.one;
            pulseRect.offsetMin = new Vector2(-8f, -8f);
            pulseRect.offsetMax = new Vector2(8f, 8f);
            pulseRect.localScale = Vector3.one;

            pulseImage = pulseObject.GetComponent<Image>();
            Image sourceImage = GetComponent<Image>();
            pulseImage.sprite = sourceImage != null ? sourceImage.sprite : null;
            pulseImage.type = sourceImage != null ? sourceImage.type : Image.Type.Simple;
            pulseImage.preserveAspect = sourceImage != null && sourceImage.preserveAspect;
            pulseImage.raycastTarget = false;
        }
    }
}
