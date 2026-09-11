using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Library.FirestoneResearch;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors.Town;

public class FirestoneResearchTask : BotTask
{
    private const int NodeCount = 16;

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.FirestoneResearchBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.FirestoneResearch;

        var panel = new ResearchPanel();
        yield return panel.Claim();

        // Buy a new concurrent research slot whenever affordable - the button itself is disabled
        // (a safe no-op click) once there's nothing left to unlock or not enough meteorites.
        yield return new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.ResearchPanelLoc.UnlockSlotBtn)
            .Click();

        if (!ResearchPanel.HasEmptySlot)
        {
            Debug("[INFO] No empty slots. Scheduling next run.");
            NextRunTime = panel.NextRunTime();
            yield break;
        }

        yield return RunSelection();
        NextRunTime = panel.NextRunTime();
    }

    /// <summary>
    ///     Picks the next talent to research by levelling every node in "waves": among all currently
    ///     researchable nodes, it always picks one at the lowest current level first (so nothing gets
    ///     rushed to max while others are still at 0), breaking ties by whichever takes the least time.
    ///     This scans and compares all 16 nodes every time a slot frees up.
    /// </summary>
    private IEnumerator RunSelection()
    {
        var node = new Node();

        while (ResearchPanel.HasEmptySlot)
        {
            int? bestIndex = null;
            var bestLevel = int.MaxValue;
            var bestTime = TimeSpan.MaxValue;

            for (var index = 1; index <= NodeCount; index++)
            {
                yield return node.Select(index);

                if (Preview.IsUnlocked && !Preview.IsMaxed)
                {
                    var level = Preview.CurrentLevel;
                    var time = Preview.TimeRequired;

                    if (level < bestLevel || (level == bestLevel && time < bestTime))
                    {
                        bestLevel = level;
                        bestTime = time;
                        bestIndex = index;
                    }
                }

                yield return Preview.Close;
            }

            if (bestIndex == null) yield break;

            Debug($"[INFO] Selected talent #{bestIndex} (level {bestLevel}, {bestTime} to complete).");

            yield return node.Select(bestIndex.Value);
            if (Preview.IsUnlocked && !Preview.IsMaxed) yield return Preview.Start;
        }
    }
}
