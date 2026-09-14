using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using ScarabGameScreen = Firebot.GameModel.Features.ScarabGame.ScarabGame;
using ScarabGameShopScreen = Firebot.GameModel.Features.ScarabGame.ScarabGameShop;

namespace Firebot.Tasks.ScarabGame;

/// <summary>
///     Claims the free daily gift in ScarabGameShop's "Saldi" tab (named "purchaseButton" but
///     confirmed genuinely free via a sibling "freeText" label - same pattern as Task 3's mystery
///     box). No v1 precedent at all - Scarab Game never existed in v1.
///     Unlike every other feature in this codebase, no Town/Guild building icon leads here - the
///     only known entry points are two battle-screen notification badges (ScarabGame, which opens
///     the mini-game screen, and ScarabGameShopFreeToken, presumed to open the shop directly like
///     OraclesGift bypasses Store). Both confirmed present on the live rail via a fresh UnityPy scan,
///     but whether either icon is a permanent HUD element or only appears when something's claimable
///     is unverified - flag for live testing.
/// </summary>
public class ScarabGameFreeTokenTask : BotTask
{
    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromHours(6);

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.ScarabGameShopFreeTokenBtn;

    public override IEnumerator Execute()
    {
        // Fast paths: either badge (when up) may already open the shop directly. Safe no-ops otherwise.
        yield return Notifications.ScarabGameShopFreeToken;
        yield return Notifications.ScarabGame;

        // Best-effort manual path - see class doc: this is the only known entry point, and it may
        // itself depend on the ScarabGame badge being lit rather than being a permanent icon.
        yield return ScarabGameScreen.OpenShop;
        yield return ScarabGameShopScreen.OpenSaleTab;
        yield return ScarabGameShopScreen.ClaimFreeToken;

        yield return ScarabGameShopScreen.Close;
        yield return ScarabGameScreen.Close;

        NextRunTime = DateTime.Now + FallbackRetryDelay;
    }
}
