using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Map;
using Firebot.GameModel.Features.Map.WarfrontCampaign;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.Tasks.Map;

/// <summary>
///     The Warfront Campaign tab's "Daily Missions" hub - separate from WarfrontCampaignLootTask
///     (different badge, different screen path within the same tab). Currently only wires the
///     "Liberation Missions" category (fight for a listed reward, no currency/cost involved anywhere
///     in this flow); the hub also has a "Dungeon Missions" category that's out of scope for now.
///     v1 had the popup paths defined but never wired any task to them at all - this whole feature
///     is new, sourced from a fresh UnityPy scan (the docs didn't capture these popups either).
/// </summary>
public class WarfrontDailyMissionsTask : BotTask
{
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
            if (fightBtn.IsClickable()) yield return fightBtn.Click();
        }

        yield return WarfrontLiberationMissions.Close;

        NextRunTime = WarfrontDailyMissions.NextRunTime;

        yield return WarfrontDailyMissions.Close;
        yield return WorldMap.Close;
    }
}
