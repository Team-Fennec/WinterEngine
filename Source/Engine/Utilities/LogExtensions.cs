using log4net.Core;
using System.Reflection;

namespace WinterEngine.Utilities;

public static class LogExtensions
{
    public static void Notice(this ILog log, string message)
    {
        log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, Level.Notice, message, null);
    }

    public static void Notice(this ILog log, string message, params object[] args)
    {
        string output = string.Format(message, args);
        log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, Level.Notice, output, null);
    }
}
