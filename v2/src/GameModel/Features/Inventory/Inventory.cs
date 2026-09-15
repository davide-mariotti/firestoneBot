using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Inventory;

public static class Inventory
{
    public static IEnumerator Open => new GameButton(Paths.BattleLoc.BottomSideUIDesktopLoc.InventoryBtn).Click();

    public static IEnumerator OpenChestsTab => new GameButton(Paths.InventoryLoc.ChestsTabBtn).Click();

    public static IEnumerator OpenItemsTab => new GameButton(Paths.InventoryLoc.ItemsTabBtn).Click();

    public static IEnumerator Close => new GameButton(Paths.InventoryLoc.CloseBtn).Click();

    public static GameElement Content => new(Paths.InventoryLoc.ContentRoot);

    /// <summary>
    ///     Clicks every item slot (in whichever tab is currently open) whose name looks like one of
    ///     the "instant gold" conversion items (Pouch/Bucket/Crate/Pile of Gold - these convert to
    ///     meteorites when used, per the user). No confirmed exact slot names found via UnityPy (the
    ///     only "gold" GameObjects found were a VFX holder, not the clickable slot itself), so this
    ///     matches generically by name instead of hardcoding possibly-wrong names - safe no-op on
    ///     anything that isn't actually clickable.
    /// </summary>
    public static IEnumerator UseAllGoldItems()
    {
        var slotNames = Content.GetChildren().Select(c => c.Name).ToList();
        foreach (var name in slotNames)
        {
            if (string.IsNullOrEmpty(name)) continue;
            if (!name.ToLowerInvariant().Contains("gold")) continue;

            var slot = new GameButton("/" + name, Content);
            while (slot.IsClickable()) yield return slot.Click();
        }
    }
}

/// <summary>
///     One chest opening flow: click a chest slot (e.g. "commonChestbox") in Inventory.Content,
///     repeatedly pick the biggest available batch (x10 then x1) via ChestOpenPreview/ChestOpening
///     until the desired remaining quantity is reached, then close back to the Inventory grid.
/// </summary>
public static class ChestOpening
{
    /// <summary>
    ///     Opens chests of the given slot (relative to Inventory.Content, e.g. "/commonChestbox")
    ///     down to (not below) targetRemaining. Safe no-op if the slot doesn't exist or is already
    ///     at/below target - every click here is gated by IsClickable() first.
    /// </summary>
    public static IEnumerator OpenDownTo(string slotPath, int targetRemaining)
    {
        var slot = new GameButton(slotPath, Inventory.Content);
        if (!slot.IsClickable()) yield break;

        var quantityTxt = new GameText(slotPath + "/quantity", Inventory.Content);
        var remainingToOpen = quantityTxt.GetParsedInt() - targetRemaining;
        if (remainingToOpen <= 0) yield break;

        yield return slot.Click(); // opens ChestOpenPreview

        var onPreview = true;
        while (remainingToOpen > 0)
        {
            var openX10 = new GameButton(onPreview
                ? Paths.ChestOpenPreviewLoc.OpenX10Btn
                : Paths.ChestOpeningLoc.OpenX10Btn);
            var openX1 = new GameButton(onPreview
                ? Paths.ChestOpenPreviewLoc.OpenX1Btn
                : Paths.ChestOpeningLoc.OpenX1Btn);

            if (remainingToOpen >= 10 && openX10.IsClickable())
            {
                yield return openX10.Click();
                remainingToOpen -= 10;
            }
            else if (openX1.IsClickable())
            {
                yield return openX1.Click();
                remainingToOpen -= 1;
            }
            else
            {
                break; // neither button available (e.g. ran out) - stop rather than loop forever
            }

            onPreview = false; // every subsequent click happens on the ChestOpening results screen
        }

        yield return new GameButton(Paths.ChestOpeningLoc.CloseBtn).Click();
        yield return new GameButton(Paths.ChestOpenPreviewLoc.CloseBtn).Click(); // safe no-op if already closed
    }

    /// <summary>Opens every owned chest of the given slot (down to 0).</summary>
    public static IEnumerator OpenAll(string slotPath) => OpenDownTo(slotPath, 0);
}
