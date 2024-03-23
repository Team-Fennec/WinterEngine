using ImGuiNET;
using System.Numerics;
using WinterEngine.Core;
using WinterEngine.Gui;

namespace WinterEngine.ToolsFramework.Gui;

// there is no reason to inherit this class ever
public sealed class ToolRootPanel : ImGuiPanel
{

    public ToolRootPanel()
    {
        Title = "Engine Tools#engine_tools_root";
        Flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoBringToFrontOnFocus;
        Size = ImGui.GetMainViewport().WorkSize;

        SetResizable(false);
        SetTitlebarVisible(false);
    }

    void ModulePopup()
    {
        // get a list of every dll within the tools folder
        if (ImGui.BeginPopup("Load Tool Module##engine_tools_module_load"))
        {
            string toolItem = "";
            // list box

            if (ImGui.Button("Load") && toolItem != "")
            {
                //Engine.SendCommand($"tool_load {toolItem}");
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }

    protected override void OnLayout()
    {
        uint DockSpaceID = ImGui.GetID("engine_tools_dockspace");
        ImGui.DockSpace(DockSpaceID, new Vector2(0.0f, 0.0f), ImGuiDockNodeFlags.None);

        ModulePopup();

        if (ImGui.BeginMenuBar())
        {
            if (ToolsFramework.GetCurrentTool() != null)
                m_CurrentTool.LayoutMenuBar();

            if (ImGui.BeginMenu("Tools"))
            {
                foreach (EngineTool tool in ToolsFramework.GetToolList())
                {
                    if (ImGui.MenuItem(tool.ToolName))
                    {
                        SwitchTool(tool.ToolName);
                    }
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Load tool module"))
                {
                    ImGui.Popup("engine_tools_module_load");
                }
                ImGui.EndMenu();
            }

            // display tool name centered
            ImGui.SetCursorPosX((ImGui.GetMainViewport().GetWorkSize().x / 2) - (ImGui.CalcTextSize(ToolsFramework.GetCurrentTool().ToolName).x / 2));
            ImGui.Text(ToolsFramework.GetCurrentTool().ToolName);

            ImGui.EndMenubar();
        }
    }
}
