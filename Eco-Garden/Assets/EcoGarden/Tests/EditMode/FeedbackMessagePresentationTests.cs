using EcoGarden.UI;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class FeedbackMessagePresentationTests
    {
        [Test]
        public void Classify_MapsCommonReleaseMessagesToSeverity()
        {
            Assert.AreEqual(FeedbackMessageSeverity.Success, FeedbackMessagePresentation.Classify("Mission reward claimed"));
            Assert.AreEqual(FeedbackMessageSeverity.Warning, FeedbackMessagePresentation.Classify("Not enough currency"));
            Assert.AreEqual(FeedbackMessageSeverity.Error, FeedbackMessagePresentation.Classify("Invalid target"));
            Assert.AreEqual(FeedbackMessageSeverity.Info, FeedbackMessagePresentation.Classify("Select target"));
        }

        [Test]
        public void DurationFor_KeepsWarningsAndErrorsReadableLongerThanInfo()
        {
            Assert.Greater(
                FeedbackMessagePresentation.DurationFor(FeedbackMessageSeverity.Warning),
                FeedbackMessagePresentation.DurationFor(FeedbackMessageSeverity.Info));
            Assert.Greater(
                FeedbackMessagePresentation.DurationFor(FeedbackMessageSeverity.Error),
                FeedbackMessagePresentation.DurationFor(FeedbackMessageSeverity.Info));
        }
    }
}
