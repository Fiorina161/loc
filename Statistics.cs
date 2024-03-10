namespace loc;

internal class FileStats : Dictionary<string, int>
{
    private static readonly string[] _ignoredPatterns =
        { "/bin/", "/obj/", "AssemblyInfo", ".vs", ".Designer.cs" };

    private static readonly Dictionary<string, Func<string, bool>> _langComments = new()
    {
        [".cs"] = s => s.StartsWith("/") || s.StartsWith("*"),
        [".vb"] = s => s.StartsWith("'")
    };

    public static FileStats Scan(string dir)
    {
        var newInstance = new FileStats();

        foreach (var filename in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
        {
            var fqn = Path.GetFullPath(filename);
            if (IsSource(fqn))
                newInstance[fqn] = CountLinesOfCode(fqn);
        }

        return newInstance;
    }

    public static FileStats Load(string dir)
    {
        var result = new FileStats();
        var filename = Path.Combine(dir, ".loc");

        if (File.Exists(filename))
            foreach (var line in File.ReadAllLines(filename))
            {
                var parts = line.Split("\t");
                result[parts[0]] = int.Parse(parts[1]);
            }

        return result;
    }

    public void Save(string dir)
    {
        var lines = this.Select(x => $"{x.Key}\t{x.Value}").ToArray();
        var filename = Path.Combine(dir, ".loc");
        File.WriteAllLines(filename, lines);
    }

    private static int CountLinesOfCode(string filename)
    {
        var isComment = _langComments[Path.GetExtension(filename)];

        return File.ReadAllLines(filename)
            .Select(x => x.Trim())
            .Count(x => !string.IsNullOrEmpty(x) && !isComment(x));
    }

    private static bool IsSource(string filename)
    {
        filename = filename.Replace('\\', '/');

        return !_ignoredPatterns.Any(filename.Contains) &&
                _langComments.Any(kvp => filename.EndsWith(kvp.Key));
    }
}