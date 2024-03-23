using log4net;
using System;
using log4net.Appender;
using log4net.Core;

namespace WinterEngine.Core;

public class GameConsoleAppender : AppenderSkeleton
{
    protected override void Append(LoggingEvent loggingEvent)
    {
        GameConsole.logMessages.Add(new LogInfo(loggingEvent.RenderedMessage, loggingEvent.Level));
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

    public static List<LogInfo> logMessages = new List<LogInfo>();
    public static List<ConCmd> commands = new List<ConCmd>();
}

// baseline commands
namespace WinterEngine.ConsoleCommands
{
    public class HelpCommand : ConCmd<HelpCommand>
    {
        public override string Command => "help";
        public override string Description => "lists all commands or info on a single command";
        public override CmdFlags Flags => CmdFlags.None;

        public override void Exec(string[] args)
        {
            if (args.Length == 0)
            {
                LogManager.GetLogger("Command").Info("Available Commands:");
                string cmdList = "";
                foreach (ConCmd command in GameConsole.commands)
                {
                    cmdList += $"{command.Command},";
                }
                LogManager.GetLogger("Command").Info(cmdList);
            }
            else
            {
                foreach (ConCmd command in GameConsole.commands)
                {
                    if (command.Command == args[0])
                    {
                        LogManager.GetLogger("Command").Info($"{args[0]}: {command.Description}");
                        break;
                    }
                }
            }
        }
    }
}
