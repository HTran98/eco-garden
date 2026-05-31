using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class UiModalPanelUtilityTests
    {
        [Test]
        public void HideOtherModalPanels_KeepsRequestedPanelAndClosesSiblings()
        {
            GameObject shopPanel = new GameObject("ShopPanel");
            GameObject missionPanel = new GameObject("MissionPanel");
            GameObject levelPanel = new GameObject("LevelPanel");
            try
            {
                shopPanel.SetActive(true);
                missionPanel.SetActive(true);
                levelPanel.SetActive(true);

                UiModalPanelUtility.HideOtherModalPanels("MissionPanel");

                Assert.IsFalse(shopPanel.activeSelf);
                Assert.IsTrue(missionPanel.activeSelf);
                Assert.IsFalse(levelPanel.activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(levelPanel);
                Object.DestroyImmediate(missionPanel);
                Object.DestroyImmediate(shopPanel);
            }
        }

        [Test]
        public void HideBackdropDismissiblePanels_ClosesNavigationPanelsOnly()
        {
            GameObject shopPanel = new GameObject("ShopPanel");
            GameObject levelPanel = new GameObject("LevelPanel");
            GameObject pausePanel = new GameObject("PausePanel");
            GameObject resultPanel = new GameObject("ResultPanel");
            try
            {
                shopPanel.SetActive(true);
                levelPanel.SetActive(true);
                pausePanel.SetActive(true);
                resultPanel.SetActive(true);

                UiModalPanelUtility.HideBackdropDismissiblePanels();

                Assert.IsFalse(shopPanel.activeSelf);
                Assert.IsFalse(levelPanel.activeSelf);
                Assert.IsTrue(pausePanel.activeSelf);
                Assert.IsTrue(resultPanel.activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(resultPanel);
                Object.DestroyImmediate(pausePanel);
                Object.DestroyImmediate(levelPanel);
                Object.DestroyImmediate(shopPanel);
            }
        }

        [Test]
        public void RaiseModalPanel_MovesPanelToTopSibling()
        {
            GameObject root = new GameObject("HUDRoot", typeof(RectTransform));
            GameObject shopPanel = new GameObject("ShopPanel", typeof(RectTransform));
            GameObject missionPanel = new GameObject("MissionPanel", typeof(RectTransform));
            GameObject topBar = new GameObject("TopBar", typeof(RectTransform));
            try
            {
                shopPanel.transform.SetParent(root.transform, false);
                missionPanel.transform.SetParent(root.transform, false);
                topBar.transform.SetParent(root.transform, false);

                UiModalPanelUtility.RaiseModalPanel(shopPanel);

                Assert.AreEqual(root.transform.childCount - 1, shopPanel.transform.GetSiblingIndex());
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
