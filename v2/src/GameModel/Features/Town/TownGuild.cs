using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town;

public static class TownGuild
{
    public static IEnumerator Open => new GameButton(Paths.BattleLoc.RightSideUILoc.GuildBtn).Click();

    public static IEnumerator OpenGuildShop => new GameButton(Paths.MenusLoc.TownGuildLoc.GuildShopBtn).Click();

    public static IEnumerator OpenExpeditions => new GameButton(Paths.MenusLoc.TownGuildLoc.ExpeditionsBtn).Click();

    public static IEnumerator OpenArcaneCrystal =>
        new GameButton(Paths.MenusLoc.TownGuildLoc.ArcaneCrystalBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.TownGuildLoc.CloseBtn).Click();
}
