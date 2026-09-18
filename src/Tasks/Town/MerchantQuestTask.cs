using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using UnityEngine;
using InventoryScreen = Firebot.GameModel.Features.Inventory.Inventory;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     Progresses the daily quest "Merchant" (sell 10 items at the Exotic Merchant) - level 30 per
///     the wiki quest table. Never automated before.
///     1. Uses up any "instant gold" items (Pouch/Bucket/Crate/Pile of Gold) from the bag FIRST -
///        these convert to meteorites and should never be sold (per the user).
///     2. Sells exactly one of each of up to SellTarget different item types (never repeats a type
///        already sold this run - a first pass sold 10 of the same thing since a depleted-but-not-
///        empty slot keeps re-winning a plain "first available" scan). Live-confirmed, 2026-09-18:
///        "Midas' Touch" is NOT safe to sell (despite the wiki listing it among Exotic Merchant items)
///        - the user explicitly wants it kept, so it's excluded alongside the gold items.
///     3. Spends the resulting exotic coins on one upgrade - whichever is cheapest/first affordable
///        in the currently-displayed tree (not scanning across all ~25 trees for the globally
///        cheapest - the user confirmed picking the first available is fine, and Exotic Upgrades
///        aren't the primary progression lever Firestone/Meteorite Research are).
///     4. User-requested: immediately claims the (now-completed) "Merchant" quest afterward instead
///        of waiting for QuestsTask's own schedule - reuses the same generic claim-every-completed-
///        quest routine (safe no-op on anything not actually claimable yet).
/// </summary>
public class MerchantQuestTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;
    protected override int MinimumCharacterLevel => 30;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    // Exactly what the "Merchant" quest requires - selling more than this is pointless churn (and,
    // per the user, risks reaching into items that shouldn't be touched).
    private const int SellTarget = 10;

    // Never sell - gold items are used separately (see InventoryScreen.UseAllGoldItems), and the
    // user explicitly excluded Midas' Touch despite the wiki listing it as sellable here.
    private static readonly string[] NeverSellTerms = { "gold", "midas" };

    // Live-confirmed, 2026-09-18: same pooled-ScrollView populate delay as Collector Quest's chest
    // list (see CollectorQuestTask.ChestListPopulateDelay) - switching to the Items tab doesn't
    // populate its real content instantly, so scanning for gold items right away found nothing.
    private static readonly WaitForSeconds ItemListPopulateDelay = new(1.5f);

    public override IEnumerator Execute()
    {
        yield return InventoryScreen.Open;
        yield return InventoryScreen.OpenItemsTab;
        yield return ItemListPopulateDelay;
        yield return InventoryScreen.UseAllGoldItems();
        yield return InventoryScreen.Close;

        yield return TownScreen.Open;
        yield return TownScreen.OpenExoticMerchant;
        yield return ExoticMerchant.OpenSellItemsTab;

        // Live-confirmed, 2026-09-18: a plain "first non-excluded slot" re-scan kept re-selecting the
        // SAME item type (it stays in place, just with a lower quantity, until fully depleted),
        // selling 10 of one thing instead of one of each. Tracking already-sold names so every pick
        // is a different item type, matching what the user actually wants.
        var soldNames = new HashSet<string>();
        while (soldNames.Count < SellTarget)
        {
            var candidate = ExoticMerchant.SellProductGrid.GetChildren().FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Name) &&
                !soldNames.Contains(c.Name) &&
                !NeverSellTerms.Any(term => c.Name.ToLowerInvariant().Contains(term)));
            if (candidate == null) break; // nothing left worth selling

            var sellBtn = new GameButton(Paths.ExoticMerchantLoc.SellLoc.SellBtn, candidate);
            if (!sellBtn.IsClickable()) break;

            yield return sellBtn.Click();
            soldNames.Add(candidate.Name);
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

        if (soldNames.Count >= SellTarget)
        {
            yield return CharacterScreen.Open;
            yield return CharacterScreen.OpenQuestsTab;
            yield return CharacterScreen.OpenDailyQuestsSubTab;
            foreach (var claimButton in CharacterScreen.DailyQuestClaimButtons())
                yield return claimButton.Click();
            yield return CharacterScreen.Close;
        }

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
