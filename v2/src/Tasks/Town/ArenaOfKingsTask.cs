using System;
using System.Collections;
using System.Collections.Generic;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using UnityEngine;
using Logger = Firebot.Core.Logger;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     Spends today's Arena of Kings battle tokens (max 5, recharge daily per the wiki) fighting the
///     weakest available opponent each time, per the user's requested strategy. No v1 precedent - this
///     whole feature is new. Level-gated at 80 per the wiki's Arena of Kings infobox.
///     For each token: scans the 3 current opponents' power against mine (ArenaOfKings.MyPower reads
///     "arenaPower", the arena-specific figure - NOT the separate "battlePower" display right next to
///     it, which is for regular campaign battles and uses a different formula per the wiki). If none
///     is strictly weaker, rerolls (free every 5s per the wiki) and checks again. The user confirmed
///     losing does NOT lower rank (only winning changes anything, by swapping ranks with the
///     opponent - the wiki is explicit about this), so there's no benefit to intentionally losing -
///     the progressive fallback below exists only so a token doesn't go completely unused/wasted, not
///     to manipulate rank:
///     - 0-3 min: only a strictly-weaker opponent is acceptable.
///     - 3-5 min: accept up to 5% stronger than me.
///     - 5-7 min: accept up to 10% stronger.
///     - 7-9 min: accept up to 20% stronger.
///     - past 9 min: fight whichever of the 3 is weakest regardless of margin.
///     Formation is set up once manually by the user (see AOKBattlePreview) - same assumption as
///     Warfront's Liberator quest, never touches changeFormationButton.
///     Worth flagging: in a bad-luck run this can legitimately take close to 9 minutes of searching
///     per token, up to 5 tokens/day - if max_task_runtime (BotSettings) is set low, a search in
///     progress could get cut off mid-wait. Watchdog's cleanup sweep (runs before/after every task)
///     should recover from that, but a generous max_task_runtime is worth double-checking if this
///     task is enabled.
/// </summary>
public class ArenaOfKingsTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Town;
    protected override int MinimumCharacterLevel => 80;

    private static readonly WaitForSeconds RerollWait = new(5f);
    private static readonly WaitForSeconds BattlePollWait = new(2f);
    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    // Safety bound only - never observed a real duration. The wiki says results are determined
    // server-side and then shown as a "recording", so this is expected to resolve quickly, but
    // errs generous rather than risk cutting a real animation short (same reasoning as Liberator).
    private const int MaxBattlePolls = 150; // ~5 minutes at 2s/poll

    // (cumulative deadline since starting this token's search, max acceptable opponent power as a
    // multiple of mine - null means "strictly weaker only"). Checked in order; the first entry whose
    // deadline hasn't passed yet applies. Past every deadline, the weakest of the 3 is force-picked
    // regardless of margin - see FindTarget.
    private static readonly (TimeSpan Deadline, double? MaxMultiplier)[] SearchPhases =
    {
        (TimeSpan.FromMinutes(3), null),
        (TimeSpan.FromMinutes(5), 1.05),
        (TimeSpan.FromMinutes(7), 1.10),
        (TimeSpan.FromMinutes(9), 1.20)
    };

    public override IEnumerator Execute()
    {
        // Fast path - opportunistic only (see Battle.cs), safe no-op if not up.
        yield return Notifications.ArenaTokens;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return TownScreen.Open;
        yield return TownScreen.OpenBattles;
        yield return WFMenuSelection.OpenArena;

        while (ArenaOfKings.TokensAvailable > 0)
        {
            var slotIndex = -1;
            yield return FindTarget(index => slotIndex = index);
            if (slotIndex < 0) break; // shouldn't happen (FindTarget always eventually force-picks)

            yield return ArenaOfKings.Fight(slotIndex);
            yield return AOKBattlePreview.Fight;

            var pollsLeft = MaxBattlePolls;
            while (!AOKBattleResult.IsVisible && pollsLeft > 0)
            {
                yield return BattlePollWait;
                pollsLeft--;
            }

            if (pollsLeft == 0)
                Logger.Warning("[ArenaOfKingsTask] Battle didn't resolve within the wait bound.");

            yield return AOKBattleResult.Close;
        }

        yield return WFMenuSelection.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }

    private IEnumerator FindTarget(Action<int> onFound)
    {
        var searchStart = DateTime.Now;

        while (true)
        {
            var myPower = ArenaOfKings.MyPower;
            var powers = ArenaOfKings.OpponentPowers();
            var elapsed = DateTime.Now - searchStart;

            var chosen = ChooseOpponent(myPower, powers, elapsed);
            if (chosen != null)
            {
                onFound(chosen.Value);
                yield break;
            }

            yield return ArenaOfKings.Reroll;
            yield return RerollWait;
        }
    }

    private static int? ChooseOpponent(double myPower, IReadOnlyList<double> powers, TimeSpan elapsed)
    {
        foreach (var (deadline, maxMultiplier) in SearchPhases)
        {
            if (elapsed >= deadline) continue; // this phase's window passed - try the next, more permissive one
            return WeakestWithin(powers, myPower, maxMultiplier);
        }

        // Past every phase - force-pick the least-bad option so the token doesn't go unused.
        return WeakestIndex(powers);
    }

    private static int? WeakestWithin(IReadOnlyList<double> powers, double myPower, double? maxMultiplier)
    {
        int? best = null;
        var bestPower = double.MaxValue;

        for (var i = 0; i < powers.Count; i++)
        {
            var acceptable = maxMultiplier == null ? powers[i] < myPower : powers[i] <= myPower * maxMultiplier.Value;
            if (!acceptable || powers[i] >= bestPower) continue;

            bestPower = powers[i];
            best = i;
        }

        return best;
    }

    private static int WeakestIndex(IReadOnlyList<double> powers)
    {
        var best = 0;
        for (var i = 1; i < powers.Count; i++)
            if (powers[i] < powers[best])
                best = i;
        return best;
    }
}
