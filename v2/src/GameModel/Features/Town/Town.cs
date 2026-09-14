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

    public static IEnumerator OpenOracle => new GameButton(Paths.MenusLoc.TownIrongardLoc.OracleBtn).Click();

    public static IEnumerator OpenAlchemist => new GameButton(Paths.MenusLoc.TownIrongardLoc.AlchemistBtn).Click();

    public static IEnumerator OpenLibrary => new GameButton(Paths.MenusLoc.TownIrongardLoc.LibraryBtn).Click();

    public static IEnumerator OpenTempleOfEternals =>
        new GameButton(Paths.MenusLoc.TownIrongardLoc.TempleOfEternalsBtn).Click();

    public static IEnumerator OpenTavern => new GameButton(Paths.MenusLoc.TownIrongardLoc.TavernBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.TownIrongardLoc.CloseBtn).Click();
}
