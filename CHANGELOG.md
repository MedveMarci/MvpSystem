## [1.2.0]

### Added

- **Configurable music volume** (`MusicVolume`). Controls the volume of the MVP
  music clip (`100` = normal, lower is quieter, higher amplifies and may distort).
- **Score-based MVP selection** (`Scoring` config section). The MVP is now the
  player with the highest score earned from what they actually did during the
  round, with fully configurable point weights:
  - `MinimumScore`, `PerHumanKill`, `PerScpKill`, `PerScpKilled`, `PerDamage`,
    `Escape`, `FirstEscapeBonus`, `FirstScpKillBonus`, `AchievementBase`,
    `AchievementPriorityBonus`.

### Changed

- MVP selection no longer picks the player who simply tops the most stat
  categories (which could crown someone who did almost nothing). The title now
  goes to the highest scorer, and is **not awarded at all** if nobody reaches
  `MinimumScore`.
- Damage and SCP kills from friendly fire (same faction/team) no longer count
  toward MVP scoring.

### Fixed

- **Escape sometimes displayed an SCP role.** The escaped role now uses the
  event's `OldRole` (the role the player escaped as) instead of the player's
  live role, which could already reflect the post-escape role transition.
- `EscapeRole` defaulted to `Scp173` (the `RoleTypeId` zero value); it now
  defaults to `None`.
- Several event handlers could throw `KeyNotFoundException` when stats were
  accessed for untracked players (dummies/NPCs). Stat access is now routed
  through a safe `GetStats` helper.

## [1.1.0]

- Refactored MVP music playback to use `SecretLabNAudio.Core`.

## [1.0.0]

- Initial release.

[1.2.0]: https://github.com/MedveMarci/MvpSystem/releases/tag/1.2.0
[1.1.0]: https://github.com/MedveMarci/MvpSystem/releases/tag/1.1.0
[1.0.0]: https://github.com/MedveMarci/MvpSystem/releases/tag/1.0.0
