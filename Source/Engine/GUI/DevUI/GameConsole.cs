using ImGuiNET;
using log4net.Core;
using System.Numerics;
using WinterEngine.Core;
using WinterEngine.RenderSystem;
using Veneer.Controls;
using Veneer;

namespace WinterEngine.DevUI;

public class UIGameConsole : Panel
{
    string userInput = "";

    public const int MAX_LOG_COUNT = 500;

    private RichTextLabel m_ConLogLabel;
    private InputField m_ConLogInput;

    public UIGameConsole()
    {
        Title = "Console";
        Size = new Vector2(500, 400);
        Pos = new Vector2(100, 100);
        Flags = ImGuiWindowFlags.NoSavedSettings;
        Visible = false; // start invisible
        ID = "game_console";

        LoadSchemeFile("ToolsScheme.res");
        CreateGui();

        GameConsole.OnLogMessage += PrintLogMsg;
    }

    private void PrintLogMsg(object? sender, GameConsole.LogInfo e)
    {
        int[] typeColor = new int[] {255, 255, 255, 255};

        if (e.Type == Level.Error || e.Type == Level.Fatal)
        {
            typeColor = new int[] {255, 0, 0, 255};
        }
        else if (e.Type == Level.Info)
        {
            typeColor = new int[] {128, 128, 128, 255};
        }
        else if (e.Type == Level.Warn)
        {
            typeColor = new int[] {255, 255, 0, 255};
        }

        PrintMessage(e.Text, typeColor);
    }

    void PrintMessage(string msg)
    {
        PrintMessage(msg, new int[] {255, 255, 255, 255});
    }

    void PrintMessage(string msg, int[] color)
    {
        m_ConLogLabel.PushColor(color[0], color[1], color[2], color[3]);
        m_ConLogLabel.AppendText(msg);
        m_ConLogLabel.PopColor();
    }

    protected override void CreateGui()
    {
        Button confirmButton = new Button()
        {
            Size = new Vector2(80, 20),
            Position = new Vector2(80, 20),
            Text = "Submit",
            VerticalAnchor = Control.AnchorPos.End,
            HorizontalAnchor = Control.AnchorPos.End
        };

        m_ConLogLabel = new RichTextLabel()
        {
            Position = new Vector2(0, 0),
            Size = new Vector2(480, 340),
            WrapText = false,
            AutoScroll = true,
            AutoSizeY = true,
            AutoSizeX = true
        };

        m_ConLogInput = new InputField()
        {
            Label = "",
            Position = new Vector2(0, 20),
            VerticalAnchor = Control.AnchorPos.End
        };

        confirmButton.OnPushed += (o,e) => {
            PrintMessage($"{m_ConLogInput.Value}\n");
            if (m_ConLogInput.Value != "")
                Engine.ExecuteCommand(m_ConLogInput.Value);
            m_ConLogInput.Value = "";
        };
        m_ConLogInput.OnConfirmed += (o,e) => {
            PrintMessage($"{m_ConLogInput.Value}\n");
            if (m_ConLogInput.Value != "")
                Engine.ExecuteCommand(m_ConLogInput.Value);
            m_ConLogInput.Value = "";
        };

        AddControl(m_ConLogLabel);
        AddControl(m_ConLogInput);
        AddControl(confirmButton);
    }

#if false
    protected override void OnLayout()
    {
        // Draw text overlay in top right
        string mainStr = $"WinterEngine {EngineVersion.Version.Major} patch {EngineVersion.Version.Minor} (build {EngineVersion.Build})";
        Vector2 position = new(ImGui.GetMainViewport().Size.X - ImGui.CalcTextSize(mainStr).X, 0);
        ImGui.GetBackgroundDrawList().AddText(
            position,
            ImGui.ColorConvertFloat4ToU32(Vector4.One),
            mainStr
        );
        ImGui.GetBackgroundDrawList().AddText(
            position with { Y = ImGui.GetTextLineHeight() },
            ImGui.ColorConvertFloat4ToU32(Vector4.One),
#if DEBUG
            "Build Mode: Debug"
#else
            "Build Mode: Release"
#endif
        );

        // Draw the scroll view with the colored text
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));

        Vector2 size = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetWindowSize().Y - 60);
        if (ImGui.BeginChild("##console_log", size))
        {
            ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);

            foreach (var line in m_LogLines)
            {
                ImGui.TextColored(line.col, line.msg);
            }
            ImGui.PopTextWrapPos();

            if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 50)
                ImGui.SetScrollY(ImGui.GetScrollMaxY());
        }
        ImGui.EndChild();

        ImGui.PopStyleColor();

        bool hitEnter = ImGui.InputText("##console_input", ref userInput, 100, ImGuiInputTextFlags.EnterReturnsTrue);
        ImGui.SameLine();
        bool hitButton = ImGui.Button("Submit");

        if (hitEnter || hitButton)
        {
            
        }
    }
#endif
}
