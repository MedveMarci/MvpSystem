using System.Collections.Generic;
using System.ComponentModel;
using Achievements;

namespace MvpSystem;

public class Config
{
    [Description("Enable debug logs")]
    public bool Debug { get; set; } = false;
    
    [Description("MVP music to be played with steamid as key and music file name as value, make sure the files are in Mvp/Music folder")]
    public Dictionary<string, string> MvpMusic { get; set; } = new()
    {
        { "steamid", "name.ogg" },
        { "steamid2", "name.ogg" }
    };

    [Description("If the StatsSystem plugin is present, it will count it if this is true")]
    public bool StatsSystemIntegration { get; set; } = true;
    
    [Description("Duration of broadcast, might need to be increased if round end time set in config is longer")]
    public ushort Duration { get; set; } = 30;

    [Description("Set text format shared between stats with tags and display text you want to appear at the top")]
    public string Start { get; set; } = "<size=31><line-height=0.9em>";
    
    [Description("Format used for MVP title")]
    public string MvpTitle { get; set; } =  "<color=#78e2ff><b>{name}</b></color> is the MVP!";

    [Description("Format used for each stat, if set to no text/empty the stat will not be displayed")]
    public string MostKillsAsScp { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> had the most kills as <color=#ff0000><b>{role}</b></color> with <color=#45ff7a><b>{kills}</b></color> kills";

    public string FirstToKillScp { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> was the first to kill a <color=#ff0000><b>SCP</b></color>";

    [Description("Replaces first_to_kill_scp if player has more than one kill")]
    public string MostScpsKilled { get; set; } = "<color=#78e2ff><b>{name}</b></color> killed {scps}";

    public string MostScpsKilledListItem { get; set; } = "<color=#ff0000><b>{scp}</b></color>";

    public string MostKillsAsHuman { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> had the most kills as a human with <color=#45ff7a><b>{kills}</b></color> kills";

    public string FirstToEscape { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> was the first to escape in <color=#45ff7a><b>{time}</b></color> as a <b>{role}</b>";

    public string BestAchievement { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> achieved <color=#45ff7a><b>{achievement}</b></color> - {description}";
    
    public string MostDamageDealt { get; set; } = 
        "<color=#78e2ff><b>{name}</b></color> dealt the most damage with <color=#45ff7a><b>{damage}</b></color> damage";

    [Description("Close out tags from the Start and display text you want to appear at the bottom")]
    public string End { get; set; } = "</line-height></size>";

    [Description(
        "Achievements to be tracked during the round. Order of achievements determine priority, ones closer to the top override lower ones. contains all valid achievements by default(christmas, halloween and They Are Just Resources... are missing due to techinal reasons)")]
    public List<AchievementName> Achievements { get; set; } =
    [
        AchievementName.BePoliteBeEfficient,
        AchievementName.ChangeInCommand,
        AchievementName.Overcurrent,
        AchievementName.Escape207,
        AchievementName.TurnThemAll,
        AchievementName.AccessGranted,
        AchievementName.CrisisAverted,
        AchievementName.DidntEvenFeelThat,
        AchievementName.MicrowaveMeal,
        AchievementName.EscapeArtist,
        AchievementName.Pacified,
        AchievementName.IllPassThanks,
        AchievementName.ForScience,
        AchievementName.FireInTheHole,
        AchievementName.ItsAlwaysLeft,
        AchievementName.SecureContainProtect,
        AchievementName.IsThisThingOn,
        AchievementName.WalkItOff,
        AchievementName.Friendship,
        AchievementName.HeWillBeBack,
        AchievementName.ExecutiveAccess,
        AchievementName.AnomalouslyEfficient,
        AchievementName.MelancholyOfDecay,
        AchievementName.ThatCanBeUseful,
        AchievementName.TMinus,
        AchievementName.ProceedWithCaution,
        AchievementName.DontBlink,
        AchievementName.DeltaCommand,
        AchievementName.LightsOut
    ];

    public Dictionary<AchievementName, string> AchievementDescriptions { get; set; } = new()
    {
        { AchievementName.LightsOut, "Respawned as Nine-Tailed Fox" },
        { AchievementName.DeltaCommand, "Respawned as Chaos Insurgency" },
        { AchievementName.DontBlink, "Successfully evaded <color=#ff0000>SCP-173</color>" },
        {
            AchievementName.ProceedWithCaution,
            "Successfully passed through a Tesla gate that <color=#ff0000>SCP-079</color> was watching"
        },
        { AchievementName.TMinus, "Survived a successful Alpha Warhead Detonation" },
        { AchievementName.ThatCanBeUseful, "Find any gun as a <color=#FF8000>Class-D</color>" },
        {
            AchievementName.MelancholyOfDecay,
            "Captured a player within five seconds of emerging from the ground as <color=#ff0000>SCP-106</color>"
        },
        {
            AchievementName.AnomalouslyEfficient,
            "Killed a player in the first minute of the game as an <color=#ff0000>SCP</color>"
        },
        { AchievementName.ExecutiveAccess, "Obtained a max-level keycard" },
        { AchievementName.HeWillBeBack, "Successfully escaped from the Pocket Dimension" },
        {
            AchievementName.Friendship,
            "As a <color=#FFFF7C>Scientist</color>, successfully upgraded their keycard alongside <color=#FF8000>Class-D's</color>"
        },
        { AchievementName.WalkItOff, "Survived a fall with less than half of their health remaining" },
        { AchievementName.IsThisThingOn, "Broadcasted a 'helpful' message via the Intercom" },
        { AchievementName.SecureContainProtect, "killed the final <color=#ff0000>SCP</color> in the round as a MTF" },
        { AchievementName.ItsAlwaysLeft, "Escaped as <color=#FF8000>Class-D</color> personnel" },
        { AchievementName.FireInTheHole, "Killed an enemy using a grenade" },
        { AchievementName.ForScience, "Escaped as a <color=#FFFF7C>Scientist</color>" },
        {
            AchievementName.IllPassThanks,
            "Killed someone who was actively using the Micro H.I.D as an <color=#ff0000>SCP</color>"
        },
        { AchievementName.Pacified, "Killed <color=#ff0000>SCP-096</color> while it was entering its rage" },
        { AchievementName.EscapeArtist, "Was the first to escape the Facility" },
        { AchievementName.MicrowaveMeal, "Killed an <color=#ff0000>SCP</color> with the Micro H.I.D" },
        { AchievementName.DidntEvenFeelThat, "Used adrenaline to survive a hit that would otherwise killed them" },
        { AchievementName.CrisisAverted, "Used SCP-500 when they were about to die" },
        {
            AchievementName.AccessGranted,
            "Killed a <color=#FFFF7C>Scientist</color> holding a keycard as a <color=#FF8000>Class-D</color>"
        },
        { AchievementName.TurnThemAll, "Cured ten people as <color=#ff0000>SCP-049</color>" },
        { AchievementName.Escape207, "Escaped while under the effects of SCP-207" },
        { AchievementName.SomethingDoneRight, "Killed an SCP as a <color=#FFFF7C>Scientist</color>" },
        {
            AchievementName.PropertyOfChaos,
            "Escaped with more than two SCP objects, as a <color=#FF8000>Class-D</color>"
        },
        { AchievementName.ThatWasClose, "Canceled the Alpha Warhead detonation in the last 15 seconds" },
        { AchievementName.Overcurrent, "Tried to recharge the Micro H.I.D" },
        { AchievementName.ChangeInCommand, "Disarmed an MTF operative" },
        { AchievementName.BePoliteBeEfficient, "Killed five enemies in less than 30 seconds" }
    };

    public Dictionary<AchievementName, string> AchievementNames { get; set; } = new()
    {
        { AchievementName.LightsOut, "Lights Out" },
        { AchievementName.DeltaCommand, "We of Delta Command..." },
        { AchievementName.DontBlink, "Don’t Blink" },
        { AchievementName.ProceedWithCaution, "Proceed With Caution" },
        { AchievementName.TMinus, "T-Minus 90 seconds..." },
        { AchievementName.ThatCanBeUseful, "... You Thinking What I'm Thinking?" },
        { AchievementName.MelancholyOfDecay, "Melancholy of Decay" },
        { AchievementName.AnomalouslyEfficient, "Anomalously Efficient" },
        { AchievementName.ExecutiveAccess, "Executive Access" },
        { AchievementName.HeWillBeBack, "He’ll Be Back..." },
        { AchievementName.Friendship, "Friendship" },
        { AchievementName.WalkItOff, "Walk It Off" },
        { AchievementName.IsThisThingOn, "Is This Thing On?" },
        { AchievementName.SecureContainProtect, "Secure. Contain. Protect." },
        { AchievementName.ItsAlwaysLeft, "It's Always Left, Brothers!" },
        { AchievementName.FireInTheHole, "Fire In The Hole!" },
        { AchievementName.ForScience, "For Science!" },
        { AchievementName.IllPassThanks, "I'll Pass, Thanks" },
        { AchievementName.Pacified, "Pacified" },
        { AchievementName.EscapeArtist, "Escape Artist" },
        { AchievementName.MicrowaveMeal, "Microwave Meal" },
        { AchievementName.DidntEvenFeelThat, "Ha! I didn't even feel that!" },
        { AchievementName.CrisisAverted, "Crisis Averted" },
        { AchievementName.AccessGranted, "Access Granted" },
        { AchievementName.TurnThemAll, "My Cure Is Most Effective..." },
        { AchievementName.Escape207, "High on the Wings of Caffeine" },
        { AchievementName.SomethingDoneRight, "If you want something done right..." },
        { AchievementName.PropertyOfChaos, "Property of the Chaos Insurgency" },
        { AchievementName.ThatWasClose, "That was... close." },
        { AchievementName.Overcurrent, "Overcurrent" },
        { AchievementName.ChangeInCommand, "Change in Command" },
        { AchievementName.BePoliteBeEfficient, "Be Polite. Be Efficient." }
    };
}