using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     Progresses the daily quest "Gamer" (play 10 times in the Tavern) - level 15 per the wiki quest
///     table. Never automated before.
///     Card draws cost game tokens (NOT beer, despite the name suggesting otherwise - confirmed via
///     the wiki: "Card draws require Game Tokens"). Plays up to 10 times but always leaves at least
///     min_token_reserve tokens unspent, so tomorrow's 10 plays aren't blocked either. See
///     BeerExchangeTask for how tokens get topped up from passively-accumulated beer.
/// </summary>
public class GamerQuestTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;
    protected override int MinimumCharacterLevel => 15;

    private const int PlayCount = 10;
    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    private MelonPreferences_Entry<int> _minTokenReserve;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_minTokenReserve != null) return;

        _minTokenReserve = category.CreateEntry(
            "min_token_reserve",
            5,
            "Minimum Game Token Reserve",
            "Never spend game tokens on Tavern card draws below this count, so tomorrow's 10 draws " +
            "for this quest aren't blocked either. Default: 5."
        );
    }

    public override IEnumerator Execute()
    {
        yield return TownScreen.Open;
        yield return TownScreen.OpenTavern;

        var minReserve = _minTokenReserve?.Value ?? 5;
        var playsDone = 0;

        while (playsDone < PlayCount && Tavern.GameTokenCount > minReserve)
        {
            var playBtn = Tavern.PlayBtn;
            if (!playBtn.IsClickable()) break;

            yield return playBtn.Click();
            playsDone++;
        }

        yield return Tavern.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
