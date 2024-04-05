using ImGuiNET;
using System.Diagnostics;

namespace Veneer.Controls;

public class InputField : Control
{
	public string Label = "";
	public string Value = "";
	public int MaxChars = 2048;

	public event EventHandler<string> OnConfirmed;
	public event EventHandler<string> OnModified;

	protected override void OnLayout()
	{
		ImGui.SetCursorPos(Position);
		string oldVal = Value;
		if (ImGui.InputText($"{Label}##{this.Guid.ToString()}", ref Value, (uint)MaxChars, ImGuiInputTextFlags.EnterReturnsTrue))
		{
			OnConfirmed?.Invoke(this, Value);
		}
		if (oldVal != Value)
		{
			OnModified?.Invoke(this, Value);
		}
	}
}
