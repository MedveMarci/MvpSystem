using System;
using LabApi.Features.Console;

namespace Mvp;

internal abstract class LogManager
{
    private static bool DebugEnabled => Mvp.Singleton?.Config?.Debug == true;

    public static void Debug(string message)
    {
        if (!DebugEnabled)
            return;

        Logger.Raw($"[DEBUG] [{Mvp.Singleton?.Name ?? "MvpSystem"}] {message}", ConsoleColor.Green);
    }

    public static void Info(string message, ConsoleColor color = ConsoleColor.Cyan)
    {
        Logger.Raw($"[INFO] [{Mvp.Singleton?.Name ?? "MvpSystemMvpSystem"}] {message}", color);
    }

    public static void Warn(string message)
    {
        Logger.Warn(message);
    }

    public static void Error(string message)
    {
        Logger.Raw(
            $"[ERROR] [{Mvp.Singleton?.Name ?? "MvpSystem"}] Details:\nVersion: {Mvp.Singleton?.Version.ToString() ?? "Unknown"}\n{message}",
            ConsoleColor.Red);
    }
}