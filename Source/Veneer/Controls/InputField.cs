using ImGuiNET;
using System.Numerics;

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
        /*ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 buttonPos = ImGui.GetCursorScreenPos();
        Vector4 cTop = Vector4.Zero;
        Vector4 cBottom = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);

        drawList.AddRectFilledMultiColor(buttonPos, buttonPos + Size,
            ImGui.ColorConvertFloat4ToU32(cTop),
            ImGui.ColorConvertFloat4ToU32(cTop),
            ImGui.ColorConvertFloat4ToU32(cBottom),
            ImGui.ColorConvertFloat4ToU32(cBottom)
        );*/

        string oldVal = Value;
		if (ImGui.InputText($"{Label}##{Guid}", ref Value, (uint)MaxChars, ImGuiInputTextFlags.EnterReturnsTrue))
		{
			OnConfirmed?.Invoke(this, Value);
		}
		if (oldVal != Value)
		{
			OnModified?.Invoke(this, Value);
		}
	}
}
