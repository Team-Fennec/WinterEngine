using ImGuiNET;
using System.Numerics;

namespace WinterEngine.Gui.Controls;

// Panels are not controls
// todo: but should they be?
public class Panel {
    protected static Guid Guid = Guid.NewGuid();

    public string Title = "";
    public Vector2 Size = Vector2.Zero;
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public bool Visible = true;
    
    public void SetTitlebarVisible(bool visible) {
        if (visible && Flags.HasFlag(ImGuiWindowFlags.NoTitlebar)) {
            Flags &= ~ImGuiWindowFlags.NoTitlebar;
        } else if (!Flag.HasFlag(ImGuiWindowFlags.NoTitlebar)) {
            Flags |= ImGuiWindowFlags.NoTitlebar;
        }
    }
    
    public void SetResizable(bool resizable) {
        if (resizable && Flags.HasFlag(ImGuiWindowFlags.NoResize)) {
            Flags &= ~ImGuiWindowFlags.NoResize;
        } else if (!Flag.HasFlag(ImGuiWindowFlags.NoResize)) {
            Flags |= ImGuiWindowFlags.NoResize;
        }
    }

    public void DoLayout() {
        ImGui.SetWindowSize(Size, ImGuiCond.Once);
        if (ImGui.Begin($"{Title}##{Guid}", ref Visible, Flags)) {
            
            foreach (GuiControl control in controls) {
                control.DoLayout();
            }
            
            // call all user defined commands after we do our own controls
            OnLayout();

            ImGui.End();
        }
    }

    // used if you want to call imgui commands directly
    protected virtual void OnLayout() {}
}
