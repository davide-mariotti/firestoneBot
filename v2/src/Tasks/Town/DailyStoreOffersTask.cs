using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;

namespace Firebot.Tasks.Town;

/// <summary>
///     Claims the daily check-in reward and the free daily mystery box (Store &gt; "Pacchetti
///     Giornalieri" tab) - NOT the paid bundle slots next to it, see Store.ClaimFreeMysteryBox.
///     Always clicks each tab explicitly rather than assuming whichever one happens to already be
///     open, since Store remembers the last tab you had open.
/// </summary>
public class DailyStoreOffersTask : BotTask
{
    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromMinutes(30);

    public override IEnumerator Execute()
    {
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
