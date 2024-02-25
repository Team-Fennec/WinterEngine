using ImGuiNET;
using System.Numerics;

namespace WinterEngine.Gui;

public abstract class ImGuiPanel {
    protected static Guid Guid = Guid.NewGuid();

    public string Title = "";
    public Vector2 Size = Vector2.Zero;
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public bool Visible = true;

    public void DoLayout() {
        ImGui.SetWindowSize(Size, ImGuiCond.Once);
        if (ImGui.Begin($"{Title}##{Guid}", ref Visible, Flags)) {
            OnLayout();

            ImGui.End();
        }
    }

    protected abstract void OnLayout();
}
