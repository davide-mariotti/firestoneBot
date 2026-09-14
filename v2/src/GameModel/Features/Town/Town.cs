using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town;

public static class Town
{
    public static IEnumerator Open => new GameButton(Paths.BattleLoc.RightSideUILoc.TownBtn).Click();

    public static IEnumerator OpenEngineer => new GameButton(Paths.MenusLoc.TownIrongardLoc.EngineerBtn).Click();

    public static IEnumerator OpenMagicQuarters =>
        new GameButton(Paths.MenusLoc.TownIrongardLoc.MagicQuartersBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.TownIrongardLoc.CloseBtn).Click();
}
