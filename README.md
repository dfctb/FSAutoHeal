<img width="1280" height="640" alt="Untitled-1" src="https://github.com/user-attachments/assets/2a780f04-d1c9-4695-9d31-8156cdecd5c6" />

# FS AutoHeal

A BepInEx plugin for the Steam version of **Fallout Shelter** that automatically uses
Stimpaks and RadAway on your dwellers, so you don't have to tap them one by one during
raids and other incidents.

Unofficial fan-made mod. Not affiliated with or endorsed by Bethesda.

## Features
- Auto-uses a Stimpak when a dweller's health drops to a configurable percent
- Auto-uses RadAway when a dweller's radiation reaches a configurable percent
- Hotkey to turn auto-healing on and off in game
- Everything is configurable in a config file

## Requirements
- Fallout Shelter (Steam, Windows)
- [BepInEx 5.4.x](https://github.com/BepInEx/BepInEx/releases) (Mono, x64)

Tested on Fallout Shelter running on Unity 6000.0.58 with BepInEx 5.4.23.2.

## Installation
1. Install BepInEx 5 into the game folder (next to `FalloutShelter.exe`) and start the game once.
2. Download `FSAutoHeal.dll` from the [Releases](../../releases) page.
3. Put it into `BepInEx/plugins`.
4. Start the game. The log (`BepInEx/LogOutput.log`) should contain `FS AutoHeal loaded`.

Back up your save before using any mods.

## Configuration
After the first launch, edit `BepInEx/config/com.conf.fsautoheal.cfg` (with the game closed):

| Section | Option | Default | Meaning |
|---|---|---|---|
| Hotkeys | ToggleKey | F7 | Turns auto-healing on/off in game |
| General | EnabledOnStart | true | Is it on when the game starts |
| General | CheckInterval | 0.5 | Seconds between checks |
| Stimpak | Enabled | true | Auto-use Stimpaks |
| Stimpak | UseWhenHealthBelowPercent | 50 | Use at or below this health % |
| RadAway | Enabled | true | Auto-use RadAway |
| RadAway | UseWhenRadiationAbovePercent | 30 | Use at or above this radiation % |

If dwellers get hurt faster than the check interval, raise the health percent or lower `CheckInterval`.

## Known limitations
- Works at any time, not only during incidents.
- PC (Steam) version only; mobile versions are not supported.

## Building from source
1. Install BepInEx into your game folder.
2. Copy `GameDir.user.props.example` to `GameDir.user.props` and set your game folder.
3. Build `FSAutoHeal.csproj` (Visual Studio or `dotnet build -c Release`).
4. Copy `bin/Release/net472/FSAutoHeal.dll` to `BepInEx/plugins`.

The game's own DLLs are not included in this repository.

## License

CC0 1.0 Universal

Fallout and Vault Boy are trademarks of Bethesda Softworks. This project is not affiliated with Bethesda; the image is not covered by this repository’s license.

---

## По-русски

Плагин для BepInEx для версии **Fallout Shelter** из Steam: автоматически применяет стимпаки
и РадАвей к жителям, чтобы не нажимать на каждого вручную во время рейдов и других инцидентов.

**Установка:** поставьте BepInEx 5.4.x (Mono, x64) в папку игры, запустите игру один раз,
скачайте `FSAutoHeal.dll` из раздела Releases и положите в `BepInEx/plugins`.

**Настройки** лежат в `BepInEx/config/com.conf.fsautoheal.cfg` (редактируйте при закрытой игре):
клавиша включения/выключения (по умолчанию F7), порог здоровья для стимпаков (50%),
порог радиации для РадАвея (30%), интервал проверки.

**Ограничения:** лечит не только во время нападений, а всегда; работает только в версии для ПК (Steam).
Перед использованием модов сделайте копию сохранений.
