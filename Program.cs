namespace loc;

internal static class Program
{
    public static void Main(string[] args)
    {
        var dir = Path.GetFullPath(args.Length == 0 ? "." : args[0]);

        if (!Directory.Exists(dir))
        {
            Console.WriteLine("Directory not found");
            return;
        }

        var currentStats = FileStats.Scan(dir);
        if (currentStats.Count == 0)
        {
            Console.WriteLine("No source files found");
            return;
        }

        var padding = currentStats.Select(kvp => kvp.Key.Length).Max();

        var previousStats = FileStats.Load(dir);
        var report = new ReportWriter(previousStats, padding);

        foreach (var (filename, lineCount) in currentStats.OrderByDescending(x => x.Value))
            report.WriteLine(filename, lineCount);

        report.WriteTotal();

        currentStats.Save(dir);
    }
}