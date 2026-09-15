using System;
using System.Collections;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using InventoryScreen = Firebot.GameModel.Features.Inventory.Inventory;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     Progresses the daily quest "Merchant" (sell 10 items at the Exotic Merchant) - level 30 per
///     the wiki quest table. No v1 precedent - never automated before.
///     1. Uses up any "instant gold" items (Pouch/Bucket/Crate/Pile of Gold) from the bag FIRST -
///        these convert to meteorites and should never be sold (per the user).
///     2. Sells one of every other sellable item (scrolls + inventory items) - always the single x1
///        sell action each item offers, easily clears the 10-item requirement given how many
///        sellable types typically exist. Checked against the wiki's full Exotic Merchant sell list
///        (Scroll of Speed/Damage/Health, Midas' Touch, War Banner, Dragon Armor, Guardian's Rune,
///        Totem of Agony/Annihilation, plus the gold items above): every one of them is a timed
///        combat buff/debuff or the already-excluded gold conversion items, never permanent Gold
///        Gain equipment (that's the separate "Gear" system per the wiki's own category listing,
///        which this screen never sells) - selling everything else here is safe as-is.
///     3. Spends the resulting exotic coins on one upgrade - whichever is cheapest/first affordable
///        in the currently-displayed tree (not scanning across all ~25 trees for the globally
///        cheapest - the user confirmed picking the first available is fine, and Exotic Upgrades
///        aren't the primary progression lever Firestone/Meteorite Research are).
/// </summary>
public class MerchantQuestTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;
    protected override int MinimumCharacterLevel => 30;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    public override IEnumerator Execute()
    {
        yield return InventoryScreen.Open;
        yield return InventoryScreen.OpenItemsTab;
        yield return InventoryScreen.UseAllGoldItems();
        yield return InventoryScreen.Close;

        yield return TownScreen.Open;
        yield return TownScreen.OpenExoticMerchant;
        yield return ExoticMerchant.OpenSellItemsTab;

        var sellSlotNames = ExoticMerchant.SellProductGrid.GetChildren().Select(c => c.Name).ToList();
        foreach (var name in sellSlotNames)
        {
            if (string.IsNullOrEmpty(name)) continue;
            if (name.ToLowerInvariant().Contains("gold")) continue; // never sell - see class doc

            var sellBtn = new GameButton(
                "/" + name + Paths.ExoticMerchantLoc.SellLoc.SellBtn, ExoticMerchant.SellProductGrid);
            if (sellBtn.IsClickable()) yield return sellBtn.Click();
        }

        yield return ExoticMerchant.OpenUpgradesTab;

        var upgradeSlotNames = ExoticMerchant.UpgradesList.GetChildren().Select(c => c.Name).ToList();
        foreach (var name in upgradeSlotNames)
        {
            if (string.IsNullOrEmpty(name)) continue;

            var upgradeBtn = new GameButton(
                "/" + name + Paths.ExoticMerchantLoc.UpgradesLoc.UpgradeBtn, ExoticMerchant.UpgradesList);
            if (upgradeBtn.IsClickable())
            {
                yield return upgradeBtn.Click();
                break; // one upgrade per run is enough - see class doc
            }
        }

        yield return ExoticMerchant.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
