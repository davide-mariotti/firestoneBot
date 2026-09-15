using System.Collections;
using Firebot.Core.Tasks;
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
///     v1 had the popup paths defined but never wired any task to them at all - this whole feature
///     is new, sourced from a fresh UnityPy scan (the docs didn't capture these popups either).
///     Also drives the "Liberator" daily quest (fight 2 Warfront battles) - confirmed by the user
///     that each fight is a real battle to wait out (formation is set up once manually on their end,
///     the bot only presses "start" on WFBattleSim), not an instant resolution.
/// </summary>
public class WarfrontDailyMissionsTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Warfront;
    protected override int MinimumCharacterLevel => 50;

    private static readonly WaitForSeconds BattlePollWait = new(2f);

    // Safety bound only - battles are expected to resolve in well under this. Never observed a real
    // duration, so this errs generous rather than risk cutting a real battle short.
    private const int MaxBattlePolls = 150; // ~5 minutes at 2s/poll

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

        foreach (var mission in WarfrontLiberationMissions.MissionsGrid.GetChildren())
        {
            var fightBtn = new GameButton(Paths.WFLiberationMissionsLoc.FightBtn, mission);
            if (!fightBtn.IsClickable()) continue;

            yield return fightBtn.Click(); // opens WFBattleSim (squad already set up by the user)
            if (!WFBattleSim.IsVisible) continue; // e.g. mission turned out locked/already resolved

            yield return WFBattleSim.Fight; // starts the real battle

            var pollsLeft = MaxBattlePolls;
            while (!WFBattleResult.IsDecided && pollsLeft > 0)
            {
                yield return BattlePollWait;
                pollsLeft--;
            }

            if (pollsLeft == 0)
                Logger.Warning("[WarfrontDailyMissionsTask] Liberation battle didn't resolve within the wait bound.");

            yield return WFBattleResult.Close;
        }

        yield return WarfrontLiberationMissions.Close;

        NextRunTime = WarfrontDailyMissions.NextRunTime;

        yield return WarfrontDailyMissions.Close;
        yield return WorldMap.Close;
    }
}
