using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using UserSettings.ServerSpecific;

namespace MvpSystem;

public class Mvp : Plugin<Config>
{
    private readonly Harmony _harmony = new("MedveMarci.MVP");
    public override string Name => "MvpSystem";
    public override string Description => "A plugin to track and reward MVP players each round.";
    public override string Author => "MedveMarci";
    public override Version Version { get; } = new(1, 0, 0);
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
    internal static Mvp Singleton { get; private set; }
    public string githubRepo = "MedveMarci/MvpSystem";

    public override void Enable()
    {
        Singleton = this;
        _harmony.PatchAll();
        if (!Directory.Exists(Path.Combine(PathManager.Configs.FullName, "MvpMusic")))
        {
            LogManager.Info("MvpMusic directory does not exist. Creating...");
            Directory.CreateDirectory(Path.Combine(PathManager.Configs.FullName, "MvpMusic"));
        }

        ServerSpecificSettingBase[] setting =
        [
            new SSGroupHeader("MvpMusic"),
            new SSTwoButtonsSetting(300, "MVP Music", "On", "Off", false, "You can enable or disable MVP music sound."),
        ];

        if (ServerSpecificSettingsSync.DefinedSettings == null ||
            ServerSpecificSettingsSync.DefinedSettings.Length == 0)
        {
            ServerSpecificSettingsSync.DefinedSettings = setting;
        }
        else
        {
            var newSettings = new List<ServerSpecificSettingBase>(ServerSpecificSettingsSync.DefinedSettings);
            newSettings.AddRange(setting);
            ServerSpecificSettingsSync.DefinedSettings = newSettings.ToArray();
        }

        ServerSpecificSettingsSync.SendToAll();
        PlayerEvents.Joined += EventHandler.OnPlayerJoined;
        ServerEvents.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
        ServerEvents.RoundStarted += EventHandler.OnRoundStart;
        PlayerEvents.Dying += EventHandler.OnPlayerDying;
        PlayerEvents.Death += EventHandler.OnPlayerDeath;
        PlayerEvents.Escaping += EventHandler.OnPlayerEscaping;
        ServerEvents.RoundEnded += EventHandler.OnRoundEnded;
        ServerEvents.RoundRestarted += EventHandler.OnRoundRestarted;
    }

    public override void Disable()
    {
        _harmony.UnpatchAll("MedveMarci.MVP");
        Singleton = null;
        PlayerEvents.Joined -= EventHandler.OnPlayerJoined;
        ServerEvents.WaitingForPlayers -= EventHandler.OnWaitingForPlayers;
        ServerEvents.RoundStarted -= EventHandler.OnRoundStart;
        PlayerEvents.Dying -= EventHandler.OnPlayerDying;
        PlayerEvents.Death -= EventHandler.OnPlayerDeath;
        PlayerEvents.Escaping -= EventHandler.OnPlayerEscaping;
        ServerEvents.RoundEnded -= EventHandler.OnRoundEnded;
        ServerEvents.RoundRestarted -= EventHandler.OnRoundRestarted;
    }
}