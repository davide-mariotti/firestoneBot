using System.Collections;
using System.Collections.Generic;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town;

/// <summary>Hub popup opened by Town.OpenBattles - only its "arena" option is wired.</summary>
public static class WFMenuSelection
{
    public static IEnumerator OpenArena => new GameButton(Paths.WFMenuSelectionLoc.ArenaBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.WFMenuSelectionLoc.CloseBtn).Click();
}

public static class ArenaOfKings
{
    // Always exactly 3 per the wiki: "choose to fight one of 3 player opponents".
    private const int OpponentCount = 3;

    public static int TokensAvailable => new GameText(Paths.ArenaOfKingsLoc.BattleTokensTxt).GetParsedInt();

    public static double MyPower =>
        new GameText(Paths.ArenaOfKingsLoc.MyArenaPowerTxt).GetParsedDoubleAbbreviated();

    public static IEnumerator Reroll => new GameButton(Paths.ArenaOfKingsLoc.RerollBtn).Click();

    private static GameElement OpponentGrid => new(Paths.ArenaOfKingsLoc.OpponentGridRoot);

    /// <summary>Current power of each of the 3 opponent slots, in slot order.</summary>
    public static IReadOnlyList<double> OpponentPowers()
    {
        var powers = new List<double>(OpponentCount);
        for (var i = 0; i < OpponentCount; i++)
        {
            var opponent = OpponentGrid.GetChild(i);
            powers.Add(new GameText(Paths.ArenaOfKingsLoc.OpponentPowerTxt, opponent).GetParsedDoubleAbbreviated());
        }

        return powers;
    }

    public static IEnumerator Fight(int slotIndex)
    {
        var opponent = OpponentGrid.GetChild(slotIndex);
        yield return new GameButton(Paths.ArenaOfKingsLoc.OpponentFightBtn, opponent).Click();
    }

    public static IEnumerator Close => new GameButton(Paths.ArenaOfKingsLoc.CloseBtn).Click();
}

/// <summary>Squad/formation preview opened by ArenaOfKings.Fight - the user sets the formation once
/// manually, so this only ever needs to press the "start" button (same as Warfront's WFBattleSim).</summary>
public static class AOKBattlePreview
{
    public static IEnumerator Fight => new GameButton(Paths.AOKBattlePreviewLoc.FightBtn).Click();
}

/// <summary>The single won/lost popup an arena battle resolves into (unlike Warfront's separate
/// popups, this is one screen with both sections and one shared close button).</summary>
public static class AOKBattleResult
{
    public static bool IsVisible => new GameElement(Paths.AOKBattleResultLoc.CloseBtn).IsVisible();

    public static IEnumerator Close => new GameButton(Paths.AOKBattleResultLoc.CloseBtn).Click();
}
