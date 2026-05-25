using UnityEngine;

namespace EcoGarden.IAP
{
    public sealed class BackendIapReceiptValidator : MonoBehaviour, IIapReceiptValidator
    {
        [SerializeField] private bool requireValidation = true;
        [SerializeField] private string endpointUrl;

        public bool RequiresValidation { get { return requireValidation; } }
        public string EndpointUrl { get { return endpointUrl; } }

        public void SetEndpointUrl(string url)
        {
            endpointUrl = url ?? string.Empty;
        }

        public void SetRequiresValidation(bool required)
        {
            requireValidation = required;
        }

        public IapReceiptValidationResult Validate(IapReceiptValidationRequest request)
        {
            if (!RequiresValidation)
            {
                return IapReceiptValidationResult.NotRequired();
            }

            if (!request.HasRequiredPayload)
            {
                return IapReceiptValidationResult.InvalidRequest("Missing product id, transaction id, or receipt payload.");
            }

            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                return IapReceiptValidationResult.BackendUnavailable("Receipt validation endpoint is not configured.");
            }

            return IapReceiptValidationResult.BackendUnavailable("Backend receipt validation transport is not implemented yet.");
        }
    }
}
