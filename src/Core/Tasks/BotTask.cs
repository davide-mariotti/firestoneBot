using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Firebot.GameModel.Base;
using MelonLoader;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Core.Tasks;

public abstract class BotTask
{
    private readonly string _className;
    private MelonPreferences_Category _category;
    private MelonPreferences_Entry<bool> _enabledEntry;
    private MelonPreferences_Entry<string> _nextRunTimeEntry;
    private GameElement _notificationElement;

    protected BotTask()
    {
        _className = GetType().Name;
    }

    public string SectionTitle => Humanize(GetType().Name);

    public DateTime NextRunTime { get; protected set; } = DateTime.MinValue;

    public DateTime? LastRunTime { get; set; }

    protected virtual string NotificationPath => null;

    public bool IsEnabled => _enabledEntry != null && _enabledEntry.Value;

    private GameElement NotificationElement
    {
        get
        {
            if (_notificationElement != null) return _notificationElement;
            if (string.IsNullOrEmpty(NotificationPath)) return null;

            _notificationElement = new GameElement(NotificationPath);
            return _notificationElement;
        }
    }

    public void InitializeConfig(string configPath)
    {
        if (_enabledEntry != null) return;

        var sectionId = SectionTitle.Replace(" ", "_").ToLowerInvariant();
        _category = MelonPreferences.CreateCategory(sectionId, $"{SectionTitle} Settings");
        _category.SetFilePath(configPath);

        _enabledEntry = _category.CreateEntry("enabled", false, "Enable Task",
            $"Enables or disables the {SectionTitle} automation task." +
            $"\nWhen disabled, this task will be ignored during the execution loop.");

        _nextRunTimeEntry = _category.CreateEntry("next_run_time_internal", "", "Next Run Time (internal)",
            "Bot-managed: remembers when this task should next check, across game/bot restarts. " +
            "Do not edit manually.");

        if (DateTime.TryParse(_nextRunTimeEntry.Value, out var savedNextRunTime) && savedNextRunTime > DateTime.Now)
            NextRunTime = savedNextRunTime;

        OnConfigure(_category);
        _category.SaveToFile();
    }

    /// <summary>
    ///     Writes the current NextRunTime to disk so a bot/game restart doesn't forget a real
    ///     in-game cooldown and re-check everything immediately.
    /// </summary>
    public void PersistNextRunTime()
    {
        if (_nextRunTimeEntry == null) return;

        _nextRunTimeEntry.Value = NextRunTime.ToString("O");
        _category.SaveToFile();
    }

    protected virtual void OnConfigure(MelonPreferences_Category category) { }

    public bool IsReady()
        => IsNotificationVisible() || (IsEnabled && DateTime.Now >= NextRunTime);

    public bool IsNotificationVisible()
        => IsEnabled && NotificationElement != null && NotificationElement.IsVisible();

    public abstract IEnumerator Execute();

    /// <summary>
    ///     Called after every execution. If the task's own logic already scheduled a real future run
    ///     (something was claimed, started, or is on a genuine cooldown), this does nothing. Otherwise
    ///     (nothing to do this cycle) it retries after minDelay instead of immediately re-tying for the
    ///     next scan cycle. Notification-driven tasks are unaffected: a visible notification badge is
    ///     still checked every cycle regardless of this floor.
    /// </summary>
    public void EnsureMinimumNextRun(TimeSpan minDelay)
    {
        var floor = DateTime.Now + minDelay;
        if (NextRunTime < floor) NextRunTime = floor;
    }

    protected void Debug(string message, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        => Logger.Debug($"[{_className}::{member}:{line}] {message}");
}