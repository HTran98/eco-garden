using EcoGarden.Editor;
using NUnit.Framework;
using System.Collections.Generic;

namespace EcoGarden.Tests.EditMode
{
    public sealed class IapCatalogAuditTests
    {
        [Test]
        public void ShopCatalog_UsesRequiredGooglePlayProductIds()
        {
            List<string> issues = EcoGardenIapCatalogAudit.AuditIapCatalogAssets();

            Assert.IsEmpty(issues, string.Join("\n", issues));
        }
    }
}
