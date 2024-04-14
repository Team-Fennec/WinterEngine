using ImGuiNET;
using System.Numerics;
using WinterEngine.Core;
using Veneer;

namespace WinterEngine.ToolsFramework.Gui;

// there is no reason to inherit this class ever
public sealed class ToolRootPanel : Panel
{
    public ToolRootPanel()
    {
        Title = "Engine Tools#engine_tools_root";
        Flags = ImGuiWindowFlags.NoDecoration
        | ImGuiWindowFlags.NoMove
        | ImGuiWindowFlags.MenuBar
        | ImGuiWindowFlags.NoSavedSettings
        | ImGuiWindowFlags.NoDocking
        | ImGuiWindowFlags.NoBringToFrontOnFocus;
        Size = ImGui.GetMainViewport().WorkSize;
        Visible = true;

        LoadSchemeFile("ToolsScheme.res");

        SetResizable(false);
        SetTitlebarVisible(false);
    }

    protected override void RawLayout()
    {
        ImGui.SetWindowPos(Vector2.Zero);

        uint DockSpaceID = ImGui.GetID("engine_tools_dockspace");
        ImGui.DockSpace(DockSpaceID, new Vector2(0.0f, 0.0f), ImGuiDockNodeFlags.None);

        GuiManager.SetPanelVisible("game_console", true);
        Panel gameConsole = GuiManager.GetPanel("game_console");
        if (gameConsole != null)
            gameConsole.DockID = DockSpaceID;

        if (ImGui.BeginMenuBar())
        {

            if (ImGui.BeginMenu("Tools"))
            {
                foreach (EngineTool tool in ToolsFramework.GetToolList())
                {
                    if (ImGui.MenuItem(tool.ToolName))
                    {
                        ToolsFramework.SwitchTool(tool.ToolName);
                    }
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Load tool module"))
                {
                    GuiManager.SetPanelVisible("load_tool_module_dialog", true);
                }
                ImGui.EndMenu();
            }

            // display tool name centered
            if (ToolsFramework.GetCurrentTool() == null)
            {
                ImGui.SetCursorPosX((ImGui.GetMainViewport().WorkSize.X / 2) - (ImGui.CalcTextSize("No Tool Loaded").X / 2));
                ImGui.Text("No Tool Loaded");
            }
            else
            {
                ImGui.SetCursorPosX((ImGui.GetMainViewport().WorkSize.X / 2) - (ImGui.CalcTextSize(ToolsFramework.GetCurrentTool().ToolName).X / 2));
                ImGui.Text(ToolsFramework.GetCurrentTool().ToolName);
            }

            ImGui.EndMenuBar();
        }
    }
}
