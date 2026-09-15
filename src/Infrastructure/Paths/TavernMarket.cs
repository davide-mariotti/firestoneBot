namespace Firebot.Infrastructure;

/// <summary>
///     Exchanges beer or gems for game tokens, opened from Tavern via TavernLoc.OpenMarketBtn. Never
///     automated before. IMPORTANT: the "gameToken" item (5 tokens) has TWO payment buttons - one priced in
///     beer, one in gems (confirmed via the wiki's two separate rows for the same 5-token offer,
///     matching this item having exactly two purchase buttons). Which button is which could NOT be
///     confirmed via UnityPy (the currency icon sprite reference didn't resolve statically) - assumed
///     "purchaseButton" = beer (listed first, matching the wiki's row order) and "purchaseButtonBulk"
///     = gems, by structural position only. Only BeerBtn (assumed) is ever wired to auto-click -
///     GemsBtn is deliberately never referenced by any task. Flag for live verification before
///     trusting this unsupervised: if wrong, this would spend gems instead of beer.
/// </summary>
public static partial class Paths
{
    public static class TavernMarketLoc
    {
        private const string Root = MenusLoc.Root + "/menus/TavernMarket";

        public const string CloseBtn = Root + "/bg/closeButton";

        private const string FiveTokenItemRoot = Root + "/bg/items/gameToken";

        public const string BuyFiveTokensWithBeerBtn = FiveTokenItemRoot + "/purchaseButton";

        // Deliberately unused by any task - assumed (not confirmed) to be the gems-priced button.
        public const string BuyFiveTokensWithGemsBtnDoNotUse = FiveTokenItemRoot + "/purchaseButtonBulk";
    }
}
