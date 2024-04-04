using log4net.Appender;
using log4net.Core;
using System.Reflection;
using WinterEngine.Core;
using WinterEngine.RenderSystem;
using WinterEngine.Utilities;

namespace WinterEngine.Core
{
    public class GameConsoleAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            GameConsole.LogMessage(RenderLoggingEvent(loggingEvent), loggingEvent.Level);
        }
    }

    public static class GameConsole
    {
        public struct LogInfo
        {
            public string Text { get; private set; }
            public Level Type { get; private set; }

            public LogInfo(string msg, Level type)
            {
                Type = type;
                Text = msg;
            }
        }

        public static void RegisterCommand(ConCmd command)
        {
            cmdList.TryAdd(command.Command, command);
        }

        public static void LogMessage(string message, Level level)
        {
            LogInfo info = new LogInfo(message, level);
            logMessages.Add(info);
            OnLogMessage?.Invoke(null, info);
        }
        public static event EventHandler<LogInfo> OnLogMessage;

        private static List<LogInfo> logMessages = new List<LogInfo>();
        public static Dictionary<string, ConCmd> cmdList = new Dictionary<string, ConCmd>();
        public static bool ShowConsole = true;
    }
}

// baseline commands
namespace WinterEngine.ConsoleCommands
{
    internal sealed class HelpCommand : ConCmd
    {
        public override string Command => "help";
        public override string Description => "usage: <command>\nLists information on a command.";
        public override CmdFlags Flags => CmdFlags.None;


        public override void Exec(string[] args)
        {
            if (args.Length == 0)
            {
                LogManager.GetLogger("Command").Notice("Available Commands:");
                List<string> cmdList = new List<string>();
                foreach (ConCmd command in GameConsole.cmdList.Values)
                {
                    cmdList.Add(command.Command);
                }
                LogManager.GetLogger("Command").Notice(string.Join(", ", cmdList));
            }
            else
            {
                if (GameConsole.cmdList.ContainsKey(args[0]))
                {
                    GameConsole.cmdList.TryGetValue(args[0], out var command);
                    LogManager.GetLogger("Command").Notice($"{args[0]}: {command.Description}");
                }
            }
        }
    }

    internal sealed class QuitCommand : ConCmd
    {
        public override string Command => "quit";
        public override string Description => "Quit to desktop";
        public override CmdFlags Flags => CmdFlags.None;

        public override void Exec(string[] args)
        {
            Device.Window.Close();
        }
    }
}
