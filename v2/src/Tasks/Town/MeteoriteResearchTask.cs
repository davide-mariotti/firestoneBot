using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Library.MeteoriteResearch;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;
using Library = Firebot.GameModel.Features.Town.Library.Library;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     The Library's OTHER research tab (Ricerca Meteoriti - talent bonuses across 5 trees of 13
///     nodes each), separate from FirestoneResearchTask's tech tree. Different theme on purpose:
///     Firestone Research runs continuously (always start the next talent the moment a slot frees
///     up), while Meteorite Research is gated by a currency (meteorite stones) spent per node - there
///     is nothing to do until enough has accumulated. Same "many at medium level beats one maxed"
///     principle as Firestone Research: always targets whichever unlocked node is currently cheapest,
///     never chases a single expensive one.
///     No v1 precedent at all - v1 never implemented this tab (docs/path.firestone.html marks its
///     notification badge "Rimossa dal bot, feature mai raggiunta"). Paths sourced from a fresh
///     UnityPy scan of the live game assets, not just the docs (which didn't capture the preview
///     popup or the per-node cost display) - flagged for live verification throughout.
/// </summary>
public class MeteoriteResearchTask : BotTask
{
    private const int TreeCount = 5;
    private const int NodeCount = 13; // research0..12 per tree

    private MelonPreferences_Entry<int> _recheckIntervalMinutes;

    // No NotificationPath, deliberately - same reasoning as FreePickaxes/Empower: this is a currency
    // threshold gate, so a badge being up (even if it reliably existed, which is itself unverified
    // here) wouldn't mean anything is actually affordable yet.
    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_recheckIntervalMinutes != null) return;

        _recheckIntervalMinutes = category.CreateEntry(
            "recheck_interval_minutes",
            60,
            "Recheck Interval (minutes)",
            "How long to wait before checking again when the cheapest available research still " +
            "isn't affordable. There is no known way to read the meteorite stone balance without " +
            "opening the Library screen (checked the live game files - no persistent currency bar " +
            "exists outside it), so this interval is what keeps the bot from reopening it constantly. " +
            "Default: 60."
        );
    }

    public override IEnumerator Execute()
    {
        // Fast path: the notification (when up) opens the Library directly. Safe no-op otherwise.
        // Unverified against v1 - see Notifications.MeteoriteResearch.
        yield return Notifications.MeteoriteResearch;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return TownScreen.Open;
        yield return TownScreen.OpenLibrary;
        yield return Library.OpenMeteoriteResearchTab;

        yield return RunCheapestResearch();

        NextRunTime = DateTime.Now + TimeSpan.FromMinutes(_recheckIntervalMinutes?.Value ?? 60);

        yield return Library.Close;
        yield return TownScreen.Close;
    }

    private IEnumerator RunCheapestResearch()
    {
        var node = new MeteoriteNode();

        int? bestIndex = null;
        int? bestTreeOffset = null;
        var bestCost = double.MaxValue;

        for (var treeOffset = 0; treeOffset < TreeCount; treeOffset++)
        {
            for (var index = 0; index < NodeCount; index++)
            {
                yield return node.Select(index);

                if (MeteoriteResearchPreview.IsUnlocked)
                {
                    var cost = MeteoriteResearchPreview.Cost;

                    // cost <= 0 covers both "failed to parse" and "no cost shown" (e.g. an already
                    // maxed node) - either way, not a real candidate.
                    if (cost > 0 && cost < bestCost)
                    {
                        bestCost = cost;
                        bestIndex = index;
                        bestTreeOffset = treeOffset;
                    }
                }

                yield return MeteoriteResearchPreview.Close;
            }

            if (treeOffset < TreeCount - 1) yield return node.NextTree;
        }

        if (bestIndex == null) yield break;

        // The scan above ends on the last tree - step back to the tree with the cheapest pick. Works
        // regardless of whether the tree carousel wraps around or clamps at the ends, since we only
        // ever move backward from a known position toward a lower one.
        for (var back = TreeCount - 1; back > bestTreeOffset; back--)
            yield return node.PreviousTree;

        Debug($"[INFO] Cheapest available meteorite research costs {bestCost:0.##} " +
              $"(tree offset {bestTreeOffset}, node {bestIndex}). Attempting - safe no-op if not yet affordable.");

        yield return node.Select(bestIndex.Value);
        yield return MeteoriteResearchPreview.Research;
        yield return MeteoriteResearchPreview.Close;
    }
}
