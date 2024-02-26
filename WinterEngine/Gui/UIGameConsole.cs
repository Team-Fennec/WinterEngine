using ImGuiNET;
using log4net;
using log4net.Core;
using System.Diagnostics;
using System.Numerics;
using WinterEngine.Core;

namespace WinterEngine.Gui;

public class UIGameConsole : ImGuiPanel {
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

            foreach (GameConsole.LogInfo msg in GameConsole.logMessages) {
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
