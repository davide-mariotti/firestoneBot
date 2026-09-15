namespace Firebot.Infrastructure;

/// <summary>
///     Scarab's Game (a slot-machine minigame) and its shop. No prior precedent at all. Corrected after
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

        public const string OpenVaultBtn = Root + "/helpCanvas/actionButtons/pharaohVault";

        // Spins the slot machine. Shows both a Noble Token cost and a Pharaoh Token cost side by
        // side (interchangeable per the wiki) - confirmed by the user that the game always draws
        // from the free Noble Tokens (10/day) first and this button simply becomes non-clickable
        // once they're exhausted, never silently spending Pharaoh Tokens (bought currency). Safe to
        // click purely via IsClickable() like everywhere else in this codebase.
        public const string SpinBtn = Root + "/helpCanvas/playButton";

        // Cycles the spin's bet multiplier (shows the current value in its "text" child - exact
        // label format, e.g. "x10" vs "10", not verified live). Per the user: always use the
        // biggest available multiplier, since it's a proportional bet/payout (same expected coins
        // per token spent, fewer clicks).
        public const string ChangeBetBtn = Root + "/helpCanvas/bottomRightUI/changePlayQuantity";

        public const string BetQuantityTxt = ChangeBetBtn + "/text";
    }

    // Opened by ScarabGameLoc.OpenVaultBtn. Spends accumulated Ancient Coins (5000 per the wiki) for
    // 5 random rewards - no real choice involved, openButton is a safe no-op via IsClickable() if
    // not yet affordable. Root path follows the same popups/&lt;Name&gt; convention as every other
    // popup opened from a menu screen (ChestOpenPreview, TalentPreview, etc.) - not independently
    // cross-checked for this specific popup, flag for live verification.
    public static class PharaohsVaultLoc
    {
        private const string Root = MenusLoc.Root + "/popups/PharaohsVault";

        public const string CloseBtn = Root + "/closeButton";

        public const string OpenBtn = Root + "/openButton";

        // Same bet-multiplier pattern as ScarabGameLoc.ChangeBetBtn above - always use the biggest
        // available (proportional cost/reward, fewer clicks to spend the same coin budget).
        public const string ChangeQuantityBtn = Root + "/bottomRightUI/changeQuantity";

        public const string QuantityTxt = ChangeQuantityBtn + "/text";
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
