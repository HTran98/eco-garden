using UnityEngine;

namespace EcoGarden.UI
{
    public static class FeedbackMessagePresentation
    {
        public const float InfoDuration = 1.0f;
        public const float SuccessDuration = 1.05f;
        public const float WarningDuration = 1.25f;
        public const float ErrorDuration = 1.35f;
        public const float DuplicateSuppressSeconds = 0.22f;

        public static FeedbackMessageSeverity Classify(string message)
        {
            string value = message ?? string.Empty;
            if (Contains(value, "failed") ||
                Contains(value, "invalid") ||
                Contains(value, "cannot") ||
                Contains(value, "not found") ||
                Contains(value, "unavailable"))
            {
                return FeedbackMessageSeverity.Error;
            }

            if (Contains(value, "locked") ||
                Contains(value, "blocked") ||
                Contains(value, "cancelled") ||
                Contains(value, "not enough") ||
                Contains(value, "no uses") ||
                Contains(value, "need") ||
                Contains(value, "pending"))
            {
                return FeedbackMessageSeverity.Warning;
            }

            if (Contains(value, "purchased") ||
                Contains(value, "claimed") ||
                Contains(value, "delivered") ||
                Contains(value, "sold") ||
                Contains(value, "merged") ||
                Contains(value, "complete"))
            {
                return FeedbackMessageSeverity.Success;
            }

            return FeedbackMessageSeverity.Info;
        }

        public static Color ColorFor(FeedbackMessageSeverity severity)
        {
            switch (severity)
            {
                case FeedbackMessageSeverity.Success:
                    return new Color(0.72f, 0.96f, 0.62f, 1f);
                case FeedbackMessageSeverity.Warning:
                    return new Color(1f, 0.86f, 0.42f, 1f);
                case FeedbackMessageSeverity.Error:
                    return new Color(1f, 0.48f, 0.42f, 1f);
                default:
                    return new Color(0.95f, 0.96f, 1f, 1f);
            }
        }

        public static float DurationFor(FeedbackMessageSeverity severity)
        {
            switch (severity)
            {
                case FeedbackMessageSeverity.Success:
                    return SuccessDuration;
                case FeedbackMessageSeverity.Warning:
                    return WarningDuration;
                case FeedbackMessageSeverity.Error:
                    return ErrorDuration;
                default:
                    return InfoDuration;
            }
        }

        private static bool Contains(string value, string token)
        {
            return value.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
