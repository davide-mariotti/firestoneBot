using System.Collections;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Character;

public static class HallOfHeroes
{
    public static IEnumerator Close => new GameButton(Paths.HallOfHeroesLoc.CloseBtn).Click();

    private static GameElement HeroGrid => new(Paths.HallOfHeroesLoc.HeroGridRoot);

    /// <summary>Every real hero slot - "allHeroesButton" (trailing sibling, opens a different screen
    /// entirely) is filtered out by name, confirmed via UnityPy.</summary>
    public static GameElement[] Heroes => HeroGrid.GetChildren().Where(h => h.Name.StartsWith("hero (")).ToArray();

    public static IEnumerator SelectHero(GameElement hero) => new GameButton(parent: hero).Click();

    public static IEnumerator OpenGearTab => new GameButton(Paths.HallOfHeroesLoc.GearTabBtn).Click();

    public static IEnumerator OpenEnchantingTab => new GameButton(Paths.HallOfHeroesLoc.EnchantingTabBtn).Click();

    public static class GearTierUnlock
    {
        public static IEnumerator OpenGalleryView =>
            new GameButton(Paths.HallOfHeroesLoc.GearSubmenuLoc.GalleryViewBtn).Click();

        public static GameButton UnlockTier2Btn => new(Paths.HallOfHeroesLoc.GearSubmenuLoc.UnlockTier2Btn);

        public static GameButton UnlockTier3Btn => new(Paths.HallOfHeroesLoc.GearSubmenuLoc.UnlockTier3Btn);
    }

    public static class GearEnchanting
    {
        public static IEnumerator OpenGearCategory =>
            new GameButton(Paths.HallOfHeroesLoc.EnchantingSubmenuLoc.GearCategoryTabBtn).Click();

        private static GameElement GearGrid => new(Paths.HallOfHeroesLoc.EnchantingSubmenuLoc.GearGridRoot);

        // T2 (Wrist/Shoulder/Belt, indices 3-5) and T3 (Ring/Relic, indices 6-7) give an all-heroes
        // bonus vs T1's (Weapon/Chest/Boots, indices 0-2) single-hero bonus per the Gear wiki's
        // Bonuses table, so those 5 slots always come first. T1 is deliberately excluded here (not
        // just deprioritized) - it needs an active-party policy that's a separate follow-up task.
        public static readonly int[] PriorityGearSlots = { 3, 4, 5, 6, 7 };

        public static GameButton SlotButton(int index) => new(parent: GearGrid.GetChild(index));
    }
}
