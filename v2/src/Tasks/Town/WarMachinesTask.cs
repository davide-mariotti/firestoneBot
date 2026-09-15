using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
using Engineer = Firebot.GameModel.Features.Town.Engineer.Engineer;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     War Machines (https://firestone-idle-rpg.fandom.com/wiki/War_Machines) leveling - the user
///     asked for a level-up logic here too, after Hall of Heroes. Per the wiki: leveling a war
///     machine costs Expedition Tokens plus 3 components (Screw/Cog/Metal, obtained passively from
///     jewel chests, auto-distributed across owned machines by the game itself) for +100 xp: "level
///     bonus = 1.05^(level-1) - 1", a flat multiplicative boost to all of that machine's attributes.
///     Both requirements are enforced by the level-up button's own clickable state, no thresholds
///     hardcoded here, same as every other gated action in this codebase.
///     Runs across every owned war machine (WarMachines.Machines), not just the 5 in the active
///     battle formation - unlike Hall of Heroes' gear T1, the wiki gives no indication that leveling
///     a benched machine is wasted (components are already auto-balanced across ALL owned machines by
///     the game itself, favoring newly unlocked ones), so there's no similar reason to scope down.
///     Screen has 3 other tabs not covered here - "garage" (read-only stats, no action at all),
///     "blueprintUpgrades" and "rarityUpgrades" (separate wiki mechanics: per-attribute currency
///     spend and big rarity-tier jumps respectively) - out of scope for "level up", not requested.
/// </summary>
public class WarMachinesTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Town;

    // No wiki-confirmed level for War Machines specifically (the mechanics page has no "Unlocks at"
    // infobox) - inferred from the Engineer building's own confirmed level-50 gate, since the wiki
    // says war machines "are unlocked by the Engineer". Flag for live verification if this is wrong.
    protected override int MinimumCharacterLevel => 50;

    internal override float? MaxRuntimeSeconds => 1800f;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    // Safety bound only, matching the pattern used everywhere else in this codebase for a "keep
    // going until nothing's left to do" loop.
    private const int MaxIterationsPerMachine = 200;

    public override IEnumerator Execute()
    {
        yield return TownScreen.Open;
        yield return TownScreen.OpenEngineer;
        yield return Engineer.OpenWarMachines;

        foreach (var machine in WarMachines.Machines)
        {
            yield return WarMachines.SelectMachine(machine);
            yield return WarMachines.OpenWorkshopTab;

            var iterations = 0;

            while (WarMachines.LevelUpBtn.IsClickable() && iterations < MaxIterationsPerMachine)
            {
                iterations++;
                yield return WarMachines.LevelUpBtn.Click();
            }
        }

        yield return WarMachines.Close;
        yield return Engineer.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
