using System;
using System.Collections;
using Firebot.BotActions;
using Firebot.Core.Tasks;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;
using MelonLoader;

namespace Firebot.Behaviors.Town;

public class TempleOfEternalsTask : BotTask
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(5);

    private MelonPreferences_Entry<float> _minResetRatio;
    private MelonPreferences_Entry<int> _minAdventureMinutes;
    private MelonPreferences_Entry<int> _maxAdventureMinutes;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_minResetRatio != null) return;

        _minResetRatio = category.CreateEntry(
            "min_reset_ratio",
            2.0f,
            "Minimum Reset Ratio",
            "Resets (empowers) the Temple of Eternals once the Firestones found in the current adventure " +
            "reach this multiple of the Firestones already banked in the Temple. Default: 2.0 (found >= 2x banked)."
        );

        _minAdventureMinutes = category.CreateEntry(
            "min_adventure_minutes",
            60,
            "Minimum Adventure Minutes",
            "Minimum time (in minutes) that must have passed in the current adventure before the bot " +
            "considers resetting, even if the ratio above is already met. Default: 60."
        );

        _maxAdventureMinutes = category.CreateEntry(
            "max_adventure_minutes",
            120,
            "Maximum Adventure Minutes",
            "Once the current adventure has run for this many minutes, the bot resets regardless of " +
            "the ratio above. Set to 0 to disable (only the ratio decides). Default: 120."
        );
    }

    public override IEnumerator Execute()
    {
        yield return new GameButton(Paths.BattleLoc.RightSideUILoc.TownBtn).Click();
        yield return new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TownIrongardLoc.TempleOfEternalsBtn).Click();

        var timePlayedText =
            new GameText(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TempleOfEternalsLoc.AdventureTimePlayedTxt)
                .GetParsedText();
        var found = new GameText(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TempleOfEternalsLoc.FirestonesFoundTxt)
            .GetParsedDoubleAbbreviated();
        var owned = new GameText(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TempleOfEternalsLoc.FirestonesYouOwnTxt)
            .GetParsedDoubleAbbreviated();

        var timePlayed = TimeParser.ParseFrom(timePlayedText);
        var ratio = owned > 0 ? found / owned : 0;
        var minRatio = _minResetRatio?.Value ?? 2.0f;
        var minDuration = TimeSpan.FromMinutes(_minAdventureMinutes?.Value ?? 60);
        var maxMinutes = _maxAdventureMinutes?.Value ?? 120;
        var maxDuration = maxMinutes > 0 ? TimeSpan.FromMinutes(maxMinutes) : TimeSpan.MaxValue;

        Debug($"[INFO] Adventure time: {timePlayed}, Firestones found: {found}, Temple's Firestones: {owned}, " +
              $"Ratio: {ratio:0.##} (need {minRatio:0.##} after {minDuration}, or force reset after {maxDuration}).");

        if ((timePlayed >= minDuration && ratio >= minRatio) || timePlayed >= maxDuration)
        {
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TempleOfEternalsLoc.EmpowerBtn).Click();
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.EmpowerPopupLoc.EmpowerBtn).Click();
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.ActionRequiredLoc.ConfirmBtn).Click();
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.TOEPrestigeCompleteLoc.ConfirmBtn).Click();
            AutoRetreat.OnAdventureReset();

            NextRunTime = DateTime.Now + RetryDelay;
        }
        else
        {
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TempleOfEternalsLoc.CloseBtn).Click();
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.TownIrongardLoc.CloseBtn).Click();

            // If the minimum adventure time hasn't elapsed yet, there is no point checking again
            // before it does - skip straight to the moment it will (plus the ratio not being met
            // yet still falls back to the regular retry delay).
            var timeUntilMinDuration = minDuration - timePlayed;
            NextRunTime = DateTime.Now + (timeUntilMinDuration > RetryDelay ? timeUntilMinDuration : RetryDelay);
        }
    }
}
