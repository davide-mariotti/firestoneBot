using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

/// <summary>Quick-access badges on the battle screen's notification rail. Grown as needed.</summary>
public static class Notifications
{
    public static IEnumerator OraclesGift =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.OraclesGiftBtn).Click();
}
