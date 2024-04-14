using ImGuiNET;
using System.Numerics;
using Veneer;
using Veneer.Controls;
using WinterEngine.Core;

namespace WinterEngine.ToolsFramework.Gui;

internal class ModuleLoadPanel : Panel
{
    string selectedTool = "";

    public ModuleLoadPanel()
    {
        Title = "Load Tool";
        Size = new Vector2(300, 400);
        Flags = ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDocking;
        ID = "load_tool_module_dialog";
        Visible = false;
        SetResizable(false);

        LoadSchemeFile("ToolsScheme.res");
        CreateGui();
    }

    protected override void CreateGui()
    {
        Button loadButton = new Button()
        {
            Position = new Vector2(6, 340),
            Size = new Vector2(122, 24),
            Text = "Load"
        };
        loadButton.OnPushed += (o, e) =>
        {
            GuiManager.SetPanelVisible("load_tool_module_dialog", false);
            Engine.ExecuteCommand($"tool_load {selectedTool}");
        };

        Button cancelButton = new Button()
        {
            Position = new Vector2(134, 340),
            Size = new Vector2(122, 24),
            Text = "Cancel"
        };
        cancelButton.OnPushed += (o, e) =>
        {
            GuiManager.SetPanelVisible("load_tool_module_dialog", false);
        };

        AddControl(loadButton);
        AddControl(cancelButton);
    }
}
