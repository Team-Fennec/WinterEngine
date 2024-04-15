#if HAS_PROFILING
using ImGuiNET;
using System.Numerics;
using Veneer;
using Veneer.Controls;
using WinterEngine.DevUI.Controls;

namespace WinterEngine.DevUI;

public class ProfilerPanel : Panel
{
    public ProfilerPanel()
    {
        Title = "Profiling";
        ID = "engine_profiler";
        Size = new Vector2(400, 320);
        Pos = new Vector2(0, 0);
        Flags = ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoDocking
            | ImGuiWindowFlags.NoBringToFrontOnFocus
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoInputs
            | ImGuiWindowFlags.NoFocusOnAppearing
            | ImGuiWindowFlags.NoBackground;

        LoadSchemeFile("ToolsScheme.res");

        CreateGui();
    }

    protected override void CreateGui()
    {
        ProfilerList widget_ProfList = new ProfilerList()
        {
            Position = new Vector2(0,0)
        };

        AddControl(widget_ProfList);
    }
}
#endif