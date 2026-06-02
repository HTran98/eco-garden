using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Tests.EditMode
{
    public sealed class AbilityHudControllerTests
    {
        private GameObject root;
        private BoardController boardController;

        [TearDown]
        public void TearDown()
        {
            if (root != null)
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CountChanged_RefreshesHudButtonState()
        {
            root = new GameObject("AbilityHudControllerTests");
            root.AddComponent<BoardView>();
            boardController = root.AddComponent<BoardController>();
            boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());
            boardController.LoadLevel();
            boardController.SetAbilityCount(AbilityKind.Shovel, 0);

            Button shovelButton = CreateButton("ShovelButton");
            CreateButton("MagicWandButton");
            CreateButton("SortingMagnetButton");
            AbilityHudController controller = root.AddComponent<AbilityHudController>();
            controller.Refresh();

            Assert.IsFalse(shovelButton.interactable);
            Assert.AreEqual("x0", shovelButton.GetComponentInChildren<Text>().text);

            boardController.SetAbilityCount(AbilityKind.Shovel, 3);

            Assert.IsTrue(shovelButton.interactable);
            Assert.AreEqual("x3", shovelButton.GetComponentInChildren<Text>().text);
        }

        private Button CreateButton(string objectName)
        {
            GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(root.transform, false);
            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(buttonObject.transform, false);
            Text label = labelObject.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = string.Empty;
            return buttonObject.GetComponent<Button>();
        }
    }
}
