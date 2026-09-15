using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Library.FirestoneResearch;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using Library = Firebot.GameModel.Features.Town.Library.Library;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

public class FirestoneResearchTask : BotTask
{
    private const int NodeCount = 16;
    private const int TreeCount = 3;

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.FirestoneResearchBtn;

    public override IEnumerator Execute()
    {
        // Fast path: the notification (when up) opens the Library directly. Safe no-op otherwise.
        yield return Notifications.FirestoneResearch;

        // Guaranteed path regardless of the notification - same reasoning as the previous tasks:
        // don't rely on the screen already being open.
        yield return TownScreen.Open;
        yield return TownScreen.OpenLibrary;
        yield return Library.OpenFirestoneResearchTab;

        var panel = new ResearchPanel();
        yield return panel.Claim();

        // Buy a new concurrent research slot whenever affordable - the button itself is disabled
        // (a safe no-op click) once there's nothing left to unlock or not enough meteorites.
        yield return new GameButton(Paths.MenusLoc.LibraryLoc.ResearchPanelLoc.UnlockSlotBtn).Click();

        if (!ResearchPanel.HasEmptySlot)
        {
            Debug("[INFO] No empty slots. Scheduling next run.");
            NextRunTime = panel.NextRunTime();
            yield return Library.Close;
            yield return TownScreen.Close;
            yield break;
        }

        yield return RunSelection();
        NextRunTime = panel.NextRunTime();

        yield return Library.Close;
        yield return TownScreen.Close;
    }

    /// <summary>
    ///     Picks the next talent to research purely by shortest time-to-complete, across all 3 trees.
    ///     Rushing one talent to a high level over many days while the rest of the tree sits at 0 is a
    ///     worse use of time than spreading the same total time across many cheaper talents - so the
    ///     fastest currently-researchable option wins, EXCEPT "Raining Gold" always wins over
    ///     everything else when it's an unlocked option (an external tips guide the user found rates
    ///     it top priority - "Raining Gold ★★★★★" - and the user asked to prioritize it here; among
    ///     multiple unlocked Raining Gold instances across the 3 trees, still picks the fastest one).
    ///     Re-scans and compares every node in every tree each time a slot frees up.
    /// </summary>
    private IEnumerator RunSelection()
    {
        var node = new Node();

        while (ResearchPanel.HasEmptySlot)
        {
            int? bestIndex = null;
            int? bestTreeOffset = null;
            var bestTime = TimeSpan.MaxValue;
            var bestIsGold = false;

            for (var treeOffset = 0; treeOffset < TreeCount; treeOffset++)
            {
                for (var index = 1; index <= NodeCount; index++)
                {
                    yield return node.Select(index);

                    if (Preview.IsUnlocked && !Preview.IsMaxed)
                    {
                        var isGold = Preview.Name.Contains("Raining Gold", StringComparison.OrdinalIgnoreCase);
                        var time = Preview.TimeRequired;

                        // A gold candidate always beats a non-gold one, regardless of time; among two
                        // candidates of the same gold-ness, the faster one wins.
                        var better = isGold != bestIsGold ? isGold : time < bestTime;

                        if (better)
                        {
                            bestTime = time;
                            bestIndex = index;
                            bestTreeOffset = treeOffset;
                            bestIsGold = isGold;
                        }
                    }

                    yield return Preview.Close;
                }

                if (treeOffset < TreeCount - 1) yield return node.NextTree;
            }

            if (bestIndex == null) yield break;

            // The scan above ends on the last tree - step back to the tree with the cheapest pick.
            // Works regardless of whether the tree carousel wraps around or clamps at the ends, since
            // we only ever move backward from a known position toward a lower one.
            for (var back = TreeCount - 1; back > bestTreeOffset; back--)
                yield return node.PreviousTree;

            Debug($"[INFO] Selected talent #{bestIndex} on tree offset {bestTreeOffset} ({bestTime} to complete).");

            yield return node.Select(bestIndex.Value);
            if (Preview.IsUnlocked && !Preview.IsMaxed) yield return Preview.Start;
        }
    }
}
