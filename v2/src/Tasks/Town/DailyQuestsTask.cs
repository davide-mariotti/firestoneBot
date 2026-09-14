using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;

namespace Firebot.Tasks.Town;

/// <summary>Claims any completed daily quest in the Character screen's Quests tab.</summary>
public class DailyQuestsTask : BotTask
{
    private static readonly TimeSpan FallbackRetryDelay = TimeSpan.FromHours(12);

    public override IEnumerator Execute()
    {
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
