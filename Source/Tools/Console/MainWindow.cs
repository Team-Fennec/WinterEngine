using System;
using WinterEngine.Core;
using log4net.Core;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Text.RegularExpressions;

namespace ConsoleTool
{
    public class MainWindow : Window
    {
        [UI] private Entry console_CommandEntry = null;
        [UI] private Button console_SubmitButton = null;
        [UI] private TextView console_LogView = null;

        private TextBuffer LogBuffer;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("ConsoleWindow"))
        {
            builder.Autoconnect(this);
            Icon = Gdk.Pixbuf.LoadFromResource("icons/Icon.png");

            LogBuffer = console_LogView.Buffer;
            GameConsole.OnLogMessage += PrintToConsole;

            #region TextView Tags
            TextTag tag = new TextTag("channel-block");
            Gdk.RGBA bgColor = new Gdk.RGBA();
            bgColor.Red = 0.0;
            bgColor.Blue = 0.0;
            bgColor.Green = 0.0;
            bgColor.Alpha = 1.0;
            tag.BackgroundRgba = bgColor;
            tag.Foreground = "white";
            tag.Size = 24;
            LogBuffer.TagTable.Add(tag);

            tag = new TextTag("info-log");
            tag.Foreground = "gray";
            LogBuffer.TagTable.Add(tag);

            tag = new TextTag("error-log");
            tag.Foreground = "red";
            LogBuffer.TagTable.Add(tag);

            tag = new TextTag("warn-log");
            tag.Foreground = "yellow";
            LogBuffer.TagTable.Add(tag);

            tag = new TextTag("notice-log");
            tag.Foreground = "white";
            LogBuffer.TagTable.Add(tag);
            #endregion

            console_SubmitButton.Clicked += OnButtonPressed;
            console_CommandEntry.Activated += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, EventArgs a)
        {
            TextIter insertIter = LogBuffer.EndIter;
            LogBuffer.Insert(ref insertIter, $"{console_CommandEntry.Text}\n");

            if (console_CommandEntry.Text != "")
            {
                Engine.ExecuteCommand(console_CommandEntry.Text);
                console_CommandEntry.Text = "";
            }
        }

        private void OnFilterModified(object sender, EventArgs a)
        {
            // todo: implement filtering of console messages
        }

        private void PrintToConsole(object? sender, GameConsole.LogInfo e)
        {
            TextIter insertIter = LogBuffer.EndIter;

            Regex logMsgRgx = new Regex(@"\[(?<level>\w+)\]\[(?<channel>\w+)\](?<message>.*)");
            string channel = logMsgRgx.Match(e.Text).Groups["channel"].Value;
            string message = logMsgRgx.Match(e.Text).Groups["message"].Value;

            // print with tag
            LogBuffer.InsertWithTagsByName(ref insertIter, channel.PadLeft(5).PadRight(5), "channel-block");
            if (e.Type == Level.Error || e.Type == Level.Fatal)
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, $"ERROR: {message}", "error-log");
            }
            else if (e.Type == Level.Info)
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, $"INFO: {message}", "info-log");
            }
            else if (e.Type == Level.Warn)
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, $"WARN: {message}", "warn-log");
            }
            else
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, $"NOTICE: {message}", "notice-log");
            }
        }
    }
}