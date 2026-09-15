namespace Firebot.Infrastructure;

/// <summary>
///     Tree of Life - two separate trees (Personal, kept even if you leave your guild; Guild, shared
///     with guildmates), both bought with a currency (the personal one with Expedition Tokens per the
///     wiki). Only the Personal tab is wired here, per the user. Reached from Guild -&gt;
///     TownGuildLoc.TreeOfLifeBtn. No v1 precedent - this whole feature is new, found via a targeted
///     UnityPy scan requested by the user.
/// </summary>
public static partial class Paths
{
    public static class TreeOfLifeLoc
    {
        private const string Root = MenusLoc.Root + "/menus/TreeOfLife";

        public const string CloseBtn = Root + "/closeButton";

        public const string PersonalTabBtn = Root + "/submenuButtons/personalTree";

        // Index 0-19, confirmed via UnityPy: the wiki's Personal Tree table lists exactly 20
        // upgrades, matching treeOfLifePersonalUpgrade (0)-(19) found live 1:1 (same validation
        // technique already used for the Talents tree - see GameModel/Features/Guild/TreeOfLife.cs
        // for the name catalog built from that table's row order).
        public const string PersonalNodeRoot = Root + "/submenus/personalTree/talentTree";

        // Relative to a node - shows its current level directly on the grid (no separate preview
        // popup was found for this feature, unlike Talents/Firestone Research/Meteorite Research -
        // clicking a node is assumed to buy its next level directly, gated by the node's own
        // clickability (affordability/cap), same as every other "click while affordable" purchase in
        // this codebase. Flag for live verification since this is the one tree-upgrade feature this
        // session that DIDN'T turn out to have a preview popup.
        public const string NodeLevelTxt = "/levelBg/level";
    }
}
