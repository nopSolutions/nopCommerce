using System.Diagnostics;

namespace Nop.Tests;

public static class TestHelper
{
    public static void ProfileAction(Action action)
    {
        var sw = new Stopwatch();
        var memory = GC.GetTotalMemory(true) / 1024.0 / 1024.0;
        sw.Start();

        action.Invoke();

        sw.Stop();
        var delta = GC.GetTotalMemory(true) / 1024.0 / 1024.0 - memory;
        Console.WriteLine("Elapsed time: {0:F} s", sw.ElapsedMilliseconds / 1000.0);
        Console.WriteLine("Memory usage: {0:F} MB", delta);
    }
}
