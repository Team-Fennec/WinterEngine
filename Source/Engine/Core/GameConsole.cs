using log4net.Appender;
using log4net.Core;
using System.Diagnostics;
using System.Reflection;
using Veneer;
using WinterEngine.Core;
using WinterEngine.RenderSystem;
using WinterEngine.Utilities;

namespace WinterEngine.Core
{
    internal class GameConsoleAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            GameConsole.LogMessage(RenderLoggingEvent(loggingEvent), loggingEvent.LocationInformation.FullInfo, loggingEvent.Level);
        }
    }

    public static class GameConsole
    {
        public struct LogInfo
        {
            public string Text { get; private set; }
            public string Inner { get; private set; }
            public Level Type { get; private set; }

            public LogInfo(string msg, string inner, Level type)
            {
                Text = msg;
                Inner = inner;
                Type = type;
            }
        }

        public sealed class CommandEntry
        {
            public string Command => attr.Command;
            public string Description => attr.Description;
            public CmdFlags Flags => attr.Flags;

            private ConCmdAttribute attr;
            private MethodInfo cmdMethod;

            public CommandEntry(ConCmdAttribute attribute, string method, System.Type type)
            {
                attr = attribute;

                cmdMethod = type.GetMethod(method,
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    CallingConventions.Any,
                    new System.Type[] { typeof(string[]) },
                    null
                );
            }

            public CommandEntry(ConCmdAttribute attribute, MethodInfo method)
            {
                attr = attribute;
                cmdMethod = method;
            }

            public void Exec(string[] args)
            {
                // invoke the method
                if (cmdMethod != null)
                {
                    cmdMethod.Invoke(null, new object[] { args });
                }
            }
        }

        public static CommandEntry? GetCommand(string cmdName)
        {
            cmdList.TryGetValue(cmdName, out var command);
            return command;
        }

        private static void RegisterCommand(CommandEntry command)
        {
            if (cmdList.ContainsKey(command.Command))
            {
                LogManager.GetLogger("Console").Error($"Failed to register command {command.Command}, key is already registered.");
            }
            else
            {
                cmdList.Add(command.Command, command);
                LogManager.GetLogger("Console").Info($"Registered command {command.Command}");
            }
        }

        /// <summary>
        /// Registers all commands found within the calling Assembly.
        /// </summary>
        public static void RegisterAllCommands()
        {
            // search every type oh god what the fuck
            LogManager.GetLogger("Console").Info("Searching types for console command attributes, this may take a minute...");
            MethodInfo[] methods = Assembly.GetCallingAssembly().GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(ConCmdAttribute), false).Length > 0)
                      .ToArray();
            foreach (MethodInfo method in methods)
            {
                RegisterCommand(new CommandEntry(
                    (ConCmdAttribute)method.GetCustomAttributes(typeof(ConCmdAttribute)).ToArray()[0],
                    method.Name,
                    method.DeclaringType
                ));
            }
        }

        public static event EventHandler<LogInfo> OnLogMessage;

        public static void LogMessage(string message, string inner, Level level)
        {
            LogInfo info = new LogInfo(message, inner, level);
            logMessages.Add(info);
            OnLogMessage?.Invoke(null, info);
        }

        private static List<LogInfo> logMessages = new List<LogInfo>();
        public static Dictionary<string, CommandEntry> cmdList = new Dictionary<string, CommandEntry>();
        public static bool ShowConsole = true;

        // baseline commands
        [ConCmd("help", "usage: <command/var>\nShows a description of a specified command or cvar.", CmdFlags.None)]
        public static void HelpCommandExec(string[] args)
        {
            if (args.Length == 0)
            {
                LogManager.GetLogger("Command").Notice("Available Commands:");
                List<string> cmdList = new List<string>();
                foreach (CommandEntry command in GameConsole.cmdList.Values)
                {
                    cmdList.Add(command.Command);
                }
                LogManager.GetLogger("Command").Notice(string.Join(", ", cmdList));
            }
            else
            {
                if (cmdList.ContainsKey(args[0]))
                {
                    cmdList.TryGetValue(args[0], out var command);
                    LogManager.GetLogger("Command").Notice($"{args[0]}: {command.Description}");
                }
            }
        }

        [ConCmd("quit", "Quit to desktop", CmdFlags.None)]
        public static void QuitCmdExec(string[] args)
        {
            Device.Window.Close();
        }

        [ConCmd("show_imgui_demo", "Show the ImGui Demo Window", CmdFlags.None)]
        public static void ImGuiDemoCmdExec(string[] args)
        {
            GuiManager.ShowImguiDemo = true;
        }
    }
}
