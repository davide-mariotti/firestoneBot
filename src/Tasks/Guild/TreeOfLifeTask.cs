using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Guild;
using Firebot.GameModel.Features.Town;

namespace Firebot.Tasks.Guild;

/// <summary>
///     Spends Expedition Tokens on Personal Tree of Life upgrades (Guild -&gt; Tree of Life -&gt; Personal
///     tab) - never the Guild Tree, which affects the whole guild and requires leader/officer rank per
///     the wiki, out of scope per the user. Level 10 gate per the wiki's Tree of Life infobox.
///     This whole feature is new, requested by the user, who also gave the priority
///     order: "Raining Gold", "Firestone Finder" and "Firestone Effect" as a group take priority over
///     the other 17 upgrades - see TreeOfLife.PriorityUpgrades. Mirrors FirestoneResearchTask's
///     "Raining Gold always wins" selection pattern exactly, generalized from one priority name to a
///     small set: among clickable (affordable, not capped) candidates, a priority one always beats a
///     non-priority one regardless of level, and within the same tier the lowest-level (cheapest,
///     since the wiki confirms cost scales purely with an upgrade's own current level) wins.
///     Re-scans and re-picks after every single purchase, same reasoning as Research: spreads
///     investment across many upgrades instead of rushing one to a high level while the rest sit at 0.
///     Live-confirmed, 2026-09-18: clicking a node only opens a preview popup ("Magic spells / Level
///     0/5 / Buy upgrade 600") - it doesn't buy directly like the NodeLevelTxt comment originally
///     assumed (see TreeOfLife.ConfirmPurchase). Its buyUpgradeButton's own IsClickable() also doesn't
///     reliably predict real affordability - running out of Expedition Tokens pops a "CurrencyMissing"
///     warning instead of silently failing (see TreeOfLife.HasInsufficientFundsMessage), so this stops
///     as soon as that appears instead of wasting the rest of the run hitting it on every remaining node.
/// </summary>
public class TreeOfLifeTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Guild;
    protected override int MinimumCharacterLevel => 10;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    // Safety bound only - one node could in principle be bought many times in a row while cheapest,
    // so this isn't "one per upgrade", just a generous ceiling matching the pattern used everywhere
    // else in this codebase for a "keep going until nothing's left to do" loop.
    private const int MaxIterations = 200;

    public override IEnumerator Execute()
    {
        yield return TownGuild.Open;
        yield return TownGuild.OpenTreeOfLife;
        yield return TreeOfLife.OpenPersonalTab;

        for (var i = 0; i < MaxIterations; i++)
        {
            var best = FindBestUpgrade();
            if (best == null) break;

            yield return TreeOfLife.PersonalNode(best.Value).Click();
            yield return TreeOfLife.ConfirmPurchase();

            if (TreeOfLife.HasInsufficientFundsMessage)
            {
                yield return TreeOfLife.CloseInsufficientFundsMessage;
                break;
            }
        }

        yield return TreeOfLife.Close;
        yield return TownGuild.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }

    private static int? FindBestUpgrade()
    {
        int? bestPriority = null;
        var bestPriorityLevel = int.MaxValue;
        int? bestOther = null;
        var bestOtherLevel = int.MaxValue;

        for (var i = 0; i < TreeOfLife.PersonalUpgradeCount; i++)
        {
            if (!TreeOfLife.PersonalNode(i).IsClickable()) continue;

            var level = TreeOfLife.PersonalNodeLevel(i);

            if (TreeOfLife.IsPriority(i))
            {
                if (level >= bestPriorityLevel) continue;
                bestPriorityLevel = level;
                bestPriority = i;
            }
            else
            {
                if (level >= bestOtherLevel) continue;
                bestOtherLevel = level;
                bestOther = i;
            }
        }

        // A priority candidate always wins if one is available at all, regardless of the cheapest
        // non-priority level.
        return bestPriority ?? bestOther;
    }
}
