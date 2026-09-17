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
///     This whole feature is new.
///     Fully stateless by design: each run re-opens the guide's nodes in order (via TalentPreview)
///     until it finds the first one still below its planned target rank, invests there, and continues
///     until either points run out or the plan is fully satisfied - it never persists "where it left
///     off" between runs. Per the user's explicit choice, once every planned entry is satisfied but
///     more points remain (the guide covers only the tree's first ~420 of 2037 total points), the task
///     stops and leaves the rest unspent rather than guessing - a wrong guess isn't free (a full tree
///     reset costs 100 gems per the wiki).
///     Confirmed by the user: upgradeTalentButton only stages a point, real investment happens on
///     talentsSaveButton. Saved after every single node visited (not batched across the whole plan)
///     so a later re-visit to the same talent (the plan revisits several - see Talents.Plan) always
///     reads back a real committed rank instead of needing to know whether the preview's rank display
///     reflects an unsaved pending change.
///     Works fine on an account that already diverged from the guide's exact order before the bot
///     ever ran it, without needing to know its history: every entry only ever compares the node's
///     CURRENT live rank against that entry's target, so a talent the account already pushed ahead of
///     plan reads as "nothing to invest" and is skipped (targetRank - currentRank comes out
///     negative/zero, the invest loop just doesn't run), while one that's behind gets topped up
///     toward the same target it would have reached by following the guide from empty. The one case
///     this can't route around: a guide-covered talent whose OWN direct prerequisite (a specific
///     earlier talent in its branch, not just the tree's cumulative point total - see the wiki) never
///     got a single point from the account's real history stays locked no matter how many points are
///     available - skipped for this run (see IsPreviewLocked below) rather than guessed at.
/// </summary>
public class TalentsTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Character;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(2);

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.TalentAvailable;

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
                    // Shouldn't happen on an account that only ever invested through this same
                    // plan, in order - but a divergent account's real history might never have put
                    // a point in this branch's specific predecessor (see the class doc). Skip just
                    // this one entry rather than giving up on the whole run: a later entry may
                    // still be perfectly investable.
                    yield return Talents.ClosePreview;
                    continue;
                }

                var currentRank = Talents.PreviewCurrentRank;
                var toInvest = Math.Min(targetRank - currentRank, availablePoints);
                var invested = 0;

                for (var i = 0; i < toInvest && Talents.UpgradeButton.IsClickable(); i++)
                {
                    yield return Talents.UpgradeButton.Click();
                    availablePoints--;
                    invested++;
                }

                yield return Talents.ClosePreview;

                if (invested > 0) yield return Talents.Save;

                if (availablePoints <= 0) break;
            }
        }

        yield return CharacterScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
