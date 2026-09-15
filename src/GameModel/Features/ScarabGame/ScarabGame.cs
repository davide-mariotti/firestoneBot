using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.ScarabGame;

public static class ScarabGame
{
    public static IEnumerator OpenShop => new GameButton(Paths.ScarabGameLoc.OpenShopBtn).Click();

    public static IEnumerator OpenVault => new GameButton(Paths.ScarabGameLoc.OpenVaultBtn).Click();

    public static GameButton SpinBtn => new(Paths.ScarabGameLoc.SpinBtn);

    /// <summary>Clicks the bet-multiplier cycle button until it shows the biggest option (best-effort -
    /// stops early if the button stops responding, since the exact number/labels of options and
    /// whether it wraps or clamps aren't verified live).</summary>
    public static IEnumerator MaxOutBet() =>
        CycleToMax(new GameButton(Paths.ScarabGameLoc.ChangeBetBtn), new GameText(Paths.ScarabGameLoc.BetQuantityTxt));

    public static IEnumerator Close => new GameButton(Paths.ScarabGameLoc.CloseBtn).Click();

    /// <summary>Shared by ScarabGame's bet selector and PharaohsVault's quantity selector - both are a
    /// single button that cycles a multiplier, shown in a sibling text label. Bounded attempts since
    /// the exact option count/labels aren't confirmed live.</summary>
    internal static IEnumerator CycleToMax(GameButton cycleBtn, GameText quantityTxt)
    {
        const int maxAttempts = 4;

        for (var i = 0; i < maxAttempts; i++)
        {
            if (quantityTxt.GetParsedText().Contains("10")) yield break;
            if (!cycleBtn.IsClickable()) yield break;
            yield return cycleBtn.Click();
        }
    }
}

public static class ScarabGameShop
{
    public static IEnumerator OpenSaleTab => new GameButton(Paths.ScarabGameShopLoc.SaleTabBtn).Click();

    public static IEnumerator ClaimFreeToken => new GameButton(Paths.ScarabGameShopLoc.FreeTokenLoc.ClaimBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.ScarabGameShopLoc.CloseBtn).Click();
}

/// <summary>Opened by ScarabGame.OpenVault - spends Ancient Coins (5000 per the wiki) for 5 random
/// rewards, including jewel/celestial chests (opened separately by CollectorQuestTask) and Sigils of
/// Prophecy (used to release Beasts - a separate system, not automated here).</summary>
public static class PharaohsVault
{
    public static GameButton OpenBtn => new(Paths.PharaohsVaultLoc.OpenBtn);

    public static IEnumerator MaxOutQuantity() => ScarabGame.CycleToMax(
        new GameButton(Paths.PharaohsVaultLoc.ChangeQuantityBtn), new GameText(Paths.PharaohsVaultLoc.QuantityTxt));

    public static IEnumerator Close => new GameButton(Paths.PharaohsVaultLoc.CloseBtn).Click();
}
