using System;

namespace Common.Utilities.Wrappers
{
    public interface IStopWatchWrapper
    {
        TimeSpan Elapsed { get; }
        long ElapsedMilliseconds { get; }
        long ElapsedTicks { get; }
        bool IsRunning { get; }
        void Reset();
        void Restart();
        void Start();
        void Stop();
    }
}