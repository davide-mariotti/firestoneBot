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

    public static IEnumerator OpenTalentsTab => new GameButton(Paths.MenusLoc.CharacterLoc.TalentsTabBtn).Click();

    public static IEnumerator OpenDailyQuestsSubTab =>
        new GameButton(Paths.MenusLoc.CharacterLoc.QuestsLoc.DailyTabBtn).Click();

    public static IEnumerator OpenWeeklyQuestsSubTab =>
        new GameButton(Paths.MenusLoc.CharacterLoc.QuestsLoc.WeeklyTabBtn).Click();

    /// <summary>Shared element - read it right after selecting the tab whose countdown you want.</summary>
    public static DateTime QuestsRenewTime => new GameText(Paths.MenusLoc.CharacterLoc.QuestsLoc.RenewTxt).Time;

    public static List<GameButton> DailyQuestClaimButtons() =>
        QuestClaimButtons(Paths.MenusLoc.CharacterLoc.QuestsLoc.DailyQuestsGridRoot);

    public static List<GameButton> WeeklyQuestClaimButtons() =>
        QuestClaimButtons(Paths.MenusLoc.CharacterLoc.QuestsLoc.WeeklyQuestsGridRoot);

    /// <summary>
    ///     Every quest slot's claim button in the given grid - clicking one that isn't actually
    ///     completable yet is a safe no-op (same as every other button in this codebase), so callers
    ///     can just click all of them without checking completion state first.
    /// </summary>
    private static List<GameButton> QuestClaimButtons(string gridRootPath)
    {
        var grid = new GameElement(gridRootPath);
        var buttons = new List<GameButton>();
        foreach (var quest in grid.GetChildren())
            buttons.Add(new GameButton("claimButton", quest));
        return buttons;
    }
}
