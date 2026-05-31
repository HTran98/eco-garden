using EcoGarden.AI;
using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class DecorationControllerTests
    {
        [Test]
        public void ApplyOwnedDecorations_AppliesBoardAndNpcCosmetics()
        {
            GameObject controllerObject = new GameObject("DecorationController");
            GameObject backdropObject = new GameObject("BoardBackdrop", typeof(SpriteRenderer), typeof(BoardBackdropController));
            GameObject backgroundObject = new GameObject("EcoGardenBackground", typeof(SpriteRenderer), typeof(EcoGardenBackgroundController));
            GameObject npcObject = new GameObject("CustomerNpc", typeof(SpriteRenderer), typeof(NpcMovementController));
            try
            {
                DecorationController controller = controllerObject.AddComponent<DecorationController>();
                controller.ApplyOwnedDecorations(new[]
                {
                    DecorationController.BoardMossStoneId,
                    DecorationController.NpcTravelerId,
                    DecorationController.BackgroundLilyPondId
                });

                Color boardColor = backdropObject.GetComponent<SpriteRenderer>().color;
                Color backgroundColor = backgroundObject.GetComponent<SpriteRenderer>().color;
                Color npcColor = npcObject.GetComponent<SpriteRenderer>().color;
                Assert.AreNotEqual(Color.white, boardColor);
                Assert.AreEqual(new Color(1f, 0.94f, 0.84f, 1f), backgroundColor);
                Assert.AreEqual(new Color(0.42f, 0.74f, 0.94f, 1f), npcColor);
            }
            finally
            {
                Object.DestroyImmediate(npcObject);
                Object.DestroyImmediate(backgroundObject);
                Object.DestroyImmediate(backdropObject);
                Object.DestroyImmediate(controllerObject);
            }
        }

        [Test]
        public void ApplyOwnedDecorations_CreatesBeeAndExtraButterfly()
        {
            GameObject controllerObject = new GameObject("DecorationController");
            GameObject butterflyObject = new GameObject("ButterflyA", typeof(SpriteRenderer), typeof(ButterflyMovementController));
            try
            {
                DecorationController controller = controllerObject.AddComponent<DecorationController>();
                controller.ApplyOwnedDecorations(new[]
                {
                    DecorationController.ButterflyVariantId,
                    DecorationController.BeeVisitorId
                });

                Assert.IsNotNull(GameObject.Find("DecorButterflyVariant"));
                Assert.IsNotNull(GameObject.Find("DecorBeeVisitor"));
                Assert.AreNotEqual(new Color(1f, 0.72f, 0.36f, 1f), butterflyObject.GetComponent<SpriteRenderer>().color);
            }
            finally
            {
                GameObject decorButterfly = GameObject.Find("DecorButterflyVariant");
                GameObject bee = GameObject.Find("DecorBeeVisitor");
                if (decorButterfly != null)
                {
                    Object.DestroyImmediate(decorButterfly);
                }

                if (bee != null)
                {
                    Object.DestroyImmediate(bee);
                }

                Object.DestroyImmediate(butterflyObject);
                Object.DestroyImmediate(controllerObject);
            }
        }
    }
}
