using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Input
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class ExternalDropZone : MonoBehaviour
    {
        [SerializeField] private ExternalDropZoneKind zoneKind;
        [SerializeField] private Image image;
        [SerializeField] private Color normalColor = new Color(0.30f, 0.44f, 0.34f, 0.92f);
        [SerializeField] private Color highlightedColor = new Color(0.48f, 0.70f, 0.40f, 1f);

        public ExternalDropZoneKind ZoneKind { get { return zoneKind; } }
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            SetHighlighted(false);
        }

        public bool ContainsScreenPoint(Vector2 screenPosition, Camera uiCamera)
        {
            if (RectTransform == null)
            {
                RectTransform = GetComponent<RectTransform>();
            }

            return RectTransformUtility.RectangleContainsScreenPoint(RectTransform, screenPosition, uiCamera);
        }

        public Vector3 GetWorldCenter(Camera uiCamera)
        {
            if (RectTransform == null)
            {
                RectTransform = GetComponent<RectTransform>();
            }

            Vector3[] corners = new Vector3[4];
            RectTransform.GetWorldCorners(corners);
            return (corners[0] + corners[2]) * 0.5f;
        }

        public Vector2 GetScreenCenter(Camera uiCamera)
        {
            Vector3 worldCenter = GetWorldCenter(uiCamera);
            return RectTransformUtility.WorldToScreenPoint(uiCamera, worldCenter);
        }

        public void SetHighlighted(bool highlighted)
        {
            if (image != null)
            {
                image.color = highlighted ? highlightedColor : normalColor;
            }
        }
    }
}
