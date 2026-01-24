using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Achievements;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using Mirror;
using MvpSystem.ApiFeatures;
using PlayerRoles;
using PlayerStatsSystem;
using StatsSystem.Extensions;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace MvpSystem;

public static class EventHandler
{
    private static readonly Dictionary<Stat, Stats> PreviousStats = new();
    private static readonly Dictionary<int, Stats> PlayerStats = new();
    private static readonly Stopwatch Stopwatch = new();


    internal static void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        if (PlayerStats.ContainsKey(ev.Player.PlayerId))
            PlayerStats[ev.Player.PlayerId] = new Stats(ev.Player);
        else
            PlayerStats.Add(ev.Player.PlayerId, new Stats(ev.Player));

        var result = PreviousStats.FirstOrDefault(p => p.Value.UserId == ev.Player.UserId);
        result.Value?.Name = ev.Player.Nickname;
    }

    internal static void OnWaitingForPlayers()
    {
        ApiManager.CheckForUpdates();
        PlayerStats.Clear();
    }

    internal static void OnRoundStart()
    {
        Stopwatch.Restart();
        if (MvpSystem.Singleton.Config == null) return;
        foreach (var config in MvpSystem.Singleton.Config.MvpMusic.Where(config =>
                     File.Exists(Path.Combine(Path.Combine(PathManager.Configs.FullName, "MvpMusic"), config.Value))))
            AudioClipStorage.LoadClip(
                Path.Combine(Path.Combine(PathManager.Configs.FullName, "MvpMusic"), config.Value),
                config.Key + "-" + config.Value);
    }

    internal static void OnRoundRestarted()
    {
        if (MvpSystem.Singleton.Config == null) return;
        foreach (var config in MvpSystem.Singleton.Config.MvpMusic.Where(config =>
                     AudioClipStorage.AudioClips.ContainsKey(config.Key + "-" + config.Value)))
            AudioClipStorage.DestroyClip(config.Key + "-" + config.Value);
    }

    internal static void OnPlayerHurt(PlayerHurtEventArgs ev)
    {
        if (ev.Attacker == null) return;
        if (ev.DamageHandler is not AttackerDamageHandler damageHandler) return;
        var stats = PlayerStats[ev.Attacker.PlayerId];
        stats.TotalDamage += damageHandler.TotalDamageDealt;
    }

    internal static void OnPlayerDying(PlayerDyingEventArgs ev)
    {
        if (ev.Player.HasEffect<PocketCorroding>())
        {
            Player scp106 = null;
            foreach (var p in Player.ReadyList)
                if (p.Role == RoleTypeId.Scp106)
                    scp106 = p;
            if (scp106 != null)
            {
                var stats = PlayerStats[scp106.PlayerId];
                stats.KillsAsScp++;
                if (stats.ScpRole == RoleTypeId.None)
                    stats.ScpRole = RoleTypeId.Scp106;
            }
        }

        if (ev.Attacker == null) return;
        {
            var stats = PlayerStats[ev.Attacker.PlayerId];
            if (ev.Player.Team == Team.SCPs && ev.Player.Role != RoleTypeId.Scp0492)
            {
                stats.ScpsKilled.Add(ev.Player.Role);

                if (stats.ScpKilledTime < 0)
                    stats.ScpKilledTime = (float)NetworkTime.time;
            }

            if (ev.Attacker.Team != Team.SCPs &&
                ev.Attacker.Faction != ev.Player.Faction)
                stats.TotalKills++;
        }
    }

    internal static void OnPlayerDeath(PlayerDeathEventArgs ev)
    {
        if (ev.Attacker == null) return;
        var stats = PlayerStats[ev.Attacker.PlayerId];
        if (ev.Attacker.Team != Team.SCPs) return;
        stats.KillsAsScp++;
        if (stats.ScpRole == RoleTypeId.None)
            stats.ScpRole = ev.Attacker.Role;
    }

    internal static void OnPlayerEscaping(PlayerEscapingEventArgs ev)
    {
        if (!ev.IsAllowed)
            return;
        var stats = PlayerStats[ev.Player.PlayerId];
        stats.EscapeTime = (float)Stopwatch.Elapsed.TotalSeconds;
        stats.EscapeRole = ev.Player.Role;
    }

    internal static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        LogManager.Debug($"OnRoundEnded called. PlayerStats count: {PlayerStats.Count}");
        try
        {
            LogManager.Debug("OnRoundEnded: starting stats aggregation");
            Stats topKillsAsScp = null;
            Stats topScpsKilled = null;
            Stats topScpKilledTime = null;
            Stats topTotalKills = null;
            Stats topEscapeTime = null;
            Stats topAchievement = null;
            Stats topTotalDamageDealt = null;
            foreach (var s in PlayerStats.Values)
            {
                LogManager.Debug(
                    $"Evaluating player {s.Name} ({s.UserId}): KillsAsScp={s.KillsAsScp}, ScpsKilled={s.ScpsKilled.Count}, ScpKilledTime={s.ScpKilledTime}, TotalKills={s.TotalKills}, EscapeTime={s.EscapeTime}, Achievement={s.Achievement}, TotalDamage={s.TotalDamage}");

                if (s.KillsAsScp != 0 && (topKillsAsScp == null || s.KillsAsScp > topKillsAsScp.KillsAsScp))
                {
                    topKillsAsScp = s;
                    LogManager.Debug($"New topKillsAsScp: {s.Name} ({s.KillsAsScp})");
                }

                if (!s.ScpsKilled.IsEmpty() &&
                    (topScpsKilled == null || s.ScpsKilled.Count > topScpsKilled.ScpsKilled.Count))
                {
                    topScpsKilled = s;
                    LogManager.Debug($"New topScpsKilled: {s.Name} (count={s.ScpsKilled.Count})");
                }

                if (s.ScpKilledTime > 0 &&
                    (topScpKilledTime == null || s.ScpKilledTime < topScpKilledTime.ScpKilledTime))
                {
                    topScpKilledTime = s;
                    LogManager.Debug($"New topScpKilledTime: {s.Name} (time={s.ScpKilledTime})");
                }

                if (s.TotalKills != 0 && (topTotalKills == null || s.TotalKills > topTotalKills.TotalKills))
                {
                    topTotalKills = s;
                    LogManager.Debug($"New topTotalKills: {s.Name} (kills={s.TotalKills})");
                }

                if (s.EscapeTime > 0 && (topEscapeTime == null || s.EscapeTime < topEscapeTime.EscapeTime))
                {
                    topEscapeTime = s;
                    LogManager.Debug($"New topEscapeTime: {s.Name} (escapeTime={s.EscapeTime})");
                }

                if (s.Achievement != null && (topAchievement == null ||
                                              MvpSystem.Singleton.Config.Achievements.IndexOf(s.Achievement.Value) <
                                              MvpSystem.Singleton.Config.Achievements.IndexOf(topAchievement.Achievement
                                                  .Value)))
                {
                    topAchievement = s;
                    LogManager.Debug($"New topAchievement: {s.Name} (achievement={s.Achievement})");
                }

                if (s.TotalDamage != 0 &&
                    (topTotalDamageDealt == null || s.TotalDamage > topTotalDamageDealt.TotalDamage))
                {
                    topTotalDamageDealt = s;
                    LogManager.Debug($"New topTotalDamageDealt: {s.Name} (damage={s.TotalDamage})");
                }
            }

            LogManager.Debug(
                $"Stats aggregation finished. topKillsAsScp={topKillsAsScp?.Name ?? "<none>"}, topScpsKilled={topScpsKilled?.Name ?? "<none>"}, topScpKilledTime={topScpKilledTime?.Name ?? "<none>"}, topTotalKills={topTotalKills?.Name ?? "<none>"}, topEscapeTime={topEscapeTime?.Name ?? "<none>"}, topAchievement={topAchievement?.Name ?? "<none>"}, topTotalDamageDealt={topTotalDamageDealt?.Name ?? "<none>"}");

            var bc = MvpSystem.Singleton.Config.Start;
            var mvp = new[]
                {
                    topTotalKills, topScpsKilled, topKillsAsScp, topTotalDamageDealt, topEscapeTime, topAchievement,
                    topScpKilledTime
                }
                .Where(s => s != null)
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            LogManager.Debug($"MVP selection result: {(mvp != null ? mvp.Name + " (" + mvp.UserId + ")" : "<none>")}");

            if (mvp != null)
            {
                if (MvpSystem.Singleton.Config.StatsSystemIntegration)
                {
                    var mvpPlayer = Player.Get(mvp.UserId);
                    if (mvpPlayer != null)
                    {
                        LogManager.Debug(
                            $"Incrementing MVP count for player {mvp.Name} ({mvp.UserId}) via StatsSystem");
                        if (LabApi.Loader.PluginLoader.EnabledPlugins.Any(plugin => plugin.Name == "StatsSystem"))
                        {
                            IncrementStat(mvpPlayer);
                            LogManager.Debug("MVP count incremented successfully.");
                        }
                        else
                            LogManager.Warn(
                                "StatsSystem plugin not found in enabled plugins. Cannot increment MVP count.");
                        
                    }
                    else
                    {
                        LogManager.Debug($"MVP player object not found for userId {mvp.UserId}");
                    }
                }

                var mvpText = MvpSystem.Singleton.Config.MvpTitle.Replace("{name}", mvp.Name);
                if (mvp != null && MvpSystem.Singleton.Config.MvpMusic.ContainsKey(mvp.UserId) &&
                    MvpSystem.Singleton.Config.MvpMusic[mvp.UserId] != null)
                {
                    LogManager.Debug(
                        $"MVP has configured music: {MvpSystem.Singleton.Config.MvpMusic[mvp.UserId]} for user {mvp.UserId}");
                    var audioPlayer = AudioPlayer.CreateOrGet("MvpPlayer",
                        condition: hub =>
                            ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(hub, 300).SyncIsA,
                        onIntialCreation: p => { p.AddSpeaker("MvpSpeaker", isSpatial: false, maxDistance: 5000f); });

                    LogManager.Debug("AudioPlayer obtained/created for MVP audio");

                    audioPlayer.AddClip(mvp.UserId + "-" + MvpSystem.Singleton.Config.MvpMusic[mvp.UserId]);
                    LogManager.Debug(
                        $"Added audio clip: {mvp.UserId}-{MvpSystem.Singleton.Config.MvpMusic[mvp.UserId]}");

                    audioPlayer.DestroyWhenAllClipsPlayed = true;
                    LogManager.Debug("AudioPlayer configured to destroy when clips played");
                    mvpText +=
                        $"\nZene neve: <b>{MvpSystem.Singleton.Config.MvpMusic[mvp.UserId].Replace(".ogg", "")}</b>";
                }

                bc += mvpText + "\n";
                LogManager.Debug($"MVP text appended to broadcast: {mvpText}");
            }

            if (topKillsAsScp != null && !string.IsNullOrEmpty(MvpSystem.Singleton.Config.MostKillsAsScp))
            {
                var line = MvpSystem.Singleton.Config.MostKillsAsScp.Replace("{name}", topKillsAsScp.Name)
                    .Replace("{role}",
                        topKillsAsScp.ScpRole == RoleTypeId.Tutorial
                            ? "<color=#FF96DE>Serpent's Hand</color>"
                            : topKillsAsScp.ScpRole.ToString())
                    .Replace("{kills}", topKillsAsScp.KillsAsScp.ToString()) + "\n";
                bc += line;
                LogManager.Debug($"Appended MostKillsAsScp line: {line}");
            }

            if (topScpsKilled != null && !(string.IsNullOrEmpty(MvpSystem.Singleton.Config.FirstToKillScp) &&
                                           string.IsNullOrEmpty(MvpSystem.Singleton.Config.MostScpsKilled)))
            {
                if ((topScpsKilled.ScpsKilled.Count == 1 ||
                     string.IsNullOrEmpty(MvpSystem.Singleton.Config.MostScpsKilled)) &&
                    topScpKilledTime != null && !string.IsNullOrEmpty(MvpSystem.Singleton.Config.FirstToKillScp))
                {
                    bc += MvpSystem.Singleton.Config.FirstToKillScp.Replace("{name}", topScpKilledTime.Name) + "\n";
                }
                else if (!string.IsNullOrEmpty(MvpSystem.Singleton.Config.MostScpsKilled))
                {
                    var roles = topScpsKilled.ScpsKilled.Select(scp =>
                            MvpSystem.Singleton.Config?.MostScpsKilledListItem.Replace("{scp}",
                                scp == RoleTypeId.Tutorial ? "<color=#FF96DE>Serpent's Hand</color>" : scp.ToString()))
                        .ToList();
                    bc += MvpSystem.Singleton.Config.MostScpsKilled.Replace("{name}", topScpsKilled.Name)
                        .Replace("{scps}", string.Join(", ", roles)) + "\n";
                }
            }

            if (topTotalKills != null && !string.IsNullOrEmpty(MvpSystem.Singleton.Config.MostKillsAsHuman))
                bc += MvpSystem.Singleton.Config.MostKillsAsHuman.Replace("{name}", topTotalKills.Name)
                    .Replace("{kills}", topTotalKills.TotalKills.ToString()) + "\n";

            if (topTotalDamageDealt != null && !string.IsNullOrEmpty(MvpSystem.Singleton.Config.MostDamageDealt))
                bc += MvpSystem.Singleton.Config.MostDamageDealt.Replace("{name}", topTotalDamageDealt.Name)
                    .Replace("{damage}", Mathf.RoundToInt(topTotalDamageDealt.TotalDamage).ToString()) + "\n";

            if (topEscapeTime != null && !string.IsNullOrEmpty(MvpSystem.Singleton.Config.FirstToEscape))
            {
                var ts = new TimeSpan(0, 0, Mathf.RoundToInt(topEscapeTime.EscapeTime));
                bc += MvpSystem.Singleton.Config.FirstToEscape.Replace("{name}", topEscapeTime.Name)
                    .Replace("{time}", ts.Minutes + ":" + ts.Seconds.ToString("D2")).Replace("{role}",
                        (topEscapeTime.EscapeRole == RoleTypeId.ClassD ? "<color=#ff731c>" : "<color=#fff287>") +
                        topEscapeTime.EscapeRole + "</color>") + "\n";
            }

            if (topAchievement != null && !string.IsNullOrEmpty(MvpSystem.Singleton.Config.BestAchievement))
                bc += MvpSystem.Singleton.Config.BestAchievement.Replace("{name}", topAchievement.Name)
                    .Replace("{achievement}",
                        MvpSystem.Singleton.Config.AchievementNames.TryGetValue(topAchievement.Achievement.Value,
                            out var name)
                            ? name
                            : $"<color=#FF0000>ERROR ACHIEVEMENT NAME TRANSLATION MISSING FOR {topAchievement.Achievement.Value}</color>")
                    .Replace("{description}",
                        MvpSystem.Singleton.Config.AchievementDescriptions.TryGetValue(topAchievement.Achievement.Value,
                            out var description)
                            ? description
                            : $"<color=#FF0000>ERROR ACHIEVEMENT DESCRIPTION TRANSLATION MISSING FOR {topAchievement.Achievement.Value}</color>");
            bc += MvpSystem.Singleton.Config.End;
            LogManager.Debug($"Final broadcast length: {bc.Length}");
            var preview = bc.Length > 200 ? bc.Substring(0, 200) + "..." : bc;
            LogManager.Debug($"Broadcast preview: {preview}");
            foreach (var p in Player.ReadyList)
            {
                LogManager.Debug($"Sending broadcast to player {p.Nickname} ({p.UserId})");
                p.SendBroadcast(bc, MvpSystem.Singleton.Config.Duration, shouldClearPrevious: true);
            }

            //command events
            PreviousStats.Clear();
            LogManager.Debug("PreviousStats cleared");
            if (topKillsAsScp != null)
            {
                PreviousStats[Stat.MostKillsAsScp] = topKillsAsScp;
                LogManager.Debug($"PreviousStats set MostKillsAsScp -> {topKillsAsScp.Name}");
            }

            if (topScpsKilled != null)
            {
                if (topScpsKilled.ScpsKilled.Count == 1 && topScpKilledTime != null)
                {
                    PreviousStats[Stat.FirstToKillScp] = topScpKilledTime;
                    LogManager.Debug($"PreviousStats set FirstToKillScp -> {topScpKilledTime.Name}");
                }
                else
                {
                    PreviousStats[Stat.MostScpsKilled] = topScpsKilled;
                    LogManager.Debug($"PreviousStats set MostScpsKilled -> {topScpsKilled.Name}");
                }
            }

            if (topTotalKills != null)
            {
                PreviousStats[Stat.MostKillsAsHuman] = topTotalKills;
                LogManager.Debug($"PreviousStats set MostKillsAsHuman -> {topTotalKills.Name}");
            }

            if (topEscapeTime != null)
            {
                PreviousStats[Stat.FirstToEscape] = topEscapeTime;
                LogManager.Debug($"PreviousStats set FirstToEscape -> {topEscapeTime.Name}");
            }

            if (topAchievement != null)
            {
                PreviousStats[Stat.BestAchievement] = topAchievement;
                LogManager.Debug($"PreviousStats set BestAchievement -> {topAchievement.Name}");
            }

            if (topTotalDamageDealt != null)
            {
                PreviousStats[Stat.MostDamageDealt] = topTotalDamageDealt;
                LogManager.Debug($"PreviousStats set MostDamageDealt -> {topTotalDamageDealt.Name}");
            }

            LogManager.Debug($"OnRoundEnded completed. PreviousStats keys: {string.Join(", ", PreviousStats.Keys)}");
        }
        catch (Exception e)
        {
            LogManager.Error($"Error during round end processing: {e}");
        }
    }

    public static void OnPlayerAchieve(Player player, AchievementName achievement)
    {
        if (MvpSystem.Singleton.Config != null &&
            !MvpSystem.Singleton.Config.Achievements.Contains(achievement)) return;
        var stats = PlayerStats[player.PlayerId];
        if (MvpSystem.Singleton.Config != null && (stats.Achievement == null ||
                                                   MvpSystem.Singleton.Config.Achievements.IndexOf(stats.Achievement
                                                       .Value) >
                                                   MvpSystem.Singleton.Config.Achievements.IndexOf(achievement)))
            stats.Achievement = achievement;
    }

    private static void IncrementStat(Player player)
    {
        player.IncrementStat("MVPs");
    }

    private class Stats(Player player)
    {
        public readonly List<RoleTypeId> ScpsKilled = [];
        public readonly string UserId = player.UserId;
        public AchievementName? Achievement;
        public RoleTypeId EscapeRole;
        public float EscapeTime = -1;
        public int KillsAsScp;
        public string Name = player.Nickname;
        public float ScpKilledTime = -1;
        public RoleTypeId ScpRole = RoleTypeId.None;
        public float TotalDamage;
        public int TotalKills;
    }

    private enum Stat
    {
        MostKillsAsScp,
        MostDamageDealt,
        FirstToKillScp,
        MostScpsKilled,
        MostKillsAsHuman,
        FirstToEscape,
        BestAchievement
    }
}