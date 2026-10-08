using System;
using System.Linq;
using System.Runtime.CompilerServices;
using LabApi.Loader;
using MvpSystem.ApiFeatures;
using StatsSystem.Extensions;

namespace MvpSystem;

internal static class StatsSystem
{
    private static bool IsInstalled { get; } = PluginLoader.Plugins.Values.Concat(PluginLoader.Dependencies).Any(assembly => assembly.GetName().Name is "StatsSystem");

    internal static void TryIncrease(string userId)
    {
        if (!MvpSystem.Singleton.Config.StatsSystemIntegration) return;
        if (!IsInstalled)
        {
            LogManager.Debug("StatsSystem integration is enabled, but the StatsSystem plugin is not installed.");
            return;
        }

        try
        {
            IncrementStat(userId);
            LogManager.Debug($"Increased MVP count for player '{userId}' via StatsSystem.");
        }
        catch (Exception e)
        {
            LogManager.Error($"Failed to increase MVP stats for player '{userId}': {e}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void IncrementStat(string userId)
    {
        userId.IncrementStat("MVPs");
    }
}
