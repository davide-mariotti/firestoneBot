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

    public static IEnumerator Experiments =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ExperimentsBtn).Click();

    public static IEnumerator WarfrontCampaign =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.WarfrontCampaignBtn).Click();

    public static IEnumerator MapMissions =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.MapMissionsBtn).Click();

    public static IEnumerator FirestoneResearch =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.FirestoneResearchBtn).Click();

    // Not independently verified (see Battle.cs comment on TemplePrestigeBtn) - never used a
    // notification for this feature at all.
    public static IEnumerator TemplePrestige =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.TemplePrestigeBtn).Click();

    // Not independently verified (see Battle.cs comment on MeteoriteResearchBtn) - never implemented
    // this feature at all.
    public static IEnumerator MeteoriteResearch =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.MeteoriteResearchBtn).Click();

    // No prior precedent (Scarab Game was never automated before), but both confirmed present on the live
    // rail via UnityPy (see Battle.cs). ScarabGame opens the mini-game screen; ScarabGameShopFreeToken
    // is presumed to open ScarabGameShop directly (same "notification skips straight to the target"
    // pattern as OraclesGift bypassing Store) - unverified, flagged in the task itself.
    public static IEnumerator ScarabGame =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ScarabGameBtn).Click();

    public static IEnumerator ScarabGameShopFreeToken =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ScarabGameShopFreeTokenBtn).Click();

    // Sourced only from the static doc scan (see Battle.cs) - no prior precedent, not independently
    // verified via UnityPy.
    public static IEnumerator ArcaneCrystal =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ArcaneCrystalBtn).Click();

    // Confirmed present via UnityPy (see Battle.cs). No prior precedent.
    public static IEnumerator BeerExchange =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.BeerExchangeBtn).Click();

    // Confirmed present via UnityPy (see Battle.cs). No prior precedent.
    public static IEnumerator TalentAvailable =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.TalentAvailableBtn).Click();

    // Confirmed present via UnityPy (see Battle.cs). No prior precedent. Opportunistic fast path only,
    // not used as any task's NotificationPath - see ArenaOfKingsTask.
    public static IEnumerator ArenaTokens =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ArenaTokensBtn).Click();

    // Confirmed present via UnityPy (see Battle.cs). No prior precedent. Opportunistic fast path only -
    // the real entry point is Town -> hallOfHeroes building icon (see HallOfHeroesGearTask).
    public static IEnumerator HallOfHeroes =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.HallOfHeroesBtn).Click();
}
