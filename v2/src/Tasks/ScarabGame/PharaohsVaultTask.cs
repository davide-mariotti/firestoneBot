using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using PharaohsVaultScreen = Firebot.GameModel.Features.ScarabGame.PharaohsVault;
using ScarabGameScreen = Firebot.GameModel.Features.ScarabGame.ScarabGame;
using TavernScreen = Firebot.GameModel.Features.Town.Tavern;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.ScarabGame;

/// <summary>
///     Spins Scarab's Game slot machine with free Noble Tokens (10/day) and spends the resulting
///     Ancient Coins to open Pharaoh's Vault (5000 coins per open, per the wiki) - both completely
///     unhandled before, separate from ScarabGameFreeTokenTask (which only claims an unrelated free
///     shop item). Requested by the user after reviewing an external tips guide (note.com).
///     Always uses the biggest available bet/quantity multiplier on both the spin and the vault open
///     (per the user - a proportional bet/payout, so it's strictly fewer clicks for the same result).
///     Confirmed by the user: the spin button always draws from free Noble Tokens first and simply
///     becomes non-clickable once they're exhausted - it never silently spends Pharaoh Tokens (bought
///     with gems), so clicking purely via IsClickable() is safe here, same as everywhere else.
///     Vault rewards can include jewel/celestial chests (opened separately by CollectorQuestTask,
///     extended for this) and Sigils of Prophecy (used to release Beasts - a separate system, out of
///     scope for now per the user).
///     Level-gated like ScarabGameFreeTokenTask - the wiki's Scarab's Game infobox lists "unlock =
///     Level 60".
/// </summary>
public class PharaohsVaultTask : BotTask
{
    private const int MinimumCharacterLevel = 60;
    private static readonly TimeSpan RecheckDelayBelowLevel = TimeSpan.FromHours(1);
    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(6);

    public override IEnumerator Execute()
    {
        if (PlayerAvatar.CharacterLevel < MinimumCharacterLevel)
        {
            NextRunTime = DateTime.Now + RecheckDelayBelowLevel;
            yield break;
        }

        yield return TownScreen.Open;
        yield return TownScreen.OpenTavern;
        yield return TavernScreen.OpenScarabGame;

        yield return ScarabGameScreen.MaxOutBet();
        var spinBtn = ScarabGameScreen.SpinBtn;
        while (spinBtn.IsClickable()) yield return spinBtn.Click();

        yield return ScarabGameScreen.OpenVault;
        yield return PharaohsVaultScreen.MaxOutQuantity();
        var openBtn = PharaohsVaultScreen.OpenBtn;
        while (openBtn.IsClickable()) yield return openBtn.Click();
        yield return PharaohsVaultScreen.Close;

        yield return ScarabGameScreen.Close;
        yield return TavernScreen.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
