using ImGuiNET;
using WinterEngine.ToolsFramework;

namespace MaterialEditor;

public sealed class MaterialEditorTool : EngineTool
{
    public override string ToolName => "Material Editor";

    public override void GameThink(double deltaTime)
    {
        throw new NotImplementedException();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void OnLayout()
    {
        // Draw Menubar
        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {

                ImGui.EndMenu();
            }

            ImGui.EndMenuBar();
        }
    }

    public override void Shutdown()
    {
        throw new NotImplementedException();
    }
}

