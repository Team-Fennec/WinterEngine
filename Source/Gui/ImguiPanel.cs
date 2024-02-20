using ImGuiNET;
using Microsoft.Xna.Framework;

namespace WinterEngine.Gui;

public abstract class ImguiPanel {
    public string Name = "";
    public Vector2 Size = Vector2.Zero;
    public bool Visible = true;

    public void DrawLayout() {
        if (Visible) {
            if (ImGui.Begin(Name, ref Visible)) {
                OnLayout();

                ImGui.End();
            }
        }
    }

    protected abstract void OnLayout();
}
