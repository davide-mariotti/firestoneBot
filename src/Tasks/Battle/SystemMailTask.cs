using System;
using System.Collections;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;

namespace Firebot.Tasks.Battle;

/// <summary>
///     Claims every claimable reward in the in-game mailbox (Arcane Crystal rewards, character-level
///     milestone rewards, Arena of Kings rank rewards, unclaimed Battle Pass rewards - per the wiki).
///     This whole feature is new, requested by the user.
///     Whether claiming removes a mail from the list or leaves it there (marked read) isn't confirmed
///     live, so the scan below is written to be correct either way: it re-reads the list's live child
///     count every iteration and only advances past index 0 once that position has nothing left to
///     claim - if claiming shrinks the list, the next mail naturally slides into position 0 and gets
///     picked up on the very next iteration; if it doesn't shrink, the now-already-claimed mail is
///     revisited once (its claim button is no longer clickable - safe no-op) before moving on. Never
///     touches deleteButton (found right next to the claim button) - the user only asked for claiming,
///     and deleting is a destructive, unrequested action; leaving claimed mail in the list is harmless.
/// </summary>
public class SystemMailTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Town;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    // Safety bound only, generously above any realistic mail count - guards against an unforeseen
    // case where the claim button stays clickable after being clicked (every other loop-with-a-
    // safety-bound in this codebase does the same, e.g. WarfrontDailyMissionsTask's battle-wait poll).
    private const int MaxIterations = 200;

    public override IEnumerator Execute()
    {
        yield return SystemMail.Open;

        var index = 0;
        var iterations = 0;

        while (index < SystemMail.MailList.GetChildren().Count() && iterations < MaxIterations)
        {
            iterations++;
            var mail = SystemMail.MailList.GetChild(index);
            if (mail == null) break;

            yield return new GameButton(parent: mail).Click();

            var claimBtn = SystemMail.ClaimBtn;
            if (claimBtn.IsClickable())
                yield return claimBtn.Click();
            else
                index++; // nothing to claim here (already claimed, or no reward) - move to the next one
        }

        yield return SystemMail.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
