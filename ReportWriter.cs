namespace loc;

internal class ReportWriter
{
    private readonly FileStats _previousStats;
    private readonly int _padding;
    private int _totalValue;
    private int _totalDelta;


    public ReportWriter(FileStats previousStats, int padding)
    {
        _previousStats = previousStats;
        _padding = padding;
    }

    public void WriteLine(string label, int currentValue)
    {
        var previousValue = _previousStats.GetValueOrDefault(label, 0);
        var delta = currentValue - previousValue;

        _totalValue += currentValue;
        _totalDelta += delta;

        Console.Write($"{label.PadRight(_padding)} {currentValue,5}");

        SetDeltaColor(delta);

        if (previousValue == 0)
            Console.WriteLine(" *new*");
        else if (currentValue != previousValue)
            Console.WriteLine($" {delta:+#;-#}");
        else
            Console.WriteLine();

        Console.ResetColor();
    }

    public void WriteTotal()
    {
        Console.Write($"{"Total".PadLeft(_padding)} {_totalValue,5}");

        SetDeltaColor(_totalDelta);

        if (_totalDelta != 0)
            Console.WriteLine($" {_totalDelta:+#;-#}");
        else
            Console.WriteLine();

        Console.ResetColor();
    }

    private static void SetDeltaColor(int delta)
    {
        if (delta > 0)
            Console.ForegroundColor = ConsoleColor.Red;
        else if (delta < 0)
            Console.ForegroundColor = ConsoleColor.Green;
        else
            Console.ResetColor();
    }
}