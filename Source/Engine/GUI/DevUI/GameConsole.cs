using ImGuiNET;
using log4net.Core;
using System.Numerics;
using WinterEngine.Core;
using WinterEngine.RenderSystem;
using Veneer.Controls;
using Veneer;
using static WinterEngine.Core.GameConsole;
using System.Xml.Linq;

namespace WinterEngine.DevUI;

public class UIGameConsole : Panel
{
    public const int MAX_LOG_COUNT = 500;
    public const int MAX_LIST_ITEMS = 15;

    [ConCmd("clear", "Clears the console log of it's contents.", CmdFlags.None)]
    public static void ClearConsoleCommand(string[] args)
    {
        // idk I can't do this yet oops.
    }

    struct HistoryItem
    {
        public string Text { get; private set; }

        public HistoryItem(string text)
        {
            Text = text;
        }
    }

    struct CompletionItem
    {
        public string DisplayText
        {
            get
            {
                if (IsCommand)
                    return Command;
                else
                    return $"{Command} {Value}";
            }
        }
        public string Command { get; private set; }
        public string Value { get; private set; }
        public bool IsCommand { get; private set; }

        public CompletionItem(CommandEntry command)
        {
            IsCommand = true;
            Value = "";
            Command = command.Command;
        }
    }

    string userInput = "";
    private List<LogInfo> m_Logs = new List<LogInfo>();
    private InputField m_ConLogInput;
    private bool showACField = false;
    private Dictionary<Level, bool> TypeFilters = new Dictionary<Level, bool>();
    private List<CompletionItem> acList = new List<CompletionItem>();
    private List<HistoryItem> histList = new List<HistoryItem>();

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

