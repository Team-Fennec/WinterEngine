using ImGuiNET;
using log4net;
using log4net.Appender;
using log4net.Core;
using SharpGen.Runtime.Win32;
using System.Diagnostics;
using System.Numerics;
using static WinterEngine.Gui.UIGameConsole;

namespace WinterEngine.Gui;

public class UIConsoleAppender : AppenderSkeleton {
    protected override void Append(LoggingEvent loggingEvent) {
        //if (logMessages.Count >= UIGameConsole.MAX_LOG_COUNT)
            //overlayMessages.RemoveAt(0);
        logMessages.Add(new LogInfo(loggingEvent.RenderedMessage, loggingEvent.Level));

        //if (overlayMessages.Count >= 20)
        //    overlayMessages.RemoveAt(0); // remove the oldest message
        //overlayMessages.Add(new LogInfo($"[{typeTag}] {logString}", type));
    }
}

public class UIGameConsole : ImGuiPanel {
    public struct LogInfo {
        public string Text { get; private set; }
        public Level Type { get; private set; }

        public LogInfo(string msg, Level type) {
            Type = type;
            Text = msg;
        }
    }

    public static List<LogInfo> logMessages = new List<LogInfo>();
    string userInput = "";

    public const int MAX_LOG_COUNT = 200;

    public UIGameConsole() {
        Title = "Console";
        Size = new Vector2(300, 300);
        Flags = ImGuiWindowFlags.NoSavedSettings;

    }

    protected override void OnLayout() {
        // Draw the scroll view with the colored text
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));

        Vector2 size = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetWindowSize().Y - 60);
        if (ImGui.BeginChild("##console_log", size)) {
            ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);

            foreach (LogInfo msg in logMessages) {
                Vector4 typeColor = new Vector4(1, 1, 1, 1);

                if (msg.Type == Level.Error) {
                    typeColor = new Vector4(1, 0, 0, 1);
                } else if (msg.Type == Level.Info) {
                    typeColor = new Vector4(0, 1, 1, 1);
                } else if (msg.Type == Level.Warn) {
                    typeColor = new Vector4(1, 1, 0, 1);
                } else if (msg.Type == Level.Fatal) {
                    typeColor = new Vector4(1, 0, 0, 1);
                }

                ImGui.TextColored(typeColor, msg.Text);
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

        if (hitEnter || hitButton) {
            Console.WriteLine(userInput);
            //if (userInput != "")
                //HandleUserInput(userInput);
            userInput = "";
        }
    }
}
