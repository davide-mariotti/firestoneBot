using System;
using System.Collections;
using System.Collections.Generic;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class CharacterScreen
{
    public static IEnumerator Open => new GameButton(Paths.BattleLoc.PlayerAvatarLoc.OpenBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.CharacterLoc.CloseBtn).Click();

    public static IEnumerator OpenQuestsTab => new GameButton(Paths.MenusLoc.CharacterLoc.QuestsTabBtn).Click();

    public static IEnumerator OpenDailyQuestsSubTab =>
        new GameButton(Paths.MenusLoc.CharacterLoc.QuestsLoc.DailyTabBtn).Click();

    public static DateTime QuestsRenewTime => new GameText(Paths.MenusLoc.CharacterLoc.QuestsLoc.RenewTxt).Time;

    /// <summary>
    ///     Every quest slot's claim button - clicking one that isn't actually completable yet is a
    ///     safe no-op (same as every other button in this codebase), so callers can just click all
    ///     of them without checking completion state first.
    /// </summary>
    public static List<GameButton> DailyQuestClaimButtons()
    {
        var grid = new GameElement(Paths.MenusLoc.CharacterLoc.QuestsLoc.DailyQuestsGridRoot);
        var buttons = new List<GameButton>();
        foreach (var quest in grid.GetChildren())
            buttons.Add(new GameButton("claimButton", quest));
        return buttons;
    }
}
