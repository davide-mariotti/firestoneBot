using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town;

public static class Tavern
{
    public static IEnumerator OpenScarabGame => new GameButton(Paths.MenusLoc.TavernLoc.OpenScarabGameBtn).Click();

    public static IEnumerator OpenMarket => new GameButton(Paths.MenusLoc.TavernLoc.OpenMarketBtn).Click();

    public static GameButton PlayBtn => new(Paths.MenusLoc.TavernLoc.PlayBtn);

    public static int GameTokenCount => new GameText(Paths.MenusLoc.TavernLoc.GameTokenCountTxt).GetParsedInt();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.TavernLoc.CloseBtn).Click();
}
