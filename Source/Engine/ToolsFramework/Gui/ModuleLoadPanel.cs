using ImGuiNET;
using System.Numerics;
using Veneer;
using Veneer.Controls;
using WinterEngine.Core;

namespace WinterEngine.ToolsFramework.Gui;

internal class ToolListControl : Control
{
    public List<string> Items;
    int m_SelectedItem = -1;

    public event EventHandler<int> OnItemSelected;

    protected override void OnLayout()
    {
        if (ImGui.BeginChild("##module_list_child", Size))
        {
            for (int i = 0; i < Items.Count; i++)
            {
                bool isSelected = false;
                ImGui.Selectable(Items[i], ref isSelected);
                if (isSelected)
                {
                    m_SelectedItem = i;
                    OnItemSelected?.Invoke(this, i);
                }
            }

            ImGui.EndChild();
        }
    }
}

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

        ToolListControl toolList = new ToolListControl()
        {
            Position = new Vector2(0, 0),
            Size = new Vector2(280, 320),
            AutoSizeX = true,
            AutoSizeY = true,
            Items = new List<string>()
        };

        foreach (string fileName in Directory.GetFiles("bin/tools", "*.dll"))
        {
            toolList.Items.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        toolList.OnItemSelected += (o, e) =>
        {
            selectedTool = toolList.Items[e];
        };

        AddControl(loadButton);
        AddControl(cancelButton);
        AddControl(toolList);
    }
}
