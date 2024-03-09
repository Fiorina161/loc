namespace loc;

internal class ReportWriter
{
    private readonly Statistics _oldStats;
    private readonly int _padding;
    private int _totalValue;
    private int _totalDelta;


    public ReportWriter(Statistics oldStats, int padding)
    {
        _oldStats = oldStats;
        _padding = padding;
    }

    public void Write(string filename, int value)
    {
        var oldValue = _oldStats.GetValueOrDefault(filename, 0);
        var delta = value - oldValue;

        _totalValue += value;
        _totalDelta += delta;

        Console.Write($"{filename.PadRight(_padding)} {value,5}");

        SetDeltaColor(delta);

        if (oldValue == 0)
            Console.WriteLine(" *new file*");
        else if (value != oldValue)
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

    private void SetDeltaColor(int delta)
    {
        if (delta > 0)
            Console.ForegroundColor = ConsoleColor.Red;
        else if (delta < 0)
            Console.ForegroundColor = ConsoleColor.Green;
    }
}