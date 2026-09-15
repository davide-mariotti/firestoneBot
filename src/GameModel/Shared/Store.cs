using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class Store
{
    public static IEnumerator Open => new GameButton(Paths.BattleLoc.RightSideUILoc.StoreBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.StoreLoc.CloseBtn).Click();

    public static IEnumerator OpenDailyRewardsTab =>
        new GameButton(Paths.MenusLoc.StoreLoc.TabsLoc.DailyRewardsBtn).Click();

    public static IEnumerator ClaimCheckIn =>
        new GameButton(Paths.MenusLoc.StoreLoc.DailyRewardsLoc.CheckInBtn).Click();

    public static DateTime CheckInNextRunTime =>
        new GameText(Paths.MenusLoc.StoreLoc.DailyRewardsLoc.NextRunTimeTxt).Time;

    public static IEnumerator OpenValueBundleDailyTab =>
        new GameButton(Paths.MenusLoc.StoreLoc.TabsLoc.ValueBundleDailyBtn).Click();

    /// <summary>
    ///     Claims ONLY the free mystery box slot in this tab. The numbered valueBundle (0)/(1)/(2)
    ///     slots next to it are real-money/premium-currency purchases - never wire those up here.
    /// </summary>
    public static IEnumerator ClaimFreeMysteryBox =>
        new GameButton(Paths.MenusLoc.StoreLoc.ValueBundleDailyLoc.FreeMysteryBoxBtn).Click();

    public static DateTime ValueBundleDailyRenewTime =>
        new GameText(Paths.MenusLoc.StoreLoc.ValueBundleDailyLoc.RenewTxt).Time;
}
