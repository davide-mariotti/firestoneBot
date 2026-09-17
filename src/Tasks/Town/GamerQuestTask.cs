using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
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
///     Live-confirmed, 2026-09-18 (user screenshots): "Play 1" costs 1 token, "Play 10" costs 10
///     (linear) - user-requested optimization: use the x10 multiplier for one round instead of 10
///     separate x1 rounds whenever there's enough headroom above the reserve for it, same idea as
///     Miner Quest's ArcaneCrystal quantity shortcut. Each round is Play + picking one of the
///     resulting card stacks (see Tavern.PlayRound) - Play alone doesn't complete anything.
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

        // Only attempted with enough headroom above the reserve for the full x10 cost (confirmed
        // linear: 10 tokens) - if that's wrong for some reason, the game's own affordability gate on
        // the button keeps it non-clickable and this safely falls through to the per-round loop below.
        if (Tavern.GameTokenCount - minReserve >= PlayCount)
        {
            yield return Tavern.TrySetPlayQuantityTo(PlayCount);

            if (Tavern.IsPlayQuantitySetTo(PlayCount) && Tavern.PlayBtn.IsClickable())
            {
                yield return Tavern.PlayRound();
                playsDone += PlayCount;
            }
            else
            {
                yield return Tavern.TrySetPlayQuantityTo(1); // revert so the per-round loop below is correct
            }
        }

        while (playsDone < PlayCount && Tavern.GameTokenCount > minReserve)
        {
            if (!Tavern.PlayBtn.IsClickable()) break;

            yield return Tavern.PlayRound();
            playsDone++;
        }

        yield return Tavern.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
