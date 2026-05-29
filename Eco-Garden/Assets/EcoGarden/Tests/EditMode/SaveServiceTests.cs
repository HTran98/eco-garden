using EcoGarden.Save;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class SaveServiceTests
    {
        [Test]
        public void Normalize_AddsSafeDefaultsForPartialOldSave()
        {
            SaveData data = SaveService.Normalize(new SaveData());

            Assert.AreEqual(SaveService.CurrentSchemaVersion, data.schemaVersion);
            Assert.AreEqual(1, data.highestUnlockedLevel);
            Assert.IsTrue(data.soundEnabled);
            Assert.IsTrue(data.musicEnabled);
            Assert.IsNotNull(data.boardItems);
            Assert.IsNotNull(data.clearedObstacles);
            Assert.IsNotNull(data.orderRequirements);
            Assert.IsNotNull(data.plantTierUnlocks);
            Assert.IsNotNull(data.purchasedShopProductIds);
            Assert.IsNotNull(data.ownedDecorationIds);
            Assert.IsNotNull(data.processedIapTransactionIds);
            Assert.IsNotNull(data.missionProgress);
        }

        [Test]
        public void Normalize_PreservesExistingProcessedIapTransactions()
        {
            SaveData data = new SaveData
            {
                schemaVersion = SaveService.CurrentSchemaVersion,
                highestUnlockedLevel = 3,
                processedIapTransactionIds = new[] { "tx_1", "tx_2" },
                soundEnabled = false,
                musicEnabled = false
            };

            SaveData normalized = SaveService.Normalize(data);

            Assert.AreEqual(3, normalized.highestUnlockedLevel);
            CollectionAssert.AreEqual(new[] { "tx_1", "tx_2" }, normalized.processedIapTransactionIds);
            Assert.IsFalse(normalized.soundEnabled);
            Assert.IsFalse(normalized.musicEnabled);
        }
    }
}
