using log4net.Appender;
using log4net.Core;

namespace WinterEngine.Utilities;

/// <summary>
/// A much nicer colored console appender.
/// </summary>
public class ConsoleExAppender : AppenderSkeleton
{
    protected override void Append(LoggingEvent loggingEvent)
    {
        ConsoleColor fgColor = ConsoleColor.White;

        if (loggingEvent.Level == Level.Debug)
            fgColor = ConsoleColor.White;
        else if (loggingEvent.Level == Level.Info)
            fgColor = ConsoleColor.DarkGray;
        else if (loggingEvent.Level == Level.Warn)
            fgColor = ConsoleColor.Yellow;
        else if (loggingEvent.Level == Level.Error
            || loggingEvent.Level == Level.Alert
            || loggingEvent.Level == Level.Fatal)
            fgColor = ConsoleColor.Red;
        else if (loggingEvent.Level == Level.Notice)
            fgColor = ConsoleColor.Blue;

        ConsoleColor prevColor = Console.ForegroundColor;
        Console.ForegroundColor = fgColor;
        Console.Write(RenderLoggingEvent(loggingEvent));
        Console.ForegroundColor = prevColor;
    }
}
