using ImGuiNET;

namespace Veneer.Controls;

public class Label : Control
{
	public string Text = "Button";

	protected override void OnLayout()
	{
		ImGui.Text(Text);
	}
}
