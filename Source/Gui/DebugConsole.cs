using ImGuiNET;
using Microsoft.Xna.Framework;

namespace WinterEngine.Gui;

public sealed class DebugConsole : ImguiPanel {
    private string conInput = "";

    public DebugConsole() {
        Name = "Console";
        Size = new Vector2(320, 320);
    }

    protected override void OnLayout() {
        ImGui.Text("hello world this is the dev console");
        
        ImGui.InputText("##console_input", ref conInput, 1024);
        ImGui.SameLine();
        ImGui.Button("Submit##console_submit");
    }
}
