using System;
using System.Collections;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Base;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using Logger = Firebot.Core.Logger;
using ScarabGameScreen = Firebot.GameModel.Features.ScarabGame.ScarabGame;
using ScarabGameShopScreen = Firebot.GameModel.Features.ScarabGame.ScarabGameShop;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.ScarabGame;

/// <summary>
///     Claims the free daily gift in ScarabGameShop's "Saldi" tab (named "purchaseButton" but
///     confirmed genuinely free via a sibling "freeText" label - same pattern as Task 3's mystery
///     box). Scarab Game was never automated before.
///     Reached via Town -&gt; the "tavern" building's TavernSelection choice popup -&gt; its "scarabGame"
///     card (live-confirmed 2026-09-18, see Town.OpenScarabGame) - corrected after initially assuming
///     there was no permanent manual entry point at all, only the battle-screen notification badges
///     (still used as a fast path below).
///     Level-gated like OraclesGiftTask: the wiki's Scarab's Game infobox lists "unlock = Level 60".
/// </summary>
public class ScarabGameFreeTokenTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.ScarabGame;
    protected override int MinimumCharacterLevel => 60;

    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromHours(6);

    protected override string NotificationBadgeName => Paths.BattleLoc.NotificationsLoc.ScarabGameShopFreeToken;

    public override IEnumerator Execute()
    {
        // Fast paths: either badge (when up) may already open the shop directly. Safe no-ops otherwise.
        yield return Notifications.ScarabGameShopFreeToken;
        yield return Notifications.ScarabGame;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return TownScreen.Open;
        yield return TownScreen.OpenScarabGame;
        yield return ScarabGameScreen.OpenShop;

        // DIAGNOSTIC (2026-09-18, kept active): root/"bg" wrapper confirmed correct - now dumping the
        // real tab names under "submenuButtons" and each tab's item structure under "submenus" to
        // find the "Monthly pass" tab's real name and its "Pharaoh's token" free-claim button (see
        // class doc comment - not yet handled by this task).
        var tabs = new GameElement(Paths.ScarabGameShopLoc.SubmenuButtonsRoot).GetChildren().ToList();
        Logger.Debug($"[DIAG] ScarabGameShop tabs ({tabs.Count}): " +
                     string.Join(", ", tabs.Select(t => $"'{t.Name}'(visible={t.IsVisible()})")));

        var submenus = new GameElement(Paths.ScarabGameShopLoc.SubmenusRoot).GetChildren().ToList();
        Logger.Debug($"[DIAG] ScarabGameShop submenus ({submenus.Count}): " +
                     string.Join(", ", submenus.Select(s => $"'{s.Name}'(visible={s.IsVisible()})")));
        foreach (var submenu in submenus)
        {
            var items = submenu.GetChildren().ToList();
            Logger.Debug($"[DIAG] '{submenu.Name}' items: " +
                         string.Join(", ", items.Select(i => $"'{i.Name}'")));

            foreach (var item in items)
            {
                var itemChildren = item.GetChildren().ToList();
                if (itemChildren.Count == 0) continue;
                Logger.Debug($"[DIAG]   '{item.Name}' children: " +
                             string.Join(", ", itemChildren.Select(c => $"'{c.Name}'")));
            }
        }

        yield return ScarabGameShopScreen.OpenSaleTab;
        yield return ScarabGameShopScreen.ClaimFreeToken;

        yield return ScarabGameShopScreen.Close;
        yield return ScarabGameScreen.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + FallbackRetryDelay;
    }
}
