using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.Infrastructure;
using MelonLoader;
using ChestOpening = Firebot.GameModel.Features.Inventory.ChestOpening;
using InventoryScreen = Firebot.GameModel.Features.Inventory.Inventory;

namespace Firebot.Tasks.Inventory;

/// <summary>
///     Progresses the daily quest "Collector" (open 4 gear chests) by opening every chest in the bag
///     except a reserve of common gear chests (kept so the quest is never blocked the next day too -
///     see min_common_reserve). No v1 precedent - chest opening was never automated before.
///     Claiming the quest reward itself is QuestsTask's job (Task 16); this only needs to make the
///     underlying objective progress, which the wiki says updates immediately, no extra step needed.
///     Only "commonChestbox" has a confirmed exact name (via UnityPy) among the 7 gear chest
///     rarities - the other 6 (uncommon..titan) have no uniquely-named template found statically, so
///     this scans every OTHER slot in the chests tab and opens it fully, skipping only the known
///     non-chest slots (mystery box claim, stat consumables, etc - see Paths.InventoryLoc). Flag for
///     live verification: if a rarity slot has a different real name than assumed, it's still
///     reachable through this generic scan since nothing here depends on knowing the name in advance.
///     Also opens jewel/celestial chests this way (on the user's request, since those otherwise pile
///     up unopened - notably from Pharaoh's Vault rewards) even though they don't count toward the
///     "Collector" quest itself, which only requires gear chests per the wiki - simplest to fold into
///     this same generic scan rather than a separate task.
/// </summary>
public class CollectorQuestTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    private MelonPreferences_Entry<int> _minCommonReserve;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_minCommonReserve != null) return;

        _minCommonReserve = category.CreateEntry(
            "min_common_chest_reserve",
            10,
            "Minimum Common Chest Reserve",
            "Common gear chests are never opened below this count, so there's always at least one " +
            "left to open for tomorrow's Collector quest too. Default: 10."
        );
    }

    public override IEnumerator Execute()
    {
        yield return InventoryScreen.Open;
        yield return InventoryScreen.OpenChestsTab;

        var slotNames = InventoryScreen.Content.GetChildren().Select(c => c.Name).ToList();
        var nonChestSlots = new HashSet<string>(Paths.InventoryLoc.KnownNonChestSlots);

        foreach (var name in slotNames)
        {
            if (string.IsNullOrEmpty(name)) continue;
            if (name == "commonChestbox") continue; // handled last, with a reserve
            if (nonChestSlots.Contains(name)) continue;
            if (name.StartsWith("emptySlot")) continue;

            yield return ChestOpening.OpenAll("/" + name);
        }

        var minReserve = _minCommonReserve?.Value ?? 10;
        yield return ChestOpening.OpenDownTo(Paths.InventoryLoc.CommonChestSlot, minReserve);

        yield return InventoryScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
