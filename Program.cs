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

        var newStats = Statistics.Scan(dir);
        if (newStats.Count == 0)
        {
            Console.WriteLine("No source files found");
            return;
        }

        var padding = newStats.Keys.Select(x => x.Length).Max();

        var report = new ReportWriter(Statistics.Load(dir), padding);

        foreach (var (filename, lineCount) in newStats.OrderByDescending(x => x.Value))
            report.Write(filename, lineCount);

        report.WriteTotal();

        newStats.Save(dir);
    }
}