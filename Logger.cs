using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DoomGame.Debug
{
    public enum LogType
    {
        Normal,
        Info,
        Warning,
        Error
    }

    public static class Logger
    {
        private const string LogFolder = "logs/";

        private static FileStream logFileWriter;

        /// <summary>
        /// initializes the logger, creating a log file for this application lifetime.
        /// </summary>
        internal static void Init()
        {
            DateTime currentTime = DateTime.Now;
            string fileName = $"session_{currentTime.Year}{currentTime.Month}{currentTime.Day}_" +
                $"{currentTime.Hour}{currentTime.Minute}{currentTime.Second}.log";

            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);
            logFileWriter = File.OpenWrite($"{LogFolder}{fileName}");
            Log("Logger", "Logger Intitialized");
        }

        /// <summary>
        /// Flushes any unwritten data to the log file and closes the file
        /// </summary>
        internal static void Shutdown()
        {
            logFileWriter.Flush();
            logFileWriter.Close();
        }

        public static void Log(string source, string message, LogType type = LogType.Normal)
        {
            string prefix = "*";
            ConsoleColor color = ConsoleColor.White;

            switch (type)
            {
                default:
                case LogType.Normal:
                    break;
                case LogType.Info:
                    prefix = "I";
                    color = ConsoleColor.Blue;
                    break;
                case LogType.Warning:
                    prefix = "W";
                    color = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    prefix = "E";
                    color = ConsoleColor.Red;
                    break;
            }

            DateTime currentTime = DateTime.Now;
            string timeStamp = $"{currentTime.Hour}:{currentTime.Minute}:{currentTime.Second}";

            string fullMessage = $"[{timeStamp}][{source}][{prefix}] {message}";

            Console.ForegroundColor = color;
            Console.WriteLine(fullMessage);
            Console.ForegroundColor = ConsoleColor.White;

            // write async so that logging doesn't stop the game.
            logFileWriter.Write(Encoding.ASCII.GetBytes($"{fullMessage}\n"));
            logFileWriter.Flush();

        #if DEBUG
            System.Diagnostics.Debug.WriteLine(fullMessage);
        #endif
        }
    }
}
