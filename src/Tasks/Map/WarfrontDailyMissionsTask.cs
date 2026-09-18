using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Base;
using Firebot.GameModel.Features.Map;
using Firebot.GameModel.Features.Map.WarfrontCampaign;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.Tasks.Map;

/// <summary>
///     The Warfront Campaign tab's "Daily Missions" hub - separate from WarfrontCampaignLootTask
///     (different badge, different screen path within the same tab). Currently only wires the
///     "Liberation Missions" category (fight for a listed reward, no currency/cost involved anywhere
///     in this flow); the hub also has a "Dungeon Missions" category that's out of scope for now.
///     the popup paths existed before but were never wired to any task - this whole feature
///     is new, sourced from a fresh UnityPy scan (the docs didn't capture these popups either).
///     Also drives the "Liberator" daily quest (fight 2 Warfront battles) - confirmed by the user
///     that each fight is a real battle to wait out (formation is set up once manually on their end,
///     the bot only presses "start" on WFBattleSim), not an instant resolution.
/// </summary>
public class WarfrontDailyMissionsTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Warfront;
    protected override int MinimumCharacterLevel => 50;

    private static readonly WaitForSeconds BattlePollWait = new(1f);

    // Live-confirmed, 2026-09-18: real liberation battles resolve in well under a minute (a handful
    // of rounds) - the original 5-minute bound was picked before ever observing a real one and made
    // getting stuck (see WFBattleResult.IsDecided) far more costly than it needed to be. Per the
    // user, 40s is a comfortable margin.
    private const int MaxBattlePolls = 40; // ~40s at 1s/poll

    // Badge lives on the button itself (WorldMap/warfrontCampaignSubmenu/dailyMissionsButton), not
    // the battle-screen leftSideUINew rail - same situation as Path of Glory, so there's no separate
    // opportunistic click to try first; this click IS the fast path once we're on the right tab.
    protected override string NotificationPath => Paths.WorldMapLoc.WarfrontLoc.DailyMissionsNotification;

    public override IEnumerator Execute()
    {
        yield return WorldMap.Open;
        yield return WorldMap.OpenWarfrontCampaignTab;
        yield return WarfrontDailyMissions.Open;
        yield return WarfrontDailyMissions.OpenLiberationMissions;
        yield return WarfrontLiberationMissions.WaitUntilLoaded();

        foreach (var mission in WarfrontLiberationMissions.MissionsGrid.GetChildren())
        {
            var fightBtn = new GameButton(Paths.WFLiberationMissionsLoc.FightBtn, mission);
            if (!fightBtn.IsClickable()) continue;

            yield return fightBtn.Click(); // opens WFBattleSim (squad already set up by the user)
            if (!WFBattleSim.IsVisible) continue; // e.g. mission turned out locked/already resolved

            yield return WFBattleSim.Fight; // starts the real battle

            // DIAGNOSTIC (2026-09-18, kept active): the user wants the battle's own speed toggle
            // (screenshot showed "x4") set to max to make each fight resolve faster - real button
            // name not yet known. Dumping WFBattle's own children once to find it.
            var battleChildren = new GameElement(Paths.MenusLoc.Root + "/menus/WFBattle").GetChildren().ToList();
            Logger.Debug($"[DIAG] WFBattle children ({battleChildren.Count}): " +
                         string.Join(", ", battleChildren.Select(c => $"'{c.Name}'(visible={c.IsVisible()})")));

            var pollsLeft = MaxBattlePolls;
            while (!WFBattleResult.IsDecided && pollsLeft > 0)
            {
                yield return BattlePollWait;
                pollsLeft--;
            }

            if (pollsLeft == 0)
            {
                Logger.Warning("[WarfrontDailyMissionsTask] Liberation battle didn't resolve within the wait bound.");

                // DIAGNOSTIC (2026-09-18, kept active): the user saw a "Here are your rewards!" / "OK"
                // popup after a battle - not matched by WFBattleWonLoc/WFBattleDefeatLoc (both assumed,
                // never live-confirmed), so IsDecided never becomes true. Dumping the real active
                // popups/menus at the moment of the timeout to find its actual name/structure.
                DumpActiveScreens();
            }

            yield return WFBattleResult.Close;
        }

        yield return WarfrontLiberationMissions.Close;

        NextRunTime = WarfrontDailyMissions.NextRunTime;

        yield return WarfrontDailyMissions.Close;
        yield return WorldMap.Close;
    }

    // DIAGNOSTIC (2026-09-18, kept active) - see the call site above.
    private static void DumpActiveScreens()
    {
        var menusRoot = GameObject.Find("menusRoot");
        if (menusRoot == null)
        {
            Logger.Debug("[DIAG] menusRoot not found");
            return;
        }

        foreach (var rootName in new[] { "popups", "menus" })
        {
            var root = menusRoot.transform.Find($"menuCanvasParent/SafeArea/menuCanvas/{rootName}");
            if (root == null)
            {
                Logger.Debug($"[DIAG] '{rootName}' not found");
                continue;
            }

            var activeChildren = new List<string>();
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.gameObject.activeSelf) activeChildren.Add(child.name);
            }

            Logger.Debug($"[DIAG] '{rootName}' has {root.childCount} children, " +
                         $"{activeChildren.Count} activeSelf=true: {string.Join(", ", activeChildren)}");
        }
    }
}
