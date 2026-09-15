namespace Firebot.Infrastructure;

/// <summary>
///     The Guild's Arcane Crystal screen (a shared, guild-wide "boss" you chip away at with
///     pickaxes). No v1 precedent - never automated before. Reached via TownGuildLoc.ArcaneCrystalBtn.
/// </summary>
public static partial class Paths
{
    public static class ArcaneCrystalLoc
    {
        private const string Root = MenusLoc.Root + "/menus/ArcaneCrystal";

        public const string CloseBtn = Root + "/closeButton";

        // Costs pickaxes per hit (confirmed via UnityPy: sits next to a pickaxeIcon) - exact cost
        // not statically known, read live via GameButton's own safe no-op if unaffordable.
        public const string HitBtn = Root + "/crystalParent/hitButton";
    }
}
