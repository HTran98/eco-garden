using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class DraggedItemCanvasGhost : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform ghostRoot;
        [SerializeField] private Image ghostImage;
        [SerializeField] private Text ghostLabel;
        [SerializeField] private float sellDuration = 0.24f;
        [SerializeField] private Vector2 startSize = new Vector2(88f, 88f);

        private Coroutine routine;

        private void Awake()
        {
            EnsureView();
            SetVisible(false);
        }

        public void PlaySell(
            Sprite sprite,
            Color color,
            int level,
            Vector2 startScreenPosition,
            Vector2 endScreenPosition,
            Action completed)
        {
            EnsureView();

            if (routine != null)
            {
                StopCoroutine(routine);
            }

            ghostImage.sprite = sprite;
            ghostImage.color = color;
            ghostLabel.text = level.ToString();
            ghostRoot.SetAsLastSibling();
            SetVisible(true);
            routine = StartCoroutine(PlaySellRoutine(startScreenPosition, endScreenPosition, completed));
        }

        public void ShowDrag(Sprite sprite, Color color, int level, Vector2 screenPosition)
        {
            EnsureView();

            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            color.a = 1f;
            ghostImage.sprite = sprite;
            ghostImage.color = color;
            ghostLabel.text = level.ToString();
            ghostLabel.color = Color.white;
            ghostRoot.localScale = Vector3.one;
            ghostRoot.SetAsLastSibling();
            SetVisible(true);
            MoveTo(screenPosition);
        }

        public void MoveTo(Vector2 screenPosition)
        {
            EnsureView();
            RectTransform canvasRect = canvas.transform as RectTransform;
            ghostRoot.anchoredPosition = ScreenToCanvasLocal(canvasRect, screenPosition);
            ghostRoot.SetAsLastSibling();
        }

        public void Hide()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            SetVisible(false);
            ghostRoot.localScale = Vector3.one;
        }

        private IEnumerator PlaySellRoutine(Vector2 startScreenPosition, Vector2 endScreenPosition, Action completed)
        {
            RectTransform canvasRect = canvas.transform as RectTransform;
            Vector2 startLocal = ScreenToCanvasLocal(canvasRect, startScreenPosition);
            Vector2 endLocal = ScreenToCanvasLocal(canvasRect, endScreenPosition);
            float elapsed = 0f;

            while (elapsed < sellDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / sellDuration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);

                ghostRoot.anchoredPosition = Vector2.Lerp(startLocal, endLocal, eased);
                float scale = Mathf.Lerp(1f, 0.18f, eased);
                ghostRoot.localScale = new Vector3(scale, scale, 1f);
                SetAlpha(1f - t);
                yield return null;
            }

            SetVisible(false);
            ghostRoot.localScale = Vector3.one;
            routine = null;
            completed?.Invoke();
        }

        private Vector2 ScreenToCanvasLocal(RectTransform canvasRect, Vector2 screenPosition)
        {
            Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, eventCamera, out Vector2 localPosition);
            return localPosition;
        }

        private void EnsureView()
        {
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }

            if (ghostRoot == null)
            {
                GameObject rootObject = new GameObject("DraggedItemGhost", typeof(RectTransform));
                rootObject.transform.SetParent(transform, false);
                ghostRoot = rootObject.GetComponent<RectTransform>();
                ghostRoot.anchorMin = new Vector2(0.5f, 0.5f);
                ghostRoot.anchorMax = new Vector2(0.5f, 0.5f);
                ghostRoot.pivot = new Vector2(0.5f, 0.5f);
                ghostRoot.sizeDelta = startSize;
            }

            if (ghostImage == null)
            {
                GameObject imageObject = new GameObject("Image", typeof(RectTransform), typeof(Image));
                imageObject.transform.SetParent(ghostRoot, false);
                RectTransform imageRect = imageObject.GetComponent<RectTransform>();
                imageRect.anchorMin = Vector2.zero;
                imageRect.anchorMax = Vector2.one;
                imageRect.offsetMin = Vector2.zero;
                imageRect.offsetMax = Vector2.zero;
                ghostImage = imageObject.GetComponent<Image>();
                ghostImage.raycastTarget = false;
            }

            if (ghostLabel == null)
            {
                GameObject labelObject = new GameObject("LevelLabel", typeof(RectTransform), typeof(Text));
                labelObject.transform.SetParent(ghostRoot, false);
                RectTransform labelRect = labelObject.GetComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;

                ghostLabel = labelObject.GetComponent<Text>();
                ghostLabel.alignment = TextAnchor.MiddleCenter;
                ghostLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                ghostLabel.fontSize = 48;
                ghostLabel.color = Color.white;
                ghostLabel.raycastTarget = false;
            }
        }

        private void SetVisible(bool visible)
        {
            if (ghostRoot != null)
            {
                ghostRoot.gameObject.SetActive(visible);
            }
        }

        private void SetAlpha(float alpha)
        {
            Color imageColor = ghostImage.color;
            imageColor.a = alpha;
            ghostImage.color = imageColor;

            Color labelColor = ghostLabel.color;
            labelColor.a = alpha;
            ghostLabel.color = labelColor;
        }
    }
}