        // populate the filters
#if DEBUG
        TypeFilters.Add(Level.Debug, true);
#else
        TypeFilters.Add(Level.Debug, false);
#endif
        TypeFilters.Add(Level.Info, true);
        TypeFilters.Add(Level.Error, true);
        TypeFilters.Add(Level.Warn, true);
        TypeFilters.Add(Level.Notice, true);
    }

    private void PrintLogMsg(object? sender, GameConsole.LogInfo e)
    {
        m_Logs.Add(e);
    }

    void PrintMessage(string msg)
    {
        //                  idk what to do here?
        m_Logs.Add(new LogInfo(msg, "", Level.Info));
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
        m_ConLogInput.OnModified += (o, e) =>
        {
            acList.Clear();
            ComposeACFromPartial(m_ConLogInput.Value);
        };

        AddControl(m_ConLogInput);
        AddControl(confirmButton);
    }

    void ComposeACFromPartial(string partial)
    {
        // if we have a space give up lmao
        if (partial.Contains(" "))
            return;

        foreach (CommandEntry command in cmdList.Values)
        {
            if (command.Command.StartsWith(partial))
            {
                acList.Add(new CompletionItem(command));
            }
        }
    }

    #region Drawing Functions
    void CollapsingLogItem(string strid, string label, string innerMsg, Vector4 color)
    {
        // custom collapsable meant to show additional info if expanded
        // change color of header
        ImGui.PushStyleColor(ImGuiCol.Header, color);

        // create a string with as many lines as we need to display
        Vector2 textSize = ImGui.CalcTextSize(label, true, ImGui.GetContentRegionAvail().X - 18);
        float lineCount = textSize.Y / ImGui.GetTextLineHeight();
        string headerStr = "";
        for (int i = 0; i < lineCount; i++)
        {
            headerStr += "\n";
        }

        // log the cursor position
        Vector2 prePos = ImGui.GetCursorPos();

        if (ImGui.TreeNodeEx(strid, ImGuiTreeNodeFlags.CollapsingHeader, headerStr))
        {
            ImGui.Indent();

            // display the stack trace if available
            ImGui.Text(innerMsg);

            ImGui.Unindent();
        }
        ImGui.PopStyleColor();

        float postPos = ImGui.GetCursorPosY();
        ImGui.SetCursorPos(prePos + new Vector2(18, 1));
        ImGui.Text(label);
        ImGui.SetCursorPosY(postPos);
    }

    void DrawConsoleLogChild()
    {
        // Draw the scroll view with the colored text
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));

        Vector2 size = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetWindowSize().Y - 60);
        if (ImGui.BeginChild("##console_log", size))
        {
            ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);

            // used to condense the console info down.
            // TODO: we could probably optimize this to do this on message add instead?
            List<int> timesSeen = new List<int>();
            List<LogInfo> uniqueMessages = new List<LogInfo>();

            foreach (LogInfo msg in m_Logs)
            {
                if (uniqueMessages.Count == 0)
                {
                    // unique message, add it to the stack
                    uniqueMessages.Add(msg);
                    timesSeen.Add(1);
                    continue;
                }

                if (uniqueMessages[uniqueMessages.Count - 1].Text != msg.Text)
                {
                    // unique message, add it to the stack
                    uniqueMessages.Add(msg);
                    timesSeen.Add(1);
                    continue;
                }

                // this is not unique. increment the number of times seen
                timesSeen[timesSeen.Count - 1]++;
            }

            int msgIdx = 0;
            foreach (LogInfo msg in uniqueMessages)
            {
                // check if this filter is active
                if (!TypeFilters[msg.Type])
                {
                    msgIdx++;
                    continue;
                }

                Vector4 typeColor = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);

                if (msg.Type == Level.Critical || msg.Type == Level.Error)
                {
                    typeColor = new Vector4(1, 0, 0, 0.5f);
                }
                else if (msg.Type == Level.Info)
                {
                    if (msg.Text.StartsWith("]"))
                    { break; }
                    typeColor = new Vector4(0, 0.5f, 1, 0.5f);
                }
                else if (msg.Type == Level.Warn)
                {
                    typeColor = new Vector4(1, 1, 0, 0.5f);
                }

                string headerStr = msg.Text;
                if (timesSeen[msgIdx] > 1)
                {
                    headerStr = $"(x{timesSeen[msgIdx]})  {headerStr}";
                }
                CollapsingLogItem($"console_log_item_{msgIdx}", headerStr, msg.Inner, typeColor);

                msgIdx++;
            }
            ImGui.PopTextWrapPos();

            if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 50)
                ImGui.SetScrollY(ImGui.GetScrollMaxY());
        }
        ImGui.EndChild();

        ImGui.PopStyleColor();
    }

    protected override void RawLayout()
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

        ImGui.SetCursorPos(ImGui.GetCursorStartPos());

        DrawConsoleLogChild();
        if (ImGui.BeginPopupContextItem("console_filter_popup"))
        {
            foreach (Level level in TypeFilters.Keys)
            {
                bool oldValue = TypeFilters[level];
                ImGui.Checkbox($"{level.Name}##console_filter_{level.DisplayName}", ref oldValue);
                TypeFilters[level] = oldValue;
            }

            ImGui.EndPopup();
        }

        bool dispAC = m_ConLogInput.IsActive || showACField;
        showACField = false;
        Vector2 ACPos = m_ConLogInput.Position + ImGui.GetWindowPos();

        #region Display Autocomplete List
        if (dispAC && (histList.Count > 0 || acList.Count > 0))
        {
            ImGui.SetNextWindowPos(ACPos + new Vector2(0, ImGui.GetTextLineHeight()), ImGuiCond.Always);
            if (ImGui.Begin("##console_autocomplete_list", ref dispAC,
                ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.Tooltip))
            {
                // if we have an autocomplete list
                if (acList.Count > 0)
                {
                    for (int i = 0; i < acList.Count && i < MAX_LIST_ITEMS; i++)
                    {
                        string text;

                        if (i == MAX_LIST_ITEMS - 1)
                        {
                            text = "...";
                        }
                        else
                        {
                            text = acList[i].DisplayText;
                        }

                        bool isSelected = false;
                        if (ImGui.Selectable(text, ref isSelected))
                        {
                            if (text != "...")
                            {
                                userInput = acList[i].Command;
                                showACField = false;
                            }
                        }

                        if (!isSelected && ImGui.IsItemHovered())
                        {
                            showACField = true;
                        }
                    }
                }
                else // no AC, display history instead
                {
                    for (int i = 0; i < histList.Count && i < MAX_LIST_ITEMS; i++)
                    {
                        string text;

                        if (i == MAX_LIST_ITEMS - 1)
                        {
                            text = "...";
                        }
                        else
                        {
                            text = histList[i].Text;
                        }

                        bool isSelected = false;
                        if (ImGui.Selectable(text, ref isSelected))
                        {
                            if (text != "...")
                            {
                                userInput = histList[i].Text;
                                showACField = false;
                            }
                        }

                        if (!isSelected && ImGui.IsItemHovered())
                        {
                            showACField = true;
                        }
                    }
                }

                ImGui.End();
            }
        }
        #endregion
    }
    #endregion
}
