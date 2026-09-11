using System;
using System.Collections;
using System.Collections.Generic;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotActions;

public static class AutoUpgrade
{
    private const float HoldPerButtonSeconds = 0.5f;
    private const float GapBetweenButtonsSeconds = 0.5f;
    private const float IdlePollSeconds = 0.5f;
    private static readonly WaitForSeconds GapBetweenButtonsWait = new(GapBetweenButtonsSeconds);
    private static readonly WaitForSeconds IdlePollWait = new(IdlePollSeconds);
    private static readonly List<CachedGameButton> ButtonsBuffer = new();
    private static bool _isRunning;
    private static object _autoUpgradeRoutineHandle;
    private static bool _isInitialized;
    private static string _cachedLevel = string.Empty;
    private static MelonPreferences_Entry<bool> _isEnabled;
    private static MelonPreferences_Entry<string> _upgradeTargetSlots;

    private static bool IsEnabled => _isEnabled?.Value ?? false;

    public static void Initialize()
    {
        if (_isInitialized) return;

        var clazzName = StringUtils.Humanize(nameof(AutoUpgrade));
        var sectionId = clazzName.Replace(" ", "_").ToLowerInvariant();

        var section = MelonPreferences.CreateCategory(sectionId, $"{clazzName} Settings");
        section.SetFilePath(BotSettings.ConfigPath);

        _isEnabled = section.CreateEntry(
            "enabled",
            false,
            "Enable AutoUpgrade",
            "Enables or disables the AutoUpgrade automation task. Starts and stops together with the main bot (shortcut_key in [firebot_settings]). Default: false."
        );

        _upgradeTargetSlots = section.CreateEntry(
            "upgrade_target_slots",
            "",
            "Upgrade Target Slots",
            "AUTOUPGRADE TARGET SLOT CONFIGURATION. " +
            "\nThis setting controls which upgrade slots (leader/heroes) will be upgraded. " +
            "\nSLOT IDs ARE ZERO-BASED and range from 0 to 6. " +
            "\nSLOT MAP: 0 = Leader, 1 to 6 = Heroes. " +
            "\nTASK PRIORITY: Main bot tasks always have priority over AutoUpgrade. " +
            "\nAUTO PAUSE/RESUME: AutoUpgrade pauses while a main task is actively executing and resumes automatically right after. " +
            "\nHOW TO USE: Enter comma-separated slot IDs to select the targets to upgrade. " +
            "\nEXAMPLES: '0,6' = only slots 0 and 6 will be upgraded. '3,1,5' = only slots 3, 1 and 5. " +
            "\nIf empty, AutoUpgrade will upgrade all slots. " +
            "\nInvalid values are ignored."
        );

        section.SaveToFile();
        _isInitialized = true;
        Logger.Info("AutoUpgrade configuration initialized.");
    }

    private static List<CachedGameButton> AllButtons()
    {
        var buttons = new List<CachedGameButton>
        {
            new(Paths.BattleLoc.BottomSideUINewLoc.LeaderPanelLoc.LvlUpBtn)
        };

        var heroSlots = new GameElement(Paths.BattleLoc.BottomSideUINewLoc.HeroSlotsLoc.Root);
        foreach (var slot in heroSlots.GetChildren())
            buttons.Add(new CachedGameButton(Paths.BattleLoc.BottomSideUINewLoc.HeroSlotsLoc.LvlUpBtn, slot));

        return buttons;
    }

    private static List<CachedGameButton> Buttons()
    {
        ButtonsBuffer.Clear();
        var allButtons = AllButtons();
        var selectedTargetSlots = GetSelectedTargetSlots();

        if (selectedTargetSlots.Length == 0)
        {
            ButtonsBuffer.AddRange(allButtons);
            return ButtonsBuffer;
        }

        foreach (var position in selectedTargetSlots)
            if (position >= 0 && position < allButtons.Count)
                ButtonsBuffer.Add(allButtons[position]);

        return ButtonsBuffer;
    }

