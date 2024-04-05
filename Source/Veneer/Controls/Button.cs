using ImGuiNET;
using System.Diagnostics;

namespace Veneer.Controls;

public class Button : Control
{
	public string Text = "Button";

	public event EventHandler OnPushed;

	protected override void OnLayout()
	{
		ImGui.SetCursorPos(Position);
		if (ImGui.Button($"{Text}##{this.Guid.ToString()}", Size))
		{
			OnPushed?.Invoke(this, EventArgs.Empty);
		}
	}
}
