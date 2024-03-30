> [!NOTE]
> This is still extremely work in progress and extremely experimental! As more things are completed we will polish the code up, but right now there is one programmer on the project and the focus is making the engine work over everything else.

![](Branding/LogoB.svg)
The modular source-like C# game engine used by Team Fennec.

Features Include:
- Familiar file formats for Source developers (KeyValues and DataModel)
- Modular design with (relatively) interchangeable components
- Entity Component System inspired by Unity
- Game as a module system
- Jolt Physics
- In Engine tooling system inspired by Source 2

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
> Neither macOS nor Linux are building at this time due to missing libvpkedit libraries

### Windows:
- Visual Studio 2022 or newer
- Windows 10 or newer

## Contribution
> [!NOTE]
> This section is not finished yet

This engine is composed of a lot of code and many different modules, this structure is extremely important to maintain to keep the engine easy to work on. As such, we have imposed the following set of guidelines for contributions:

- Pull requests should only contain changes related to the subject of the PR
- Broad PRs should be split up into several more specific PRs if possible
- Pull using rebase, avoid having merge commits in your history
- All code should be formatted according to the editorconfig file inside the repository

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
- SharpGLTF

# Disclaimer
Team Fennec is not affiliated with, nor endorsed by, Valve Software. Source Engine and Source 2 Engine are property of Valve Software. Team Fennec is not affiliated with, nor endorsed by, Unity Technologies. Unity Engine is property of Unity Technologies.
