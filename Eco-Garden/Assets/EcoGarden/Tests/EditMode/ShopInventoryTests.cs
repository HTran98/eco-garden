using EcoGarden.Shop;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class ShopInventoryTests
    {
        [Test]
        public void UseDecoration_RequiresOwnedDecoration()
        {
            ShopInventory inventory = new ShopInventory();

            bool used = inventory.UseDecoration("deco_bee_visitor");

            Assert.IsFalse(used);
            Assert.IsFalse(inventory.IsDecorationActive("deco_bee_visitor"));
        }

        [Test]
        public void UseDecoration_MarksOwnedDecorationActive()
        {
            ShopInventory inventory = new ShopInventory();
            inventory.AddDecorations(new[] { "deco_bee_visitor" });

            bool used = inventory.UseDecoration("deco_bee_visitor");

            Assert.IsTrue(used);
            Assert.IsTrue(inventory.IsDecorationActive("deco_bee_visitor"));
        }

        [Test]
        public void UseDecorationExclusive_DeactivatesOtherDecorationInGroup()
        {
            ShopInventory inventory = new ShopInventory();
            inventory.AddDecorations(new[] { "skin_npc_merchant", "skin_npc_moon" });
            inventory.UseDecoration("skin_npc_merchant");

            bool used = inventory.UseDecorationExclusive(
                "skin_npc_moon",
                new[] { "skin_npc_merchant", "skin_npc_moon" });

            Assert.IsTrue(used);
            Assert.IsFalse(inventory.IsDecorationActive("skin_npc_merchant"));
            Assert.IsTrue(inventory.IsDecorationActive("skin_npc_moon"));
        }

        [Test]
        public void RemoveDecoration_DeactivatesOwnedDecoration()
        {
            ShopInventory inventory = new ShopInventory();
            inventory.AddDecorations(new[] { "skin_background_moon_lotus" });
            inventory.UseDecoration("skin_background_moon_lotus");

            bool removed = inventory.RemoveDecoration("skin_background_moon_lotus");

            Assert.IsTrue(removed);
            Assert.IsFalse(inventory.IsDecorationActive("skin_background_moon_lotus"));
        }

        [Test]
        public void Restore_IgnoresActiveDecorationsThatAreNotOwned()
        {
            ShopInventory inventory = new ShopInventory();

            inventory.Restore(null, new[] { "skin_board_moss_stone" }, new[] { "deco_bee_visitor" });

            Assert.IsTrue(inventory.IsDecorationOwned("skin_board_moss_stone"));
            Assert.IsFalse(inventory.IsDecorationActive("deco_bee_visitor"));
        }
    }
}
