[![GitHub all releases](https://img.shields.io/github/downloads/RandomGuyJCI/Pacemaker/total)](https://github.com/RandomGuyJCI/Pacemaker/releases/latest)
[![Contributions Welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/RandomGuyJCI/RDGameplayPatches/issues)
[![Discord](https://img.shields.io/discord/296802696243970049?color=%235865F2&label=discord&logo=Discord&logoColor=%23ffffff)](https://discord.gg/rhythmdr)

<div align="center">
  <h1>Pacemaker</h1>
  <i>A performance optimization plugin for <a href="https://rhythmdr.com">Rhythm Doctor</a>.</i>
</div>

---

## Features

- **Custom Animations:** Optimized the CustomAnimation.LateUpdate() function, significantly improving the performance of levels with lots of decorations and/or custom character animations. When tested with [Play With Fire](https://codex.rhythm.cafe/play-wit-hqHVhSHwfyH.rdzip), the call counts per second of the function [increased by 1615%](https://i.imgur.com/uVos6KK.png) and the level now runs at a consistent 60 fps.
- **Bloom:** Fixed a [memory leak](https://cdn.discordapp.com/attachments/751852834320023593/1004421730107924510/unknown.png) caused by a temporary render texture not being properly released. Levels that extensively use bloom should no longer crash the game after running for a long period of time.

## TODO
- [ ] Optimize editor loading and scrubbing performance

## Installation
1. Download the latest version of **BepInEx 5 x86** [here](https://github.com/BepInEx/BepInEx/releases/latest). \
**Make sure you use the x86 version of BepInEx 5!** RD is x86 so the x64 version of BepInEx will not work, and BepInEx 6 is currently not yet compatible with BepInEx 5 mods.
2. Unzip the file into your RD folder. You should have a `winhttp.dll`, `doorstop_config.ini`, and `BepInEx` folder next to Rhythm Doctor.exe.
3. Launch RD once to generate BepInEx files.
4. Download the latest version of the mod from [here](https://github.com/RandomGuyJCI/Pacemaker/releases). It should be named `Pacemaker_1.x.x.zip`.
5. Unzip the file you downloaded into your Rhythm Doctor installation folder. You should now have a file at `BepInEx/plugins/Pacemaker/Pacemaker.dll`.
6. Launch the game, and the plugin should automatically enable.

For more information, check out the [BepInEx installation guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html).
