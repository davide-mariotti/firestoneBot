using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Tasks.Town;

/// <summary>
///     Claims any completed daily quest in the Character screen's Quests tab. A "Quests" badge
///     appears on the battle screen's notification rail when one completes - prioritized via
///     NotificationPath (like Engineer/MapMissions/etc.) so it's caught quickly instead of waiting
///     for the next scheduled check, and used as a navigation fast-path in Execute() below.
/// </summary>
public class DailyQuestsTask : BotTask
{
    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromHours(12);

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.QuestsBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.Quests; // fast path if the badge is already up; safe no-op otherwise
        yield return CharacterScreen.Open;
        yield return CharacterScreen.OpenQuestsTab;
        yield return CharacterScreen.OpenDailyQuestsSubTab;

        foreach (var claimButton in CharacterScreen.DailyQuestClaimButtons())
            yield return claimButton.Click();

        var renewTime = CharacterScreen.QuestsRenewTime;
        yield return CharacterScreen.Close;

        NextRunTime = renewTime > DateTime.Now ? renewTime : DateTime.Now + FallbackRetryDelay;
    }
}
