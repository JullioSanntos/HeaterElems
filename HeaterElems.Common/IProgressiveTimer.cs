using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeaterElems.Common
{
    public interface IProgressiveTimer
    {
        /// <summary>
        /// Event is raised when <see cref="ProgressiveTimer.EndTime"/> time is reached
        /// </summary>
        event EventHandler<TimeSpan?> RunCompleted;

        /// <summary>
        /// Event is raise every <see cref="ProgressiveTimer.TickIntervalMilliseconds"/>
        /// 
        /// </summary>
        event EventHandler<Tuple<TimeSpan, CancellationToken>> Tick;

        ProgressiveTimerStateEnum TimerState { get; }

        List<TimeSpan> RunningTimeSegments { get; }

        /// <summary>
        /// It returns the total time that this Timer has been running.
        /// If this Timer is running the total running time keeps increasing.
        /// If this Timer has stopped or paused, this property returns the timespan between
        /// a Start (<see cref="Start"/> or <see cref="StartAsync"/>)
        /// and a Stop (<see cref="StopAt"/> or <see cref="StopAfter"/>).
        /// If Pause was invoked (<see cref="Pause"/>, the Pause time interval is not counted by
        /// accumulating only the Running Time Segments in this property.
        /// </summary>
        TimeSpan TotalRunningTime { get; }

        /// <summary>
        /// Time when <see cref="Start"/> or <see cref="StartAsync"/> was invoked.
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Time at which this Timer will stop running and will stop raising <see cref="Tick"/> events.
        /// This property is setup by calling <see cref="StopAt"/> or <see cref="StopAfter"/>.
        /// This property is also adjusted on a re-start (<see cref="Start"/>) after a <see cref="Pause"/>.
        /// </summary>
        DateTime EndTime { get; }

        DateTime? StopAtDateTime { get; }

        /// <summary>
        /// How long the clock should run in milliseconds.
        /// This property is set by <see cref="StopAfter"/> and is used in the calculation of <see cref="EndTime"/>
        /// </summary>
        int StopAfterMilliseconds { get; }

        /// <summary>
        /// Indicates how frequently <see cref="Tick"/> event should be raised.
        /// Minimum Tick interval is 100 milliseconds.
        /// </summary>
        int TickIntervalMilliseconds { get; set; }

        /// <summary>
        /// Indicates if a run was cancelled.
        /// Its state is changed by invoking <see cref="Cancel"/> only.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Calls StartAsync without waiting for a response
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the clock in which progress is indicated by raising <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the property <see cref="ProgressiveTimer.TotalRunningTime"/>
        /// The event is raised as often as determined by <see cref="ProgressiveTimer.TickIntervalMilliseconds"/> in milliseconds.
        /// This clock will have a hard stop when elapsed time indicated by <see cref="ProgressiveTimer.DefaultMaxDurationMilliseconds"/> or when time in <see cref="ProgressiveTimer.EndTime"/> is reached.
        /// This awaitable method returns when the clock is stopped.
        /// <example>
        /// <code>
        ///     var sut = new ProgressiveTimer();
        ///     var isCompleted = false;;
        ///     sut.RunCompleted += (s, e) => isCompleted = true;
        ///     sut.StopAfter(1000);
        ///     await sut.StartAsync();
        ///     Assert.IsTrue(isCompleted);
        /// </code>
        /// </example>
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// Immediately stops the clock
        /// </summary>
        void Cancel();

        /// <summary>
        ///     Stops the current ProgressiveTimer's run after passed milliseconds
        /// </summary>
        /// <param name="stopAfterMilliseconds"></param>
        void StopAfter(int stopAfterMilliseconds);

        /// <summary>
        ///     Stops the current ProgressiveTimer's run at a future time
        /// </summary>
        /// <param name="endTIme"></param>
        void StopAt(DateTime endTIme);

        void Pause();
    }

}
