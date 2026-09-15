using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Character;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Tasks.Character;

/// <summary>
///     Spends talent points (Character screen, Talents tab) following the priority order in
///     GameModel/Features/Character/Talents.cs, transcribed from the user's docs/talents-guide.html.
///     No v1 precedent - this whole feature is new.
///     Fully stateless by design: each run re-opens the guide's nodes in order (via TalentPreview)
///     until it finds the first one still below its planned target rank, invests there, and continues
///     until either points run out or the plan is fully satisfied - it never persists "where it left
///     off" between runs. Per the user's explicit choice, once every planned entry is satisfied but
///     more points remain (the guide covers only the tree's first ~448 of 2037 total points), the task
///     stops and leaves the rest unspent rather than guessing - a wrong guess isn't free (a full tree
///     reset costs 100 gems per the wiki).
/// </summary>
public class TalentsTask : BotTask
{
    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(2);

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.TalentAvailableBtn;

    public override IEnumerator Execute()
    {
        // Fast path - safe no-op if not up.
        yield return Notifications.TalentAvailable;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return CharacterScreen.Open;
        yield return CharacterScreen.OpenTalentsTab;

        var availablePoints = Talents.AvailablePoints;
        if (availablePoints > 0)
        {
            foreach (var (catalogIndex, targetRank) in Talents.Plan)
            {
                yield return Talents.OpenNode(catalogIndex);

                if (Talents.IsPreviewLocked)
                {
                    // Shouldn't normally happen - the guide's own order should already respect
                    // real prerequisites (validated by hand against the wiki's per-tier costs
                    // while building the catalog). Stop rather than skip past it out of order.
                    yield return Talents.ClosePreview;
                    break;
                }

                var currentRank = Talents.PreviewCurrentRank;
                var toInvest = Math.Min(targetRank - currentRank, availablePoints);

                for (var i = 0; i < toInvest && Talents.UpgradeButton.IsClickable(); i++)
                {
                    yield return Talents.UpgradeButton.Click();
                    availablePoints--;
                }

                yield return Talents.ClosePreview;

                if (availablePoints <= 0) break;
            }

            yield return Talents.Save;
        }

        yield return CharacterScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
