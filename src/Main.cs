using System.Reflection;
using Firebot.BotActions;
using Firebot.Core;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;
using Main = Firebot.Main;

[assembly: MelonInfo(typeof(Main), "Firebot", "0.2.7-alpha.1", "danilogmoura", "https://github.com/danilogmoura/firebot")]
[assembly: MelonGame]

[assembly: MelonColor(255, 255, 0, 255)]
[assembly: MelonAuthorColor(255, 0, 255, 0)]

[assembly: AssemblyTitle("Firebot")]
[assembly:
    AssemblyDescription("A bot for automating tasks using MelonLoader.")]
[assembly: AssemblyCopyright("Created by danilogmoura")]

namespace Firebot;

public class Main : MelonMod
{
    private bool _isGameReady;

    public override void OnInitializeMelon()
    {
        BotSettings.Initialize();
        BotManager.Initialize();
        AutoSkill.Initialize();
        AutoUpgrade.Initialize();
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

        if (_isGameReady && Input.GetKeyDown(KeyCode.F9))
            DumpHudHierarchy();

        if (_isGameReady && Input.GetKeyDown(KeyCode.F10))
            DumpSafeAreaHierarchy();
    }

    // Temporary diagnostic: dumps the live battle HUD hierarchy to the log so broken/renamed
    // UI paths can be identified without a separate hierarchy-inspection tool.
    private static void DumpHudHierarchy()
    {
        var target = GameObject.Find("battleRoot")
            ?.transform.Find("battleMain/battleCanvas/SafeArea/bottomSideUINew");
        if (target == null)
        {
            Logger.Info("[HUD DUMP] bottomSideUINew not found.");
            return;
        }

        Logger.Info("[HUD DUMP] bottomSideUINew children (name, active/activeInHierarchy):");
        DumpChildren(target, 0, 8);
    }

    // Temporary diagnostic: dumps the whole battle SafeArea (every top-level HUD region, including
    // ones not mapped in Paths.cs yet, like the top-center stage bar) so new UI can be located
    // without a separate hierarchy-inspection tool.
    private static void DumpSafeAreaHierarchy()
    {
        var target = GameObject.Find("battleRoot")?.transform.Find("battleMain/battleCanvas/SafeArea");
        if (target == null)
        {
            Logger.Info("[SAFEAREA DUMP] SafeArea not found.");
            return;
        }

        Logger.Info("[SAFEAREA DUMP] SafeArea children (name, active/activeInHierarchy):");
        DumpChildren(target, 0, 6);
    }

    private static void DumpChildren(Transform parent, int depth, int maxDepth)
    {
        if (depth > maxDepth) return;

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            Logger.Info(
                $"[HUD DUMP] {new string(' ', depth * 2)}{child.name} (active={child.gameObject.activeSelf}/{child.gameObject.activeInHierarchy})");
            DumpChildren(child, depth + 1, maxDepth);
        }
    }
}