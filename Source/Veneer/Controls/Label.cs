using ImGuiNET;

namespace Veneer.Controls;

public class Label : Control
{
	public string Text = "Button";

	protected override void OnLayout()
	{
		ImGui.SetCursorPos(Position);
		ImGui.Text(Text);
	}
}
