using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Tests.EditMode
{
    public sealed class StartupSplashControllerTests
    {
        [Test]
        public void SplashResourcePath_UsesLotimoriArtwork()
        {
            Assert.AreEqual("Splash/startup_splash_lotimori_portrait", StartupSplashController.SplashResourcePath);
        }

        [Test]
        public void Show_CreatesBlockingSplashCanvas()
        {
            GameObject controllerObject = new GameObject("StartupSplashController");
            float previousTimeScale = Time.timeScale;
            try
            {
                StartupSplashController controller = controllerObject.AddComponent<StartupSplashController>();

                controller.Show();

                Canvas canvas = controllerObject.GetComponentInChildren<Canvas>();
                CanvasGroup canvasGroup = controllerObject.GetComponentInChildren<CanvasGroup>();
                Button skipButton = controllerObject.GetComponentInChildren<Button>();

                Assert.NotNull(canvas);
                Assert.NotNull(canvasGroup);
                Assert.NotNull(skipButton);
                Assert.AreEqual(RenderMode.ScreenSpaceOverlay, canvas.renderMode);
                Assert.GreaterOrEqual(canvas.sortingOrder, 30000);
                Assert.IsTrue(canvasGroup.blocksRaycasts);
                Assert.AreEqual(0f, Time.timeScale);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                Time.timeScale = previousTimeScale;
            }
        }
    }
}
