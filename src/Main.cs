using Firebot.BotActions;
using Firebot.Core;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;
using Main = Firebot.Main;

[assembly: MelonInfo(typeof(Main), "Firebot", "0.1.0", "davide-mariotti", "https://github.com/davide-mariotti/firestoneBot")]
[assembly: MelonGame]

[assembly: MelonColor(255, 255, 0, 255)]
[assembly: MelonAuthorColor(255, 0, 255, 0)]

namespace Firebot;

/// <summary>
///     Firebot (see PLAN.md for the task list and design notes). The build does NOT deploy to the
///     game's live Mods folder (see Directory.Build.props) so working on it can't silently overwrite
///     the dll that's actually running on the production bots - copy it over by hand once a change
///     is ready to go live.
/// </summary>
public class Main : MelonMod
{
    private bool _isGameReady;

    public override void OnInitializeMelon()
    {
        BotSettings.Initialize();
        BotManager.Initialize();
        HeroUpgrade.Initialize();
        AutoRetreat.Initialize();

        Logger.Info("Firebot System Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _isGameReady = sceneName == "mainScene" || sceneName == "Game";

        if (_isGameReady)
        {
            if (BotSettings.AutoStart) BotManager.Start();
        }
        else BotManager.Stop();
    }

    public override void OnUpdate()
    {
        if (_isGameReady && Input.GetKeyDown(BotSettings.ShortcutKey))
        {
            if (BotManager.IsRunning) BotManager.Stop();
            else BotManager.Start();
        }
    }
}
