namespace Firebot.Infrastructure;

/// <summary>
///     Scarab Game (a Chaos Rift-linked mini-game) and its shop. No v1 precedent at all. Unlike
///     every other feature in this codebase, no Town/Guild building icon leads here - the only known
///     entry point is the battle-screen notification rail (both ScarabGame and
///     ScarabGameShopFreeToken are confirmed there via a fresh UnityPy scan). Flag for live
///     verification: it's unclear whether the ScarabGame icon is a permanent HUD element (gated only
///     by unlock/account level, like Store/Guild) or genuinely only appears when the rail badge is
///     lit (unlike every other rail entry, which all have a separate always-there manual path).
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
