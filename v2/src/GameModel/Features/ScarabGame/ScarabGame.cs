using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.ScarabGame;

public static class ScarabGame
{
    public static IEnumerator OpenShop => new GameButton(Paths.ScarabGameLoc.OpenShopBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.ScarabGameLoc.CloseBtn).Click();
}

public static class ScarabGameShop
{
    public static IEnumerator OpenSaleTab => new GameButton(Paths.ScarabGameShopLoc.SaleTabBtn).Click();

    public static IEnumerator ClaimFreeToken => new GameButton(Paths.ScarabGameShopLoc.FreeTokenLoc.ClaimBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.ScarabGameShopLoc.CloseBtn).Click();
}
