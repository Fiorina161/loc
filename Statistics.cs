
namespace loc;

internal class Statistics : Dictionary<string, int>
{
    private static readonly string[] _exclusions =
        { "/bin/", "/obj/", "AssemblyInfo", ".vs", ".Designer.cs" };

    // Lambda functions that determine if a line is a comment depending on file type.
    private static readonly Dictionary<string, Func<string, bool>> _commentPatterns = new()
    {
        [".cs"] = s => s.StartsWith("/") || s.StartsWith("*"),
        [".vb"] = s => s.StartsWith("'")
    };

    public static Statistics Scan(string dir)
    {
        var result = new Statistics();

        foreach (var filename in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
        {
            var fqn = Path.GetFullPath(filename);
            if (IsSource(fqn))
                result[fqn] = CountLinesOfCode(fqn);
        }

        return result;
    }

    /**
     * Returns content of the .loc file in the given directory or an empty
     * object if fo file is found.
     */
    public static Statistics Load(string directory)
    {
        var result = new Statistics();
        var filename = Path.Combine(directory, ".loc");

        if (File.Exists(filename))
            foreach (var line in File.ReadAllLines(filename))
            {
                var parts = line.Split("\t");
                result.Add(parts[0], int.Parse(parts[1]));
            }

        return result;
    }

    /**
     * Saves content of current instance in .loc file in given directory.
     */
    public void Save(string directory)
    {
        var lines = this.Select(x => $"{x.Key}\t{x.Value}").ToArray();
        var filename = Path.Combine(directory, ".loc");
        File.WriteAllLines(filename, lines);

    }

    /**
     * Returns the number of lines that are not blank and are not a comment.
     */
    private static int CountLinesOfCode(string filename)
    {
        var isComment = _commentPatterns[Path.GetExtension(filename)];

        return File.ReadAllLines(filename)
            .Select(x => x.Trim())
            .Count(x => !string.IsNullOrEmpty(x) && !isComment(x));
    }

    /**
     * Returns true if given file does not match any exclusions and matches
     * a supported file type. The filename is converted to Unix style to
     * simplify pattern matching.
     */
    private static bool IsSource(string filename)
    {
        filename = filename.Replace('\\', '/');

        return !_exclusions.Any(filename.Contains)
               && _commentPatterns.Keys.Any(filename.EndsWith);
    }
}