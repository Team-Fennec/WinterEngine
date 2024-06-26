using ImGuiNET;
using System.Numerics;
using ValveKeyValue;
using WinterEngine.Resource;
using log4net;

namespace Veneer;

public class Panel
{
    protected Guid Guid = Guid.NewGuid();

    public string Title = "";
    public string ID { get; protected set; }
    public Vector2 Size = Vector2.Zero;
    public Vector2 Pos = Vector2.Zero;
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public bool Visible = true;
    public uint DockID = 0;

    List<(ImGuiCol style, Vector4 color)> m_StyleColors = new List<(ImGuiCol style, Vector4 color)>();
    List<(ImGuiStyleVar var, float value)> m_StyleOptions = new List<(ImGuiStyleVar var, float value)>();
    string m_ID = "";

    protected List<Control> m_Controls = new List<Control>();

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

    public void SetMenubarVisible(bool visible)
    {
        if (visible && !Flags.HasFlag(ImGuiWindowFlags.MenuBar))
        {
            Flags |= ImGuiWindowFlags.MenuBar;
        }
        else if (Flags.HasFlag(ImGuiWindowFlags.MenuBar))
        {
            Flags &= ~ImGuiWindowFlags.MenuBar;
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

    protected virtual void CreateGui()
    {
        // do your thing
    }

    protected void LoadLayoutFile(string filename)
    {

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

        // parse out each style color we have in the styles block
        foreach (KVObject styleObj in (IEnumerable<KVObject>)schemeData["Styles"])
        {
            // get our enum value
            ImGuiCol enumVal = Enum.Parse<ImGuiCol>($"{styleObj.Name}");

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

        // parse out each style var we have in the options block
        foreach (KVObject optObj in (IEnumerable<KVObject>)schemeData["Options"])
        {
            // get our enum value
            ImGuiStyleVar enumVal = Enum.Parse<ImGuiStyleVar>($"{optObj.Name}");

            m_StyleOptions.Add((enumVal, float.Parse(optObj.Value.ToString())));
        }
    }

    public void AddControl(Control control)
    {
        control.Parent = this;
        m_Controls.Add(control);
    }

    public Control? GetControl(string controlName)
    {
        foreach (Control control in m_Controls)
        {
            if (control.Name == controlName)
                return control;
        }
        LogManager.GetLogger("Veneer").Warn($"No control by the name of {controlName} was found in the control list!");
        return null;
    }
    public Control? Q(string controlName) => GetControl(controlName);

    public void DoLayout()
    {
        if (!Visible) return;
        foreach (var styleStruct in m_StyleColors)
        {
            ImGui.PushStyleColor(styleStruct.style, styleStruct.color);
        }
        foreach (var optStruct in m_StyleOptions)
        {
            ImGui.PushStyleVar(optStruct.var, optStruct.value);
        }

        if (DockID != 0)
        {
            ImGui.SetNextWindowDockID(DockID, ImGuiCond.Once);
        }

        ImGui.SetNextWindowSize(Size, ImGuiCond.Once);
        ImGui.SetNextWindowPos(Pos, ImGuiCond.Once);
        if (ImGui.Begin($"{Title}##{ID}", ref Visible, Flags))
        {
            foreach (Control control in m_Controls)
            {
                control.Draw();
            }
            RawLayout();

            ImGui.End();
        }

        // don't try to pop anything if we didn't push anything :P
        if (m_StyleColors.Count > 0)
            ImGui.PopStyleColor(m_StyleColors.Count);
        if (m_StyleOptions.Count > 0)
            ImGui.PopStyleVar(m_StyleOptions.Count);
    }

    // call raw imgui commands on a panel
    protected virtual void RawLayout() { }
}
