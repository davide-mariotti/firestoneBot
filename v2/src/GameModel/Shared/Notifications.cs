using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

/// <summary>Quick-access badges on the battle screen's notification rail. Grown as needed.</summary>
public static class Notifications
{
    public static IEnumerator OraclesGift =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.OraclesGiftBtn).Click();

    public static IEnumerator CheckIn =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.CheckInBtn).Click();

    public static IEnumerator MysteryBox =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.MysteryBoxBtn).Click();

    public static IEnumerator Quests =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.QuestsBtn).Click();

    public static IEnumerator FreePickaxes =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.FreePickaxesBtn).Click();

    public static IEnumerator Engineer =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.EngineerBtn).Click();

    public static IEnumerator Expeditions =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ExpeditionsBtn).Click();

    public static IEnumerator GuardianTraining =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.GuardianTrainingBtn).Click();

    public static IEnumerator OracleRituals =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.OracleRitualsBtn).Click();
}
