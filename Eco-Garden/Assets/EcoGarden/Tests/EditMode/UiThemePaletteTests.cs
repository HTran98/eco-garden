using EcoGarden.UI;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class UiThemePaletteTests
    {
        [Test]
        public void MainTextPairsKeepReadableContrast()
        {
            Assert.GreaterOrEqual(UiThemePalette.ContrastRatio(UiThemePalette.TextDark, UiThemePalette.Panel), 4.5f);
            Assert.GreaterOrEqual(UiThemePalette.ContrastRatio(UiThemePalette.TextLight, UiThemePalette.TopBar), 4.5f);
            Assert.GreaterOrEqual(UiThemePalette.ContrastRatio(UiThemePalette.TextLight, UiThemePalette.PrimaryButton), 3f);
        }

        [Test]
        public void BackgroundTextChoiceMatchesPaletteLuminance()
        {
            Assert.AreEqual(UiThemePalette.TextDark, UiThemePalette.GetTextForBackground(UiThemePalette.Panel));
            Assert.AreEqual(UiThemePalette.TextLight, UiThemePalette.GetTextForBackground(UiThemePalette.TopBar));
        }
    }
}