    private static int[] GetSelectedTargetSlots()
    {
        if (_upgradeTargetSlots == null) return Array.Empty<int>();

        var value = _upgradeTargetSlots.Value;
        if (string.IsNullOrWhiteSpace(value)) return Array.Empty<int>();

        var parts = value.Split(',');
        var result = new List<int>(parts.Length);
        var seen = new bool[7];

        foreach (var part in parts)
        {
            var candidate = part.Trim();
            if (!int.TryParse(candidate, out var slot)) continue;
            if (slot < 0 || slot > 6) continue;
            if (seen[slot]) continue;

            seen[slot] = true;
            result.Add(slot);
        }

        return result.Count > 0 ? result.ToArray() : Array.Empty<int>();
    }

    private static bool IsTopTier(string levelText) =>
        levelText.IndexOf("x100", StringComparison.OrdinalIgnoreCase) >= 0 ||
        levelText.IndexOf("MAX", StringComparison.OrdinalIgnoreCase) >= 0;

    /// <summary>
    ///     Cycles the buy-quantity toggle until it reaches x100 or MAX, whichever the button offers,
    ///     so each upgrade click spends as much gold as possible in one go.
    /// </summary>
    private static IEnumerator SetUpgradeLevel()
    {
        var levelTxt = new GameText(Paths.BattleLoc.BottomSideUINewLoc.ChangeLevelUpModeLoc.Text);
        var toggleBtn = new GameButton(Paths.BattleLoc.BottomSideUINewLoc.ChangeLevelUpModeLoc.Button);

        var currentLevel = levelTxt.GetParsedText();

        if (!string.IsNullOrEmpty(currentLevel) &&
            string.Equals(currentLevel, _cachedLevel, StringComparison.Ordinal))
            yield break;

        if (!string.IsNullOrEmpty(currentLevel) && IsTopTier(currentLevel))
        {
            _cachedLevel = currentLevel;
            yield break;
        }

        const int maxAttempts = 10; // Prevent infinite loop in case of unexpected issues
        for (var attempts = 0; attempts < maxAttempts; attempts++)
        {
            yield return toggleBtn.Click();
            var nextLevel = levelTxt.GetParsedText();

            if (string.IsNullOrEmpty(nextLevel)) break;
            if (IsTopTier(nextLevel))
            {
                currentLevel = nextLevel;
                break;
            }

            // Cycled all the way back to where we started without finding x100/MAX.
            if (string.Equals(nextLevel, currentLevel, StringComparison.Ordinal)) break;

            currentLevel = nextLevel;
        }

        _cachedLevel = currentLevel;
    }

    public static void Start()
    {
        if (!IsEnabled) return;
        if (_isRunning) return;
        _isRunning = true;
        _autoUpgradeRoutineHandle = MelonCoroutines.Start(UpgradeLoop());
        Logger.Info("AutoUpgrade started.");
    }

    public static void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        if (_autoUpgradeRoutineHandle != null) MelonCoroutines.Stop(_autoUpgradeRoutineHandle);
        _autoUpgradeRoutineHandle = null;
        Logger.Info("AutoUpgrade stopped.");
    }

    private static IEnumerator UpgradeLoop()
    {
        while (_isRunning)
        {
            if (BotManager.ShouldPauseBackgroundTasks())
            {
                yield return IdlePollWait;
                continue;
            }

            yield return SetUpgradeLevel();

            var buttons = Buttons();
            if (buttons.Count == 0)
            {
                yield return null;
                continue;
            }

            foreach (var buyUpgradeBtn in buttons)
            {
                if (BotManager.ShouldPauseBackgroundTasks()) break;

                Logger.Debug($"Name: {buyUpgradeBtn.Name}, IsVisible: {buyUpgradeBtn.IsVisible()}");
                yield return buyUpgradeBtn.HoldButton(HoldPerButtonSeconds);
                yield return GapBetweenButtonsWait;
            }
        }
    }
}
