using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace Veneer.Controls;

public class Button : Control
{
	public string Text = "Button";

	public event EventHandler OnPushed;

	protected override void OnLayout()
	{
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 buttonPos = ImGui.GetCursorScreenPos();
        Vector4 cTop = Vector4.Zero;
        Vector4 cBottom = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);

		if (ImGui.Button($"##{Guid}", Size))
		{
			OnPushed?.Invoke(this, EventArgs.Empty);
		}

        drawList.AddRectFilledMultiColor(buttonPos, buttonPos + Size,
            ImGui.ColorConvertFloat4ToU32(cTop),
            ImGui.ColorConvertFloat4ToU32(cTop),
            ImGui.ColorConvertFloat4ToU32(cBottom),
            ImGui.ColorConvertFloat4ToU32(cBottom)
        );

        Vector2 pos = (Size / 2) - (ImGui.CalcTextSize(Text) / 2);
        uint cText = ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Text]);
        drawList.AddText(buttonPos + pos, cText, Text);
    }
}
