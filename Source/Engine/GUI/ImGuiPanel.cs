using ImGuiNET;
using System.Numerics;
using ValveKeyValue;
using WinterEngine.Resource;

namespace WinterEngine.Gui;

public class ImGuiPanel
{
    protected static Guid Guid = Guid.NewGuid();

    public string Title = "";
    public Vector2 Size = Vector2.Zero;
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public bool Visible = true;

    List<(ImGuiCol style, Vector4 color)> m_StyleColors = new();

    public void SetTitlebarVisible(bool visible)
    {
        if (visible && Flags.HasFlag(ImGuiWindowFlags.NoTitleBar))
        {
            Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }
        else if (!Flags.HasFlag(ImGuiWindowFlags.NoTitleBar))
        {
            Flags |= ImGuiWindowFlags.NoTitleBar;
        }
    }

    public void SetResizable(bool resizable)
    {
        if (resizable && Flags.HasFlag(ImGuiWindowFlags.NoResize))
        {
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else if (!Flags.HasFlag(ImGuiWindowFlags.NoResize))
        {
            Flags |= ImGuiWindowFlags.NoResize;
        }
    }

    protected void LoadSchemeFile(string filename)
    {
        Stream fileData = ResourceManager.GetData($"resource/{filename}");
        m_StyleColors.Clear();

        // run scheme file through our kv interpreter
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject schemeData = kv.Deserialize(fileData);

        // make a list of the color vars we have
        Dictionary<string, Vector4> colorVars = new();
        foreach (KVObject colorObj in (IEnumerable<KVObject>)schemeData["Colors"])
        {
            // Colors are stored in RGB888/RGBA8888 and must be converted
            string[] sepColorStr = colorObj.Value.ToString().Split(" ");
            float[] convColor = new float[4];

            convColor[0] = float.Parse(sepColorStr[0]) / 255;
            convColor[1] = float.Parse(sepColorStr[1]) / 255;
            convColor[2] = float.Parse(sepColorStr[2]) / 255;
            if (sepColorStr.Length > 3)
                convColor[3] = float.Parse(sepColorStr[3]) / 255;
            else
                convColor[3] = 1.0f;

            colorVars.TryAdd(colorObj.Name, new Vector4(convColor[0], convColor[1], convColor[2], convColor[3]));
        }

        // parse out each style var we have in the styles block
        foreach (KVObject styleObj in (IEnumerable<KVObject>)schemeData["Styles"])
        {
            // get our enum value
            ImGuiCol enumVal = Enum.Parse<ImGuiCol>($"ImGuiCol.{styleObj.Name}");

            // do we happen to read equal to a color var on value?
            string colorVar = string.Empty;
            foreach (string varName in colorVars.Keys)
            {
                if (styleObj.Value.ToString() == varName)
                {
                    colorVar = varName;
                    break;
                }
            }

            if (colorVar != string.Empty)
            {
                colorVars.TryGetValue(colorVar, out var color);
                m_StyleColors.Add((enumVal, color));
            }
            else
            {
                // convert a presumably RGB888/RGBA8888
                string[] sepColorStr = styleObj.Value.ToString().Split(" ");
                float[] convColor = new float[4];

                convColor[0] = float.Parse(sepColorStr[0]) / 255;
                convColor[1] = float.Parse(sepColorStr[1]) / 255;
                convColor[2] = float.Parse(sepColorStr[2]) / 255;
                if (sepColorStr.Length > 3)
                    convColor[3] = float.Parse(sepColorStr[3]) / 255;
                else
                    convColor[3] = 1.0f;

                m_StyleColors.Add((enumVal, new Vector4(convColor[0], convColor[1], convColor[2], convColor[3])));
            }
        }
    }

    public void DoLayout()
    {
        if (!Visible) return;
        foreach (var styleStruct in m_StyleColors)
        {
            ImGui.PushStyleColor(styleStruct.style, styleStruct.color);
        }

        ImGui.SetWindowSize(Size, ImGuiCond.Once);
        if (ImGui.Begin($"{Title}##{Guid}", ref Visible, Flags))
        {

#if false
            foreach (GuiControl control in controls)
            {
                control.DoLayout();
            }
#endif

            // call all user defined commands after we do our own controls
            OnLayout();

            ImGui.End();
        }

        // don't try to pop anything if we didn't push anything :P
        if (m_StyleColors.Count > 0)
            ImGui.PopStyleColor(m_StyleColors.Count);
    }

    // This is what you override to do the panel layout with ImGui
    protected virtual void OnLayout() { }
}
