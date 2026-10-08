using System;
using System.Linq;
using System.Runtime.CompilerServices;
using LabApi.Loader;
using MvpSystem.ApiFeatures;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using UserSettings.ServerSpecific;

namespace MvpSystem;

internal static class MusicPlayer
{
    internal static bool IsAvailable { get; } = PluginLoader.Plugins.Values.Concat(PluginLoader.Dependencies).Any(assembly => assembly.GetName().Name is "SecretLabNAudio" or "SecretLabNAudio.Core");

    internal static bool TryPlay(string clipPath, float volume)
    {
        if (!IsAvailable)
            return false;
        try
        {
            Play(clipPath, volume);
            return true;
        }
        catch (Exception e)
        {
            LogManager.Error($"Failed to play MVP music '{clipPath}': {e}");
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Play(string clipPath, float volume)
    {
        SpeakerSettings settings = new()
        {
            IsSpatial = false, MaxDistance = 5000f, Volume = volume
        };
        AudioPlayerPool.Rent(settings).WithFilteredSendEngine(p => ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(p.ReferenceHub, 300)?.SyncIsA ?? false).UseFile(clipPath).DestroyOnEnd().PoolOnEnd();
    }
}