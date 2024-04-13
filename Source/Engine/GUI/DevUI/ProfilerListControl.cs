/*
* WinterEngine Profiler Control
*/
#if HAS_PROFILING
using ImGuiNET;
using Veneer;
using WinterEngine.Diagnostics;
using System.Numerics;

namespace WinterEngine.DevUI.Controls;

public class ProfilerList : Control
{
	private static Vector4[] lineColors = new Vector4[]
    {
        new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
        new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
        new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
        new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
        new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
        new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
        new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        new Vector4(0.0f, 0.5f, 1.0f, 1.0f),
    };

    protected override void OnLayout()
    {
    	int index = 0;
        foreach (string name in Profiler.Profs.Keys)
        {
            ImGui.PushStyleColor(ImGuiCol.PlotLines, ImGui.ColorConvertFloat4ToU32(lineColors[index]));
            float[] numbers = Profiler.Profs[name];
            ImGui.PlotLines($"##{name}", ref numbers[0], 100, 0, name, float.MaxValue, float.MaxValue, new Vector2(300, 30));
            ImGui.PopStyleColor();
            index++;
        }
    }
}
#endif
