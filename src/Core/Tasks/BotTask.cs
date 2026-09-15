using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Firebot.GameModel.Base;
using Firebot.GameModel.Shared;
using MelonLoader;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Core.Tasks;

/// <summary>
///     Thematic grouping for the config file and the terminal status table - not a game-code
///     concept, purely so ~25 tasks stay navigable instead of appearing in whatever arbitrary order
///     reflection happens to return them in. Declaration order here IS display order (both places
///     sort by this first). "Quests" groups the 6 tasks that drive/claim the 9 daily quests together
///     regardless of which in-game screen they each actually use, since that's what's relevant when
///     scanning for them - not the underlying screen a Town/Guild/Map/etc. grouping would imply.
///     "Map" and "Warfront" are both reached from the same WorldMap screen (its two tabs) but kept
///     separate per the user: Map Missions is unlocked from the start and dispatches missions in
///     ascending/descending time order (matches the original mission-dispatch behavior), while Warfront Campaign is an
///     unrelated level-50 sub-feature (war machines) with its own loot/daily-mission tasks - grouping
///     them together would hide that they're functionally unrelated beyond sharing a screen.
/// </summary>
public enum TaskGroup
{
    Quests,
    Town,
    Guild,
    Map,
    Warfront,
    Character,
    ScarabGame
}

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

    /// <summary>Which section of the terminal table / config file this task belongs in - see
    /// TaskGroup. Every task must declare one; there's no sensible generic default. Internal (not
    /// protected) so BotManager can sort on it directly.</summary>
    internal abstract TaskGroup Group { get; }

    private static string GroupLabel(TaskGroup group) => group switch
    {
        TaskGroup.ScarabGame => "Scarab Game",
        _ => group.ToString()
    };

    public string SectionTitle
    {
        get
        {
            var group = GroupLabel(Group);
            var name = Humanize(GetType().Name);
            // Avoids "Quests - Quests" for a task whose humanized name already matches its group
            // (e.g. QuestsTask, which just claims quest rewards rather than driving one specific
            // quest's progress).
            return name == group ? group : $"{group} - {name}";
        }
    }

    public DateTime NextRunTime { get; protected set; } = DateTime.MinValue;

    public DateTime? LastRunTime { get; set; }

    protected virtual string NotificationPath => null;

    /// <summary>
    ///     Character level this task's underlying feature unlocks at, per the wiki - 0 (default)
    ///     means no known/relevant gate. Enforced generically here (IsReady/IsNotificationVisible)
    ///     instead of each task re-implementing its own "below level, reschedule later" boilerplate:
    ///     a task below its level requirement is simply never ready, and reacts within one scan cycle
    ///     of actually reaching it (no separate recheck-delay bookkeeping needed).
    /// </summary>
    protected virtual int MinimumCharacterLevel => 0;

    private bool MeetsLevelRequirement => PlayerAvatar.CharacterLevel >= MinimumCharacterLevel;

    /// <summary>
    ///     Per-task override for how long BotManager lets a single Execute() run before forcibly
    ///     abandoning it (see BotManager.RunSafe) - null (default, almost every task) means "use the
    ///     global BotSettings.MaxTaskRuntime". Exists for the rare task whose OWN legitimate worst
    ///     case (not a bug - a deliberately bounded retry loop) can run long: raising the global
    ///     limit for every task just to accommodate one would weaken the safety net everywhere else.
    ///     A task that gets cut off mid-run isn't corrupted by it - Watchdog's cleanup sweep (runs
    ///     right after, unconditionally) closes whatever got left open, same as any other interruption.
    /// </summary>
    internal virtual float? MaxRuntimeSeconds => null;

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

        var sectionId = _className.ToLowerInvariant();
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
        => MeetsLevelRequirement && (IsNotificationVisible() || (IsEnabled && DateTime.Now >= NextRunTime));

    public bool IsNotificationVisible()
        => MeetsLevelRequirement && IsEnabled && NotificationElement != null && NotificationElement.IsVisible();

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
