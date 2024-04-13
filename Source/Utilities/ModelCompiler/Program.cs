using System.IO;

namespace ModelCompiler;

internal class Program
{
    static void PrintArg(string key, string description)
    {
        Console.WriteLine($"-{key} | {description}");
    }

    static void PrintHelp()
    {
        Console.WriteLine("Usage: -in [file path] -out [file path] [args]");
        Console.WriteLine("===============================\nArguments:");
        Console.WriteLine("\n   Required:");
        PrintArg("in [file path]", "Defines the input config file for the model.");
        PrintArg("out [file path]", "Defines the output path for the compiled model.");
        Console.WriteLine("\n   Optional:");
        PrintArg("h", "Shows this message.");
    }

    private static int Main(string[] args)
    {
        Console.WriteLine($"WinterEngine Model Compiler v{ProgInfo.Version}\n");
        if (args.Length == 0 || args.Contains("-h"))
        {
            PrintHelp();
            return 0;
        }

        return 0;
    }
}
