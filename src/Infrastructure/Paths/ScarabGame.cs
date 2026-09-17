namespace Firebot.Infrastructure;

/// <summary>
///     Scarab's Game (a slot-machine minigame) and its shop. No prior precedent at all. Corrected
///     twice: first from "no permanent manual entry point" to "Town -&gt; Tavern -&gt; Tavern's own 'shop'
///     button" (per the wiki), then live-confirmed 2026-09-18 that the real route is Town -&gt; the
///     "tavern" building's TavernSelection choice popup -&gt; its "scarabGame" card directly - it never
///     actually goes through the card-flip Tavern screen at all (see Town.OpenScarabGame). Unlocks at
///     character level 60 per the wiki (Tavern itself unlocks earlier, at level 15).
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
    // not yet affordable. Live-confirmed, 2026-09-18: it's a "menus/" screen, not a "popups/" one -
    // the originally assumed "popups/PharaohsVault" never resolved at all (same wrong-guess pattern
    // already seen on BattlePass and TavernMarket), while a generic Watchdog sweep found
    // "menus/PharaohsVault/closeButton" resolving fine.
    public static class PharaohsVaultLoc
    {
        private const string Root = MenusLoc.Root + "/menus/PharaohsVault";

        public const string CloseBtn = Root + "/closeButton";

        public const string OpenBtn = Root + "/openButton";

        // Same bet-multiplier pattern as ScarabGameLoc.ChangeBetBtn above - always use the biggest
        // available (proportional cost/reward, fewer clicks to spend the same coin budget).
        public const string ChangeQuantityBtn = Root + "/bottomRightUI/changeQuantity";

        public const string QuantityTxt = ChangeQuantityBtn + "/text";
    }

    // Live-confirmed, 2026-09-18: this is a popup, not a menu (same wrong-guess pattern already seen
    // on BattlePass/TavernMarket/PharaohsVault) - a generic active-screen dump found it listed under
    // "popups" (with "menus/ScarabGame" still active underneath it, confirming it's an overlay).
    public static class ScarabGameShopLoc
    {
        private const string Root = MenusLoc.Root + "/popups/ScarabGameShop";

        public const string CloseBtn = Root + "/closeButton";

        // DIAGNOSTIC (2026-09-18): the user found a SEPARATE free claim ("Pharaoh's token x1") under
        // a "Monthly pass" tab, not yet handled - only the "sale" tab's free item is claimed today.
        // Exposed to find the tab's real internal name and item structure, and whether a "bg" wrapper
        // actually exists at this level (unconfirmed - the old menus/ root guess never resolved far
        // enough to tell).
        public const string ContentRoot = Root;

        public const string SubmenuButtonsRoot = Root + "/bg/submenuButtons";
        public const string SubmenusRoot = Root + "/bg/submenus";

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
