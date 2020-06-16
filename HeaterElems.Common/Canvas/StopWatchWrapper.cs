using System;
using System.Diagnostics;

namespace Common.Utilities.Wrappers
{
    public class StopWatchWrapper : IStopWatchWrapper
    {
        private readonly Stopwatch _stopWatch;

        public StopWatchWrapper()
        {
            _stopWatch = new Stopwatch();
        }

        //
        // Summary:
        //     Gets the total elapsed time measured by the current instance.
        //
        // Returns:
        //     A read-only System.TimeSpan representing the total elapsed time measured by the
        //     current instance.
        public TimeSpan Elapsed => _stopWatch.Elapsed;

        //
        // Summary:
        //     Gets the total elapsed time measured by the current instance, in milliseconds.
        //
        // Returns:
        //     A read-only long integer representing the total number of milliseconds measured
        //     by the current instance.
        public long ElapsedMilliseconds => _stopWatch.ElapsedMilliseconds;

        //
        // Summary:
        //     Gets the total elapsed time measured by the current instance, in timer ticks.
        //
        // Returns:
        //     A read-only long integer representing the total number of timer ticks measured
        //     by the current instance.
        public long ElapsedTicks => _stopWatch.ElapsedTicks;

        //
        // Summary:
        //     Gets a value indicating whether the System.Diagnostics.Stopwatch timer is running.
        //
        // Returns:
        //     true if the System.Diagnostics.Stopwatch instance is currently running and measuring
        //     elapsed time for an interval; otherwise, false.
        public bool IsRunning => _stopWatch.IsRunning;

        //
        // Summary:
        //     Stops time interval measurement and resets the elapsed time to zero.
        public void Reset() => _stopWatch.Reset();

        //
        // Summary:
        //     Stops time interval measurement, resets the elapsed time to zero, and starts
        //     measuring elapsed time.
        public void Restart() => _stopWatch.Restart();

        //
        // Summary:
        //     Starts, or resumes, measuring elapsed time for an interval.
        public void Start() => _stopWatch.Start();

        //
        // Summary:
        //     Stops measuring elapsed time for an interval.
        public void Stop() => _stopWatch.Stop();
    }
}
