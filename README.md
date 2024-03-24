![](Branding/LogoB.svg)
The modular source-like inhouse C# game engine used by Team Fennec. Forever open source, MIT Licensed.

Features Include:
- Familiar file formats for Source:tm: developers (KeyValues and DataModel)
- Modular design with (relatively) interchangeable components
- Entity Component System inspired by Unity:tm: and Source 2:tm:
- Game as a module system
- Physics Engine inspired by VPhysics:tm:
- In Engine tooling system inspired by Source 2:tm:
- Complete Level Editor inspired by Source 2:tm: Hammer

## Building
In order to build WinterEngine you'll need the following:
- Windows, Mac, or Linux
- .NET 7.0
- Git

## Development:
> [!NOTE]
> This section is not finished yet

In order to work with the Engine code effecively, the following setups are recommended for the specified platforms:
> [!NOTE]
> On no platform is Mono supported at this time. While it may build, it has not been tested at all.

### Windows:
- Visual Studio 2022 or newer
- Windows 10 or newer

### Linux:
- JetBrains Rider
- DotNET Core
- EditorConfig plugin
- Arch Linux

## macOS:
- JetBrains Rider
- DotNET Core
- EditorConfig plugin
- macOS 11+

## Contribution
> [!NOTE]
> This section is not finished yet

This engine is composed of a lot of code and many different modules, this structure is extremely important to maintain to keep the engine easy to work on. As such, we have imposed the following set of guidelines for contributions:

- Pull requests should only contain changes related to the subject of the PR
- Broad PRs should be split up into several more specific PRs if possible
- Pull using rebase, avoid having merge commits in your history
- All code should be formatted according to the editorconfig file inside the repository
- All code sources should be legally sourced, Leaked code will result in a ban from contributing

# Acknowledgements
**Sources for logo:**
- https://commons.wikimedia.org/wiki/File:Nordic_Snowflake.svg
- Readex Pro (Font)

**Open Source Projects used:**
- Jolt Physics
- JoltSharp
- ImGui
- ImGuiNET
- Veldrid
- SDL2
- ValveKeyValue
- Datamodel.NET
- LibVPKEdit

# Disclaimer
Team Fennec is not affiliated with, nor endorsed by, Valve Software. Source:tm: Engine and Source 2:tm: Engine are property of Valve Software. Team Fennec is not affiliated with, nor endorsed by, Unity Technologies. Unity:tm: Engine is property of Unity Technologies.

Team Fennec is not responsible for any losses, damages, or other inconvenienes you may encounter while using this engine.
