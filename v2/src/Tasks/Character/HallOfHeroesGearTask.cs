using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;
using HallOfHeroesModel = Firebot.GameModel.Features.Character.HallOfHeroes;

namespace Firebot.Tasks.Character;

/// <summary>
///     Hall of Heroes (https://firestone-idle-rpg.fandom.com/wiki/Hall_of_Heroes) gear management -
///     first of a planned 2-3 task set the user asked to design from scratch
///     ("come proponi di muoverci con una logica?"), grounded in the Gear wiki
///     (https://firestone-idle-rpg.fandom.com/wiki/Gear) plus the external tips guide already used
///     for Firestone/Meteorite Research.
///     Covers, for every hero in the roster:
///     - Tier unlock: T2 (Wrist/Shoulder/Belt) and T3 (Ring/Relic), each gated on both hero power and
///       Meteorites per the wiki - both enforced by the game itself via the unlock button's own
///       clickable state, same as every other gated action in this codebase (PharaohsVault,
///       Awakening, Tree of Life), so no threshold values are hardcoded here.
///     - Gear enchanting, restricted to T2/T3 slots: their bonuses apply to ALL heroes, while T1
///       (Weapon/Chest/Boots) only benefits the hero wearing it - per the Gear wiki's Bonuses table,
///       not just the tips guide's opinion, so T2/T3 always wins regardless of hero. Also sidesteps
///       the tension between the tips guide ("concentrate resources on a few core heroes") and this
///       codebase's usual spread-evenly default, since T1 - the part that would need a "which heroes
///       matter" policy - isn't touched here at all.
///     T1 enchanting (needs an active-party policy - user chose "current team, dynamic" over a fixed
///     hero list) and Jewels (structurally parallel enchant system, no tier-unlock action found for
///     it) are a planned follow-up task once this one is live-verified. Soulstones explicitly
///     deferred by the user to a future task (level 200+).
/// </summary>
public class HallOfHeroesGearTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Character;

    internal override float? MaxRuntimeSeconds => 1800f;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    // Safety bound only, matching the pattern used everywhere else in this codebase for a "keep
    // going until nothing's left to do" loop.
    private const int MaxEnchantIterationsPerSlot = 50;

    public override IEnumerator Execute()
    {
        // No Town-building or Character-menu entry point exists for this screen (checked both) - the
        // notification rail icon is the only way in, so unlike every other notification-backed task
        // this is the sole open step, not just an opportunistic fast path.
        yield return Notifications.HallOfHeroes;

        foreach (var hero in HallOfHeroesModel.Heroes)
        {
            yield return HallOfHeroesModel.SelectHero(hero);

            yield return HallOfHeroesModel.OpenGearTab;
            yield return HallOfHeroesModel.GearTierUnlock.OpenGalleryView;

            if (HallOfHeroesModel.GearTierUnlock.UnlockTier2Btn.IsClickable())
                yield return HallOfHeroesModel.GearTierUnlock.UnlockTier2Btn.Click();

            if (HallOfHeroesModel.GearTierUnlock.UnlockTier3Btn.IsClickable())
                yield return HallOfHeroesModel.GearTierUnlock.UnlockTier3Btn.Click();

            yield return HallOfHeroesModel.OpenEnchantingTab;
            yield return HallOfHeroesModel.GearEnchanting.OpenGearCategory;

            foreach (var slot in HallOfHeroesModel.GearEnchanting.PriorityGearSlots)
            {
                var button = HallOfHeroesModel.GearEnchanting.SlotButton(slot);
                var iterations = 0;

                while (button.IsClickable() && iterations < MaxEnchantIterationsPerSlot)
                {
                    iterations++;
                    yield return button.Click();
                }
            }
        }

        yield return HallOfHeroesModel.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
