using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using MvpSystem.ApiFeatures;
using UserSettings.ServerSpecific;

namespace MvpSystem;

public class MvpSystem : Plugin<Config>
{
    private readonly Harmony _harmony = new("MedveMarci.MVP");

    public override string Name => "MvpSystem";

    public override string Description => "A plugin to track and reward MVP players each round.";

    public override string Author => "MedveMarci";

    public override Version Version { get; } = new(1, 3, 0);

    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    internal static MvpSystem Singleton { get; private set; }

    public override void Enable()
    {
        Singleton = this;
        _harmony.PatchAll();

        if (!Directory.Exists(Path.Combine(PathManager.Configs.FullName, "MvpMusic")))
        {
            LogManager.Info("MvpMusic directory does not exist. Creating...");
            Directory.CreateDirectory(Path.Combine(PathManager.Configs.FullName, "MvpMusic"));
        }

        if (MusicPlayer.IsAvailable)
            RegisterMusicSetting();

        PlayerEvents.Joined += EventHandler.OnPlayerJoined;
        ServerEvents.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
        ServerEvents.RoundStarted += EventHandler.OnRoundStart;
        PlayerEvents.Dying += EventHandler.OnPlayerDying;
        PlayerEvents.Death += EventHandler.OnPlayerDeath;
        PlayerEvents.Escaped += EventHandler.OnPlayerEscaped;
        ServerEvents.RoundEnded += EventHandler.OnRoundEnded;
        PlayerEvents.Hurt += EventHandler.OnPlayerHurt;
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
        PlayerEvents.Escaped -= EventHandler.OnPlayerEscaped;
        ServerEvents.RoundEnded -= EventHandler.OnRoundEnded;
        PlayerEvents.Hurt -= EventHandler.OnPlayerHurt;
    }

    private void RegisterMusicSetting()
    {
        MusicSettingTexts texts = Config.MusicSetting;
        ServerSpecificSettingBase[] setting =
        [
            new SSGroupHeader(texts.Header),
            new SSTwoButtonsSetting(300, texts.Label, texts.OnText, texts.OffText, false, texts.Hint)
        ];

        if (ServerSpecificSettingsSync.DefinedSettings == null || ServerSpecificSettingsSync.DefinedSettings.Length == 0)
        {
            ServerSpecificSettingsSync.DefinedSettings = setting;
        }
        else
        {
            List<ServerSpecificSettingBase> newSettings =
            [
                .. ServerSpecificSettingsSync.DefinedSettings,
                .. setting
            ];
            ServerSpecificSettingsSync.DefinedSettings = [.. newSettings];
        }

        ServerSpecificSettingsSync.SendToAll();
    }
}