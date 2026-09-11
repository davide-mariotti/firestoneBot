using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;

namespace Firebot.GameModel.Features.Town.Library.FirestoneResearch;

public static class Preview
{
    public static bool IsUnlocked =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.UnlockedTxt).IsVisible();

    public static bool IsMaxed =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.MaxedTxt).IsVisible();

    public static int CurrentLevel
    {
        get
        {
            var text = new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.LevelTxt).GetParsedText();
            return StringUtils.TryParseIntFromString(text, out var level) ? level : 0;
        }
    }

    public static TimeSpan TimeRequired =>
        TimeParser.ParseFrom(
            new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.RealTimeTxt).GetParsedText());

    public static IEnumerator Start =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.ActivateBtn).Click();

    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.CloseBtn).Click();
}
