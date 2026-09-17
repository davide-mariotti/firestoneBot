using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Tasks.Town;

/// <summary>
///     Claims any completed daily AND weekly quest in the Character screen's Quests tab. A "Quests"
///     badge appears on the battle screen's notification rail when one completes (shared between
///     both - no separate weekly badge exists) - prioritized via NotificationPath (like
///     Engineer/MapMissions/etc.) so it's caught quickly instead of waiting for the next scheduled
///     check, and used as a navigation fast-path in Execute() below.
///     Renamed from DailyQuestsTask when weekly quest claiming was added - no prior precedent for
///     either, both newly automated.
/// </summary>
public class QuestsTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;

    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromHours(12);

    protected override string NotificationBadgeName => Paths.BattleLoc.NotificationsLoc.Quests;

    public override IEnumerator Execute()
    {
        yield return Notifications.Quests; // fast path if the badge is already up; safe no-op otherwise
        yield return CharacterScreen.Open;
        yield return CharacterScreen.OpenQuestsTab;

        yield return CharacterScreen.OpenDailyQuestsSubTab;
        foreach (var claimButton in CharacterScreen.DailyQuestClaimButtons())
            yield return claimButton.Click();
        var dailyRenewTime = CharacterScreen.QuestsRenewTime;

        yield return CharacterScreen.OpenWeeklyQuestsSubTab;
        foreach (var claimButton in CharacterScreen.WeeklyQuestClaimButtons())
            yield return claimButton.Click();
        var weeklyRenewTime = CharacterScreen.QuestsRenewTime;

        yield return CharacterScreen.Close;

        var earliestRenew = dailyRenewTime < weeklyRenewTime ? dailyRenewTime : weeklyRenewTime;
        NextRunTime = earliestRenew > DateTime.Now ? earliestRenew : DateTime.Now + FallbackRetryDelay;
    }
}
