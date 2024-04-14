using System;
using WinterEngine.Core;
using log4net.Core;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

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
            TextTag tag = new TextTag("info-log");
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

        private void PrintToConsole(object? sender, GameConsole.LogInfo e)
        {
            TextIter insertIter = LogBuffer.EndIter;

            // print with tag
            if (e.Type == Level.Error || e.Type == Level.Fatal)
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, e.Text, "error-log");
            }
            else if (e.Type == Level.Info)
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, e.Text, "info-log");
            }
            else if (e.Type == Level.Warn)
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, e.Text, "warn-log");
            }
            else
            {
                LogBuffer.InsertWithTagsByName(ref insertIter, e.Text, "notice-log");
            }
        }
    }
}