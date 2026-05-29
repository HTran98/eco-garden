using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Input
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class ExternalDropZone : MonoBehaviour
    {
        private static readonly List<ExternalDropZone> ActiveZones = new List<ExternalDropZone>();

        [SerializeField] private ExternalDropZoneKind zoneKind;
        [SerializeField] private Image image;
        [SerializeField] private Color normalColor = new Color(0.30f, 0.44f, 0.34f, 0.92f);
        [SerializeField] private Color highlightedColor = new Color(0.48f, 0.70f, 0.40f, 1f);
        [SerializeField] private float highlightedScale = 1.045f;

        private readonly Vector3[] worldCorners = new Vector3[4];
        private Vector3 baseScale = Vector3.one;
        private bool baseScaleCached;

        public ExternalDropZoneKind ZoneKind { get { return zoneKind; } }
        public RectTransform RectTransform { get; private set; }

        public static bool TryGetAtScreenPosition(Vector2 screenPosition, Camera uiCamera, out ExternalDropZone dropZone)
        {
            for (int i = ActiveZones.Count - 1; i >= 0; i--)
            {
                ExternalDropZone zone = ActiveZones[i];
                if (zone == null)
                {
                    ActiveZones.RemoveAt(i);
                    continue;
                }

                if (zone.isActiveAndEnabled && zone.ContainsScreenPoint(screenPosition, uiCamera))
                {
                    dropZone = zone;
                    return true;
                }
            }

            dropZone = null;
            return false;
        }

        public static bool TryGetFirst(ExternalDropZoneKind kind, out ExternalDropZone dropZone)
        {
            for (int i = ActiveZones.Count - 1; i >= 0; i--)
            {
                ExternalDropZone zone = ActiveZones[i];
                if (zone == null)
                {
                    ActiveZones.RemoveAt(i);
                    continue;
                }

                if (zone.isActiveAndEnabled && zone.ZoneKind == kind)
                {
                    dropZone = zone;
                    return true;
                }
            }

            dropZone = null;
            return false;
        }

        public void Configure(ExternalDropZoneKind kind, Color normal, Color highlighted)
        {
            zoneKind = kind;
            normalColor = normal;
            highlightedColor = highlighted;

            if (image == null)
            {
                image = GetComponent<Image>();
            }

            SetHighlighted(false);
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            CacheBaseScale();
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            ApplyTextPresentation();
            SetHighlighted(false);
        }

        private void OnEnable()
        {
            if (!ActiveZones.Contains(this))
            {
                ActiveZones.Add(this);
            }
        }

        private void OnDisable()
        {
            ActiveZones.Remove(this);
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

            RectTransform.GetWorldCorners(worldCorners);
            return (worldCorners[0] + worldCorners[2]) * 0.5f;
        }

        public Vector2 GetScreenCenter(Camera uiCamera)
        {
            Vector3 worldCenter = GetWorldCenter(uiCamera);
            return RectTransformUtility.WorldToScreenPoint(uiCamera, worldCenter);
        }

        public void SetHighlighted(bool highlighted)
        {
            CacheBaseScale();
            if (image != null)
            {
                image.color = highlighted ? highlightedColor : normalColor;
            }

            transform.localScale = highlighted ? baseScale * highlightedScale : baseScale;
        }

        private void CacheBaseScale()
        {
            if (baseScaleCached)
            {
                return;
            }

            baseScale = transform.localScale;
            baseScaleCached = true;
        }

        private void ApplyTextPresentation()
        {
            Text[] texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                Text text = texts[i];
                text.alignment = TextAnchor.MiddleCenter;
                Shadow shadow = text.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = text.gameObject.AddComponent<Shadow>();
                }

                shadow.effectColor = new Color(0.02f, 0.08f, 0.04f, 0.45f);
                shadow.effectDistance = new Vector2(1.1f, -1.1f);
                shadow.useGraphicAlpha = true;
            }
        }
    }
}
