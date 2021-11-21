using System;
using System.Timers;

public class Delay
{
    public static void Do(int after, Action action)
    {
        if (after <= 0 || action == null) return;

        var timer = new Timer { Interval = after, Enabled = false };

        timer.Elapsed += (sender, e) =>
        {
            timer.Stop();
            action.Invoke();
            timer.Dispose();
            GC.SuppressFinalize(timer);
        };

        timer.Start();
    }
}