# MvpSystem

[![Version](https://img.shields.io/github/v/release/MedveMarci/MvpSystem?label=Version&color=d500ff)](https://github.com/MedveMarci/MvpSystem/releases/latest) [![LabAPI Version](https://img.shields.io/badge/LabAPI_Version-1.1.7-51f4ff)](https://github.com/northwood-studios/LabAPI/releases/tag/1.1.7) [![SCP:SL Version](https://img.shields.io/badge/SCP:SL_Version-14.2.7-e5b200)](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/) [![Downloads](https://img.shields.io/github/downloads/MedveMarci/MvpSystem/total?label=Downloads&color=ffbf00)](https://github.com/MedveMarci/MvpSystem/releases) [![License](https://img.shields.io/github/license/MedveMarci/MvpSystem?label=License&color=2ea44f)](LICENSE)

An SCP: Secret Laboratory [LabAPI](https://github.com/northwood-studios/LabAPI) plugin which shows some statistics about
the round and selects an MVP at the end of the round.

---

## Features

- Fully customizable in the config.
- The end of round summary can also be sent to a Discord channel through a webhook (`discord` section in the config).
- Players can mute the music for themselves in the Server-specific settings.
- Compatible with [StatsSystem](https://github.com/MedveMarci/StatsSystem/releases/latest) - if
  `StatsSystemIntegration` is `true` in the config and the plugin is present, it counts the MVPs.
- You can add MVP music for specific players using [SecretLabNAudio](https://github.com/Axwabo/SecretLabNAudio). If they
  are the MVP, the music is played. SecretLabNAudio is optional - without it the plugin works normally, just without
  music.
    - Supported formats: `.ogg`, `.mp3`, `.wav`, `.aiff`
    - Recommended: mono (1 channel), 48 kHz - other formats are converted automatically

---

## Installation

1. Download `MvpSystem.dll` and `dependencies.zip` from
   the [latest release](https://github.com/MedveMarci/MvpSystem/releases/latest).
2. Move `MvpSystem.dll` to:
    - Windows: `%AppData%\SCP Secret Laboratory\LabAPI\plugins\global\`
    - Linux: `~/.config/SCP Secret Laboratory/LabAPI/plugins/global/`
3. Extract `0Harmony.dll` from the `dependencies` folder of `dependencies.zip` to:
    - Windows: `%AppData%\SCP Secret Laboratory\LabAPI\dependencies\global\`
    - Linux: `~/.config/SCP Secret Laboratory/LabAPI/dependencies/global/`
4. Start the server once to generate the config and the music folder, then edit the config and restart.
5. Put the MVP music files into the `MvpMusic` folder and reference them by file name in the `mvp_music` config entry:
    - Windows: `%AppData%\SCP Secret Laboratory\LabAPI\configs\MvpMusic\`
    - Linux: `~/.config/SCP Secret Laboratory/LabAPI/configs/MvpMusic/`

---

## Support

<a href="https://discord.gg/KmpA8cfaSA"><img src="https://www.allkpop.com/upload/2021/01/content/262046/1611711962-discord-button.png" height="80"></a>

---

## Credits

- Original plugin made by TheRiptide
- Updated and maintained by [MedveMarci](https://github.com/MedveMarci)
