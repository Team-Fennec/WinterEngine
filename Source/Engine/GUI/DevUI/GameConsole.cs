using ImGuiNET;
using log4net.Core;
using System.Numerics;
using WinterEngine.Core;

namespace WinterEngine.Gui.DevUI;

public class UIGameConsole : ImGuiPanel
{
    string userInput = "";

    public const int MAX_LOG_COUNT = 500;

    List<(string msg, Vector4 col)> m_LogLines = new List<(string msg, Vector4 col)>();

    public UIGameConsole()
    {
        Title = "Console";
        Size = new Vector2(500, 400);
        Pos = new Vector2(100, 100);
        Flags = ImGuiWindowFlags.NoSavedSettings;
        Visible = false; // start invisible
        ID = "game_console";

        LoadSchemeFile("ToolsScheme.res");

        GameConsole.OnLogMessage += PrintLogMsg;
    }

    private void PrintLogMsg(object? sender, GameConsole.LogInfo e)
    {
        Vector4 typeColor = new Vector4(1, 1, 1, 1);

        if (e.Type == Level.Error || e.Type == Level.Fatal)
        {
            typeColor = new Vector4(1, 0, 0, 1);
        }
        else if (e.Type == Level.Info)
        {
            typeColor = new Vector4(0.5f, 0.5f, 0.5f, 1);
        }
        else if (e.Type == Level.Warn)
        {
            typeColor = new Vector4(1, 1, 0, 1);
        }

        PrintMessage(e.Text, typeColor);
    }

    void PrintMessage(string msg)
    {
        PrintMessage(msg, Vector4.One);
    }

    void PrintMessage(string msg, Vector4 color)
    {
        if (m_LogLines.Count == MAX_LOG_COUNT)
        {
            m_LogLines.RemoveAt(0);
        }
        m_LogLines.Add((msg, color));
    }

    protected override void OnLayout()
    {
        // Draw text overlay in top right
        if (ImGui.Begin("##engine_version_overlay", ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoDocking
            | ImGuiWindowFlags.NoBringToFrontOnFocus
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoInputs
            | ImGuiWindowFlags.NoFocusOnAppearing
            | ImGuiWindowFlags.NoBackground))
        {
            ImGui.Text($"WinterEngine {EngineVersion.Version.Major} patch {EngineVersion.Version.Minor} (build {EngineVersion.Build})");
#if DEBUG
            ImGui.Text("Build Mode: Debug");
#else
            ImGui.Text("Build Mode: Release");
#endif

            ImGui.SetWindowPos(new(ImGui.GetMainViewport().WorkSize.X - ImGui.GetWindowSize().X, 0));
            ImGui.End();
        }

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
            PrintMessage(userInput);
            if (userInput != "")
                Engine.ExecuteCommand(userInput);
            userInput = "";
        }
    }
}
