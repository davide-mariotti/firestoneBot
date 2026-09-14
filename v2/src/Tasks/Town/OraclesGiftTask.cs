using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Oracle;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Tasks.Town;

/// <summary>Claims the daily Oracle's Gift once the character reaches the level it unlocks at.</summary>
public class OraclesGiftTask : BotTask
{
    private const int MinimumCharacterLevel = 200;
    private static readonly TimeSpan RecheckDelayBelowLevel = TimeSpan.FromHours(1);

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.OraclesGiftBtn;

    public override IEnumerator Execute()
    {
        if (PlayerAvatar.CharacterLevel < MinimumCharacterLevel)
        {
            // Feature doesn't exist yet below the unlock level - nothing to open, just recheck later
            // as the character levels up instead of retrying every scan cycle.
            NextRunTime = DateTime.Now + RecheckDelayBelowLevel;
            yield break;
        }

        yield return Notifications.OraclesGift;
        yield return OracleStore.ClaimGift;
        NextRunTime = OracleStore.NextRunTime;
        yield return OracleStore.Close;
    }
}
