using EcoGarden.IAP;
using EcoGarden.Shop;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EcoGarden.Editor
{
    public static class EcoGardenIapCatalogAudit
    {
        private const string ShopAssetFolder = "Assets/EcoGarden/ScriptableObjects/Shop";

        [MenuItem("Eco Garden/Validation/Audit IAP Catalog")]
        public static void AuditIapCatalog()
        {
            List<string> issues = AuditIapCatalogAssets();
            if (issues.Count > 0)
            {
                string message = "Eco Garden IAP catalog audit failed:\n- " + string.Join("\n- ", issues);
                Debug.LogError(message);
                throw new InvalidOperationException(message);
            }

            Debug.Log("Eco Garden IAP catalog audit passed.");
        }

        public static List<string> AuditIapCatalogAssets()
        {
            List<string> issues = new List<string>();
            HashSet<string> requiredIds = new HashSet<string>(IapProductIds.CreateRequiredConsumableIds());
            HashSet<string> foundIds = new HashSet<string>();
            HashSet<string> duplicateIds = new HashSet<string>();

            string[] guids = AssetDatabase.FindAssets("t:ShopItemDefinition", new[] { ShopAssetFolder });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                ShopItemDefinition item = AssetDatabase.LoadAssetAtPath<ShopItemDefinition>(path);
                if (item == null || item.Price == null || item.Price.PurchaseKind != ShopPurchaseKind.Iap)
                {
                    continue;
                }

                string storeProductId = item.Price.IapProductId;
                if (string.IsNullOrWhiteSpace(storeProductId))
                {
                    issues.Add(item.name + " has an empty IAP product id.");
                    continue;
                }

                if (!requiredIds.Contains(storeProductId))
                {
                    issues.Add(item.name + " uses undocumented IAP product id: " + storeProductId);
                }

                if (!foundIds.Add(storeProductId))
                {
                    duplicateIds.Add(storeProductId);
                }
            }

            foreach (string duplicateId in duplicateIds)
            {
                issues.Add("Duplicate IAP product id in shop catalog: " + duplicateId);
            }

            foreach (string requiredId in requiredIds)
            {
                if (!foundIds.Contains(requiredId))
                {
                    issues.Add("Missing required IAP product id in shop catalog: " + requiredId);
                }
            }

            return issues;
        }
    }
}
