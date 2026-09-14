namespace Firebot.Infrastructure;

/// <summary>
///     Scarab's Game (a slot-machine minigame) and its shop. No v1 precedent at all. Corrected after
///     initially assuming there was no permanent manual entry point (only the battle-screen
///     notification badges) - the user pointed out and the wiki confirms
///     (firestone-idle-rpg.fandom.com/wiki/Tavern) it's reached via Town/townButton -&gt; Tavern
///     (TavernLoc in Menus.cs) -&gt; Tavern's own "shop" action button, same guaranteed-navigation
///     pattern as every other task. Unlocks at character level 60 per the wiki (Tavern itself
///     unlocks earlier, at level 15).
/// </summary>
public static partial class Paths
{
    public static class ScarabGameLoc
    {
        private const string Root = MenusLoc.Root + "/menus/ScarabGame";

        public const string CloseBtn = Root + "/closeButton";

        public const string OpenShopBtn = Root + "/helpCanvas/actionButtons/shop";
    }

    public static class ScarabGameShopLoc
    {
        private const string Root = MenusLoc.Root + "/menus/ScarabGameShop";

        public const string CloseBtn = Root + "/closeButton";

        public const string SaleTabBtn = Root + "/bg/submenuButtons/sale/button";

        public static class FreeTokenLoc
        {
            private const string ItemRoot = Root + "/bg/submenus/sale/items/scarabGameShopFreeTokenInteraction";

            // Named "purchaseButton" but confirmed genuinely free - sibling "freeText" label
            // ("Gratis"), same distinguishing pattern already used for Task 3's mystery box.
            public const string ClaimBtn = ItemRoot + "/claimBg/purchaseButton";
        }
    }
}
