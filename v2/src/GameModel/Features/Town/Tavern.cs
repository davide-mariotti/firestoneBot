using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town;

public static class Tavern
{
    public static IEnumerator OpenScarabGame => new GameButton(Paths.MenusLoc.TavernLoc.OpenScarabGameBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.TavernLoc.CloseBtn).Click();
}
