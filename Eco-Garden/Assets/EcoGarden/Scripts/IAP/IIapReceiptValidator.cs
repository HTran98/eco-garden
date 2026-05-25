namespace EcoGarden.IAP
{
    public interface IIapReceiptValidator
    {
        bool RequiresValidation { get; }
        IapReceiptValidationResult Validate(IapReceiptValidationRequest request);
    }
}
