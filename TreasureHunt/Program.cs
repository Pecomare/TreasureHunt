using TreasureHunt;

if (args.Length > 2 
    || (args.Length == 1 
        && args[0].Equals("--help")))
{
    Console.WriteLine("Usages\r\nTreasureHunt\r\nTreasureHunt <input file>\r\nTreasureHunt <input file> <output file>");
    Environment.Exit(0);
}

string inputPath = args.Length >= 1 ? args[0] : "default.in";
string outputPath = args.Length >= 2 ? args[1] : "default.out";

Map map = Map.CreateMapFromFilePath(inputPath);

map.Execute();

map.ExportToFile(outputPath);