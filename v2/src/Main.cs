using Firebot2.Core;
using MelonLoader;
using UnityEngine;
using Logger = Firebot2.Core.Logger;
using Main = Firebot2.Main;

[assembly: MelonInfo(typeof(Main), "Firebot2", "0.1.0", "davide-mariotti", "https://github.com/davide-mariotti/firestoneBot")]
[assembly: MelonGame]

[assembly: MelonColor(255, 255, 0, 255)]
[assembly: MelonAuthorColor(255, 0, 255, 0)]

namespace Firebot2;

/// <summary>
///     Rewrite of Firebot, meant to eventually replace it (see v2/PLAN.md). Not meant to run
///     alongside the original mod on the same account - both would click the same things twice.
/// </summary>
public class Main : MelonMod
{
    private bool _isGameReady;

    public override void OnInitializeMelon()
    {
        BotSettings.Initialize();
        BotManager.Initialize();

        Logger.Info("Firebot2 System Initialized.");
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
