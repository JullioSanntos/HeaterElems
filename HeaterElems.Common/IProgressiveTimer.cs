using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeaterElems.Common
{
    public interface IProgressiveTimer
    {
        /// <summary>
        /// Event is raised when <see cref="ProgressiveTimer.EndTime"/> time is reached, unless it was cancelled by invoking <see cref="ProgressiveTimer.Cancel"/>
        /// This event is invoked synchronously.
        /// </summary>
        event EventHandler<TimeSpan?> RunCompleted;

        /// <summary>
        /// Event is raised every <see cref="ProgressiveTimer.TickIntervalMilliseconds"/>, for as long as this Timer is "Active"
        /// This event is invoked asynchronously on another thread than the one that started this Timer.
        /// This event is cancellable by invoking <see cref="ProgressiveTimer.Cancel"/>
        /// All subscribers receive the <see cref="ProgressiveTimer.CancellationToken"/> to, cooperatively and graciously, handle a cancellation.
        /// </summary>
        event EventHandler<Tuple<TimeSpan, CancellationToken>> Tick;

        /// <summary>
        /// <see cref="ProgressiveTimerStateEnum"/>
        /// "Idle" is the state before this Timer is started (<see cref="Start"/> or <see cref="StartAsync"/> is invoked) or
        ///     after the <see cref="RunCompleted"/> event returns control to this Timer.
        /// "Active" Indicates that the timer has started (<see cref="StartTime"/> or <see cref="Start"/> was invoked).
        /// "Paused"  indicates that <see cref="Pause"/> was invoked
        /// "Cancelled" indicates that <see cref="Cancel"/> was invoked
        /// "Completed" means that the EndTime was reached and its scope is just for the duration of the <see cref="RunCompleted"/> event.
        /// </summary>
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
        TimeSpan TotalRunningTime {
            get;
            // to be used only on automated tests
        }

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
        /// Indicates how frequently the <see cref="Tick"/> event is raised
        /// Minimum Tick interval is 100 milliseconds.
        /// </summary>
        int TickIntervalMilliseconds { get; set; }

        /// <summary>
        /// Indicates if a run was cancelled.
        /// Its state is changed by <see cref="Cancel"/> only.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Calls <see cref="ProgressiveTimer.StartAsync"/> without waiting for a response.
        /// Please, see <see cref="ProgressiveTimer.StartAsync"/> documentation.
        /// <example>
        /// <code>
        ///     var sut = new ProgressiveTimer();
        ///     var isCompleted = false;;
        ///     sut.RunCompleted += (s, e) => isCompleted = true;
        ///     sut.Start(); 
        ///     sut.StopAfter(1000);
        /// </code>
        /// </example>
        /// </summary>        
        void Start();

        /// <summary>
        /// 
        /// Starts a timer in which progress is indicated by raising the <see cref="ProgressiveTimer.Tick"/> event
        /// The event is raised as often as determined by <see cref="ProgressiveTimer.TickIntervalMilliseconds"/> in milliseconds.
        /// This timer will have a hard stop when elapsed time indicated by <see cref="ProgressiveTimer.EndTime"/> is reached.
        /// This awaitable <see cref="ProgressiveTimer.StartAsync"/>method returns when the Timer is stopped because <see cref="ProgressiveTimer.EndTime"/> was reached or
        ///     <see cref="ProgressiveTimer.Cancel"/> was invoked
        /// When the times stops the <see cref="ProgressiveTimer.RunCompleted"/> is raised
        /// <example>
        /// <code>
        ///     var sut = new ProgressiveTimer();
        ///     var isCompleted = false;;
        ///     sut.RunCompleted += (s, e) => isCompleted = true;
        ///     sut.StopAfter(1000);
        ///     await sut.StartAsync();
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

        /// <summary>
        /// Pause the time count and stop the <see cref="Tick"/> event from being raised.
        /// Use <see cref="Start"/> or <see cref="StartAsync"/> to resume the time count.
        /// </summary>
        void Pause();
    }
}