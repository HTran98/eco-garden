using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class UiModalBackdropController : MonoBehaviour
    {
        private Image backdropImage;
        private Button backdropButton;

        private void Awake()
        {
            EnsureBackdrop();
            RefreshBackdrop();
        }

        private void LateUpdate()
        {
            RefreshBackdrop();
        }

        public void RefreshBackdrop()
        {
            EnsureBackdrop();
            bool visible = HasVisibleModalPanel();
            if (backdropImage != null)
            {
                backdropImage.gameObject.SetActive(visible);
                if (visible)
                {
                    MoveBehindFirstVisibleModal();
                }
            }
        }

        private void EnsureBackdrop()
        {
            if (backdropImage != null)
            {
                return;
            }

            Transform existing = transform.Find("ModalBackdrop");
            GameObject backdropObject = existing != null
                ? existing.gameObject
                : new GameObject("ModalBackdrop", typeof(RectTransform), typeof(Image), typeof(Button));
            backdropObject.transform.SetParent(transform, false);

            RectTransform rect = backdropObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            backdropImage = backdropObject.GetComponent<Image>();
            backdropImage.sprite = null;
            backdropImage.color = new Color(0.03f, 0.07f, 0.06f, 0.42f);
            backdropImage.raycastTarget = true;

            backdropButton = backdropObject.GetComponent<Button>();
            backdropButton.transition = Selectable.Transition.None;
            backdropButton.onClick.RemoveListener(DismissBackdropPanels);
            backdropButton.onClick.AddListener(DismissBackdropPanels);
            backdropObject.SetActive(false);
        }

        private void DismissBackdropPanels()
        {
            UiModalPanelUtility.HideBackdropDismissiblePanels();
            RefreshBackdrop();
        }

        private bool HasVisibleModalPanel()
        {
            for (int i = 0; i < UiModalPanelUtility.ModalPanelNames.Length; i++)
            {
                GameObject panel = UiModalPanelUtility.FindObjectIncludingInactive(UiModalPanelUtility.ModalPanelNames[i]);
                if (panel != null && panel.activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }

        private void MoveBehindFirstVisibleModal()
        {
            if (backdropImage == null)
            {
                return;
            }

            GameObject topPanel = null;
            int topSiblingIndex = -1;
            for (int i = 0; i < UiModalPanelUtility.ModalPanelNames.Length; i++)
            {
                GameObject panel = UiModalPanelUtility.FindObjectIncludingInactive(UiModalPanelUtility.ModalPanelNames[i]);
                if (panel == null || !panel.activeInHierarchy || panel.transform.parent != transform)
                {
                    continue;
                }

                int siblingIndex = panel.transform.GetSiblingIndex();
                if (siblingIndex > topSiblingIndex)
                {
                    topSiblingIndex = siblingIndex;
                    topPanel = panel;
                }
            }

            if (topPanel != null)
            {
                int targetIndex = Mathf.Max(0, topPanel.transform.GetSiblingIndex() - 1);
                backdropImage.transform.SetSiblingIndex(targetIndex);
                return;
            }

            backdropImage.transform.SetAsFirstSibling();
        }
    }
}
