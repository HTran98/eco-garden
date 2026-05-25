namespace EcoGarden.IAP
{
    public readonly struct IapReceiptValidationResult
    {
        public IapReceiptValidationResult(
            IapReceiptValidationStatus status,
            string serverTransactionId = "",
            string message = "")
        {
            Status = status;
            ServerTransactionId = serverTransactionId;
            Message = message;
        }

        public IapReceiptValidationStatus Status { get; }
        public string ServerTransactionId { get; }
        public string Message { get; }
        public bool Approved { get { return Status == IapReceiptValidationStatus.Approved; } }

        public static IapReceiptValidationResult NotRequired()
        {
            return new IapReceiptValidationResult(IapReceiptValidationStatus.NotRequired);
        }

        public static IapReceiptValidationResult Approve(string serverTransactionId = "")
        {
            return new IapReceiptValidationResult(IapReceiptValidationStatus.Approved, serverTransactionId);
        }

        public static IapReceiptValidationResult Reject(string message)
        {
            return new IapReceiptValidationResult(IapReceiptValidationStatus.Rejected, string.Empty, message);
        }

        public static IapReceiptValidationResult BackendUnavailable(string message)
        {
            return new IapReceiptValidationResult(IapReceiptValidationStatus.BackendUnavailable, string.Empty, message);
        }

        public static IapReceiptValidationResult InvalidRequest(string message)
        {
            return new IapReceiptValidationResult(IapReceiptValidationStatus.InvalidRequest, string.Empty, message);
        }
    }
}
