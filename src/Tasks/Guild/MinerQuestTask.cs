using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Guild;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Tasks.Guild;

/// <summary>
///     Progresses the daily quest "Miner" (hit the Arcane Crystal 5 times) - level 50 per the wiki
///     quest table. Always hits exactly 5 times regardless of pickaxe cost per hit: FreePickaxesTask
///     (Task 5) already trickles pickaxes in continuously (per the wiki, a free claim every 96
///     minutes = 15/day), so there's no real risk of running out just from this quest's 5 hits - per
///     the user, confirmed acceptable to just always spend them.
///     Never automated before.
/// </summary>
public class MinerQuestTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;
    protected override int MinimumCharacterLevel => 50;

    private const int HitCount = 5;
    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    public override IEnumerator Execute()
    {
        // Fast path - not independently verified/UnityPy (see Battle.cs), safe no-op if not up.
        yield return Notifications.ArcaneCrystal;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return TownGuild.Open;
        yield return TownGuild.OpenArcaneCrystal;

        for (var i = 0; i < HitCount; i++)
            yield return ArcaneCrystal.Hit;

        yield return ArcaneCrystal.Close;
        yield return TownGuild.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
