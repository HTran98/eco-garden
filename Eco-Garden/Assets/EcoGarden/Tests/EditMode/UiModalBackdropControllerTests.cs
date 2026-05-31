using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Tests.EditMode
{
    public sealed class UiModalBackdropControllerTests
    {
        [Test]
        public void RefreshBackdrop_ShowsRaycastBlockerBehindVisibleModal()
        {
            GameObject root = new GameObject("HUDRoot", typeof(RectTransform));
            GameObject shopPanel = new GameObject("ShopPanel", typeof(RectTransform));
            try
            {
                shopPanel.transform.SetParent(root.transform, false);
                shopPanel.SetActive(true);

                UiModalBackdropController controller = root.AddComponent<UiModalBackdropController>();
                controller.RefreshBackdrop();

                Transform backdrop = root.transform.Find("ModalBackdrop");
                Assert.IsNotNull(backdrop);
                Assert.IsTrue(backdrop.gameObject.activeSelf);
                Assert.Less(backdrop.GetSiblingIndex(), shopPanel.transform.GetSiblingIndex());

                Image image = backdrop.GetComponent<Image>();
                Assert.IsNotNull(image);
                Assert.IsTrue(image.raycastTarget);
                Assert.Greater(image.color.a, 0.3f);

                Button button = backdrop.GetComponent<Button>();
                Assert.IsNotNull(button);
                Assert.AreEqual(Selectable.Transition.None, button.transition);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void RefreshBackdrop_HidesBlockerWhenNoModalIsVisible()
        {
            GameObject root = new GameObject("HUDRoot", typeof(RectTransform));
            GameObject pausePanel = new GameObject("PausePanel", typeof(RectTransform));
            try
            {
                pausePanel.transform.SetParent(root.transform, false);
                pausePanel.SetActive(false);

                UiModalBackdropController controller = root.AddComponent<UiModalBackdropController>();
                controller.RefreshBackdrop();

                Transform backdrop = root.transform.Find("ModalBackdrop");
                Assert.IsNotNull(backdrop);
                Assert.IsFalse(backdrop.gameObject.activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void BackdropClick_DismissesNavigationPanelsButKeepsPauseAndResultPanels()
        {
            GameObject root = new GameObject("HUDRoot", typeof(RectTransform));
            GameObject shopPanel = new GameObject("ShopPanel", typeof(RectTransform));
            GameObject pausePanel = new GameObject("PausePanel", typeof(RectTransform));
            GameObject resultPanel = new GameObject("ResultPanel", typeof(RectTransform));
            try
            {
                shopPanel.transform.SetParent(root.transform, false);
                pausePanel.transform.SetParent(root.transform, false);
                resultPanel.transform.SetParent(root.transform, false);
                shopPanel.SetActive(true);
                pausePanel.SetActive(true);
                resultPanel.SetActive(true);

                UiModalBackdropController controller = root.AddComponent<UiModalBackdropController>();
                controller.RefreshBackdrop();

                Button backdropButton = root.transform.Find("ModalBackdrop").GetComponent<Button>();
                backdropButton.onClick.Invoke();

                Assert.IsFalse(shopPanel.activeSelf);
                Assert.IsTrue(pausePanel.activeSelf);
                Assert.IsTrue(resultPanel.activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void RefreshBackdrop_MovesBehindTopmostVisibleModal()
        {
            GameObject root = new GameObject("HUDRoot", typeof(RectTransform));
            GameObject shopPanel = new GameObject("ShopPanel", typeof(RectTransform));
            GameObject missionPanel = new GameObject("MissionPanel", typeof(RectTransform));
            try
            {
                shopPanel.transform.SetParent(root.transform, false);
                missionPanel.transform.SetParent(root.transform, false);
                shopPanel.SetActive(true);
                missionPanel.SetActive(true);
                shopPanel.transform.SetAsLastSibling();

                UiModalBackdropController controller = root.AddComponent<UiModalBackdropController>();
                controller.RefreshBackdrop();

                Transform backdrop = root.transform.Find("ModalBackdrop");
                Assert.AreEqual(shopPanel.transform.GetSiblingIndex() - 1, backdrop.GetSiblingIndex());
                Assert.Greater(shopPanel.transform.GetSiblingIndex(), missionPanel.transform.GetSiblingIndex());
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
