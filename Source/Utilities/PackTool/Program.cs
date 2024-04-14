namespace PackTool;

internal class Program
{
    static void PrintArg(string key, string description)
    {
        Console.WriteLine($"-{key} | {description}");
    }

    static void PrintHelp()
    {
        Console.WriteLine("Usage: [args] [files]");
        Console.WriteLine("Specify files and folders to be packed. Default output is the name of the first file or folder.");
        Console.WriteLine("===============================\nArguments:");
        Console.WriteLine("\n   Optional:");
        PrintArg("h", "Shows this message.");
        PrintArg("out-name [name]", "Specifies the name of the output pack file.");
        PrintArg("x [name]", "Extracts the provided pack file");
    }

    private static int Main(string[] args)
    {
        Console.WriteLine($"WinterEngine Pack Tool v{ProgInfo.Version}\n");
        if (args.Length == 0 || args.Contains("-h"))
        {
            PrintHelp();
            return 0;
        }

        return 0;
    }
}
