using libGod2Iso;
using System.Reflection;
using System.IO;

namespace God2IsoCli;

internal class Program 
{
    private static readonly string DefaultOutputDirectory = Environment.CurrentDirectory;

    public static void Main(string[] args)
    {
        Arguments arguments;

        if(args.Length == 1)
        {
            switch(args[0].ToLower())
            {
                case "-h":
                case "--help":
                    ShowUsage();
                    return;
                case "-v":
                case "--version":
                    ShowVersionInfo();
                    return;
                default:
                    // Error case. Let ParseArguments handle it
                    break;
            }
        }

        try
        {
            arguments = ParseArguments(args);
        }
        catch(Exception ex)
        {
            Console.WriteLine("An error occurred while parsing arguments: " + ex.Message);
            return;
        }

        if(arguments.Packages.Count == 0)
        {
            if(!arguments.Silent)
                Console.WriteLine("You must specify at least one package. -h or --help for details");
            return;
        }

        if(!arguments.Silent)
        {
            Console.WriteLine("Output Directory: " + arguments.OutputDirectory);
            Console.WriteLine("Fix \"CreateGoodIso\" Header: " + arguments.CreateGoodIsoHeader);
        }

        foreach(var package in arguments.Packages)
        {
            if(!arguments.Silent) Console.WriteLine(package + " -> " + Path.Combine(arguments.OutputDirectory, Path.GetFileNameWithoutExtension(package)) + ".iso");
            if(!GamesOnDemandConverter.ConvertPackage(package, arguments.OutputDirectory, arguments.CreateGoodIsoHeader))
            {
                if(!arguments.Silent) Console.WriteLine("Failed to convert package: " + package);
            }
        }

        if(!arguments.Silent) Console.WriteLine("Done.");
    }

    private static Arguments ParseArguments(string[] args)
    {
        List<string> packages = [];
        string? outputDirectory = null;
        bool createGoodIsoHeader = false;
        bool silent = false;

        for(int i = 0; i < args.Length; i++)
        {
            switch(args[i].ToLower())
            {
                case "-h":
                case "--help":
                case "-v":
                case "--version":
                    throw new Exception(args[i] + ": Must be the only argument");
                    break;
                case "-i":
                case "--include":
                    if(i+1 >= args.Length)
                        throw new Exception("-i: argument requires a package file to be specified");
                    var package = args[++i];
                    if(!File.Exists(package))
                        throw new Exception("-i: " + package + " does not exist");
                    packages.Add(package);    
                    break;
                case "-f": 
                case "--fix_create_good_iso_header":
                    createGoodIsoHeader = true;
                    break;
                case "-o":
                case "--output_directory":
                    if(i+1 >= args.Length)
                        throw new Exception("-o: argument requires an output directory to exist");
                    outputDirectory = args[++i];
                    if(!Directory.Exists(outputDirectory))
                        throw new Exception("-o: " + outputDirectory + " does not exist");
                    break;
                case "-s":
                case "--silent":
                    silent = true;
                    break;
                default:
                    throw new Exception("Unexpected Argument: " + args[i]);
            }
        }

        return new Arguments()
        {
            Packages = packages,
            OutputDirectory = outputDirectory ?? DefaultOutputDirectory,
            CreateGoodIsoHeader = createGoodIsoHeader,
            Silent = silent
        };
    }

    private static void ShowUsage()
    {
        Console.WriteLine("-f, --fix_create_good_iso_header: Enable Fix \"CreateGoodIso\" Header");
        Console.WriteLine("-h, --help: Show this usage screen.");
        Console.WriteLine("-i, --include: Include Package. Must be followed by a package file");
        Console.WriteLine("-o, --output_directory: Specify the output directory, Optional. Must be followed by a directory. Default output directory is the current working directory.");
        Console.WriteLine("-v, --version: Print the Version info for God2IsoCli.");
    }

    private static void ShowVersionInfo()
    {
        Console.WriteLine("God2IsoCli v" + typeof(Program).Assembly.GetName().Version.ToString());
    }
}