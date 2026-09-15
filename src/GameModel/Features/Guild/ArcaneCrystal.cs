using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Guild;

public static class ArcaneCrystal
{
    public static IEnumerator Hit => new GameButton(Paths.ArcaneCrystalLoc.HitBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.ArcaneCrystalLoc.CloseBtn).Click();
}
