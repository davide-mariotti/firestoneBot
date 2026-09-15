using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Map;
using Firebot.GameModel.Features.Map.WarfrontCampaign;
using Firebot.GameModel.Shared;

namespace Firebot.Tasks.Map;

// No NotificationPath, deliberately, same as before - this task is cooldown-driven only. Unlike the
// other tasks the badge here isn't a reliable "something to claim" signal on its own (Warfront also
// covers the daily/liberation missions sub-tabs, which this task doesn't handle), so it's still worth
// clicking opportunistically below but not worth promoting to notification-priority scheduling.
public class WarfrontCampaignLootTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Warfront;
    protected override int MinimumCharacterLevel => 50;

    public override IEnumerator Execute()
    {
        yield return Notifications.WarfrontCampaign;

        // Guaranteed path regardless of the notification - same reasoning as the previous tasks:
        // don't rely on the screen/tab already being open/selected.
        yield return WorldMap.Open;
        yield return WorldMap.OpenWarfrontCampaignTab;

        yield return WarfrontLoot.Claim;
        NextRunTime = WarfrontLoot.NextRunTime;

        yield return WorldMap.Close;
    }
}
