using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Tests.EditMode
{
    public sealed class GameplayFeedbackControllerTests
    {
        [Test]
        public void PlayHudMessage_RaisesFeedbackAboveModalSiblings()
        {
            GameObject root = new GameObject("HUDRoot", typeof(RectTransform));
            GameObject feedback = CreateText("FeedbackText", root.transform);
            GameObject panel = new GameObject("ShopPanel", typeof(RectTransform));
            panel.transform.SetParent(root.transform, false);
            GameObject controllerObject = new GameObject("GameplayFeedbackController");
            try
            {
                GameplayFeedbackController controller = controllerObject.AddComponent<GameplayFeedbackController>();

                controller.PlayHudMessage("Purchased");

                Assert.AreEqual(root.transform.childCount - 1, feedback.transform.GetSiblingIndex());
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                Object.DestroyImmediate(root);
            }
        }

        private static GameObject CreateText(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            Text text = gameObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return gameObject;
        }
    }
}
