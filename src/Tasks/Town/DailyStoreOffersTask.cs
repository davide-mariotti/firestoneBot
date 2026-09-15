using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;

namespace Firebot.Tasks.Town;

/// <summary>
///     Claims the daily check-in reward and the free daily mystery box (Store &gt; "Pacchetti
///     Giornalieri" tab) - NOT the paid bundle slots next to it, see Store.ClaimFreeMysteryBox.
///     No single NotificationPath: this covers two separately-badged claims (CheckIn, MysteryBox),
///     same as before's DailyRewardsTask - each is opportunistically fast-pathed via its own notification
///     click below, but neither alone should drive this task's scheduling priority.
///     Always clicks each tab explicitly afterwards rather than assuming whichever one the
///     notification (if any) happened to open is the only one that needs doing - Store remembers
///     the last tab you had open, and both claims need checking every run regardless of entry point.
/// </summary>
public class DailyStoreOffersTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Town;

    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromMinutes(30);

    public override IEnumerator Execute()
    {
        // Fast path: jump straight in if a badge is already showing. Safe no-ops otherwise.
        yield return Notifications.CheckIn;
        yield return Notifications.MysteryBox;

        yield return Store.Open;

        yield return Store.OpenDailyRewardsTab;
        yield return Store.ClaimCheckIn;
        var checkInNext = Store.CheckInNextRunTime;

        yield return Store.OpenValueBundleDailyTab;
        yield return Store.ClaimFreeMysteryBox;
        var mysteryBoxNext = Store.ValueBundleDailyRenewTime;

        yield return Store.Close;

        NextRunTime = EarliestValid(checkInNext, mysteryBoxNext) ?? DateTime.Now + FallbackRetryDelay;
    }

    /// <summary>Picks the soonest future timestamp, ignoring any that failed to parse (DateTime.MinValue).</summary>
    private static DateTime? EarliestValid(params DateTime[] times)
    {
        DateTime? earliest = null;
        foreach (var t in times)
        {
            if (t <= DateTime.Now) continue;
            if (earliest == null || t < earliest) earliest = t;
        }

        return earliest;
    }
}
