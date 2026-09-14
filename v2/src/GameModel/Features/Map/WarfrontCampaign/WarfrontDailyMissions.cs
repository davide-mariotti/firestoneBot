using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map.WarfrontCampaign;

public static class WarfrontDailyMissions
{
    public static IEnumerator Open => new GameButton(Paths.WorldMapLoc.WarfrontLoc.DailyMissionsBtn).Click();

    public static IEnumerator OpenLiberationMissions =>
        new GameButton(Paths.WFDailyMissionsLoc.OpenLiberationMissionsBtn).Click();

    public static DateTime NextRunTime => new GameText(Paths.WFDailyMissionsLoc.NextRunTimeTxt).Time;

    public static IEnumerator Close => new GameButton(Paths.WFDailyMissionsLoc.CloseBtn).Click();
}

public static class WarfrontLiberationMissions
{
    public static GameElement MissionsGrid => new(Paths.WFLiberationMissionsLoc.MissionsGridRoot);

    public static IEnumerator Close => new GameButton(Paths.WFLiberationMissionsLoc.CloseBtn).Click();
}
