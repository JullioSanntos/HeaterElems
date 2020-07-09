using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using HeaterElems.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
// ReSharper disable UseObjectOrCollectionInitializer

//namespace Common.Utilities_UnitTests
namespace HeaterElems.Tests.Common
{
    [TestFixture()]
    public class ProgressiveTimerTests
    {
        [Test]
        public void InstantiationTest()
        {
            var sut = new ProgressiveTimer();
            Assert.IsNotNull(sut);
        }

        [Test]
        public async Task StopAfterTest()
        {
            var sut = new ProgressiveTimer();
            var isCompleted = false;
            sut.RunCompleted += (s, e) => isCompleted = true;
            sut.StopAfter(1000);
            await sut.StartAsync();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public void EndTimeTest()
        {
            var sut = new ProgressiveTimer();
            Assert.IsTrue((sut.EndTime - DateTime.Now).TotalMilliseconds <= ProgressiveTimer.DEFAULT_MAX_DURATION_MILLISECONDS);
        }

        [Test]
        public void EndTimeAWhenStopAtInvokedTest()
        {
            var sut = new ProgressiveTimer();
            var runningTime = 5000;
            sut.StopAt(DateTime.Now.AddMilliseconds(runningTime));
            Assert.IsTrue((sut.EndTime - DateTime.Now).TotalMilliseconds <= runningTime);
        }

        [Test]
        public void EndTimeAWhenStopAfterAndStartAreInvokedTest()
        {
            var sut = new ProgressiveTimer();
            var runningTime = 5000;
            sut.Start(); // A StartTime is required to compute the EndTime
            sut.StopAfter(runningTime);
            var actual = (sut.EndTime - DateTime.Now).TotalMilliseconds;
            Assert.IsTrue(actual <= runningTime);
        }

        [Test]
        public void EndTimeAWhenStopAfterInvokedTest()
        {
            var sut = new ProgressiveTimer();
            var runningTime = 5000;
            sut.StopAfter(runningTime);
            // Without StartTime, with Just the StopAfter, the EndTime can not be computed.
            // It gets just the DateTime's MinValue instead
            Assert.AreEqual(DateTime.MinValue, sut.EndTime);
        }

        [Test]
        public void CancelTest()
        {
            var sut = new ProgressiveTimer();
            var isCompleted = false;
            sut.RunCompleted += (s, e) => isCompleted = true;
            Assert.IsFalse(sut.IsActive);
            sut.Start();
            Assert.IsTrue(sut.IsActive);
            sut.Cancel();
            Assert.IsFalse(isCompleted);
        }

        [Test]
        public async Task TicksTest()
        {
            var testStartTime = DateTime.Now;
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 500;
            var newValues = new ConcurrentQueue<double>();
            sut.Tick += (s, e) =>
            { newValues.Enqueue(sut.TotalRunningTime.TotalSeconds); };
            sut.StopAfter(3000);
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any(), "ProgressTicks not received");
            var testEndTime = DateTime.Now;
            var testTimeMilliseconds = (int)(testEndTime - testStartTime).TotalMilliseconds;
            Assert.That(testTimeMilliseconds, Is.GreaterThanOrEqualTo(sut.StopAfterMilliseconds - sut.TickIntervalMilliseconds));
            Assert.That(testTimeMilliseconds, Is.LessThanOrEqualTo((int)(sut.StopAfterMilliseconds * 1.3)));
            var expectedNumberOfTicks = sut.StopAfterMilliseconds / sut.TickIntervalMilliseconds;
            var minTolerance = (int)(expectedNumberOfTicks * 0.5); // We need tolerance level because the testing threads may be occupied with other tests. Windows is not a real time OS.
            Assert.That(newValues.Count, Is.GreaterThanOrEqualTo(minTolerance));
            Assert.That(newValues.Count, Is.LessThanOrEqualTo(expectedNumberOfTicks + 1));
        }

        [Test]
        public async Task StopAtTest()
        {
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 1000;
            var newValues = new List<double>();
            sut.Tick += (s, e) => {
                newValues.Add(sut.TotalRunningTime.TotalSeconds);
            };
            sut.StopAt(DateTime.Now + new TimeSpan(0, 0, 3));
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any());
            Assert.That(newValues.Count, Is.EqualTo(3).Within(1));
        }

        [Test]
        public async Task LargeRefreshRateTest()
        {
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 300;
            sut.StopAfter(1000);
            int duration = 0;
            sut.RunCompleted += (s, timerDuration) => duration = (int)(timerDuration?.TotalMilliseconds ?? 0);
            await sut.StartAsync();
            var durationInMilliseconds = (int)sut.TotalRunningTime.TotalMilliseconds;
            var expectedDuration = sut.StopAfterMilliseconds;
            var tolerance = (int)(expectedDuration * 1.25); // 25% longer tolerance when threads are busy
            Assert.That(expectedDuration, Is.EqualTo(duration).Within(tolerance));
        }

        [Test]
        public void GetNextTickTimeWithNoEndTimeTest()
        {
            var sut = new ProgressiveTimerShunt();
            sut.DateTimeNowFunc = new Func<DateTime>(() => new DateTime(2000, 1, 1, 0, 0, 0)); // to facilitate tests we use a frozen time 
            sut.StartTime = sut.DateTimeNowFunc();
            sut.TickIntervalMilliseconds = 3000; // Time ticks every 3 seconds. 
            var nextTickSpan = sut.GetNextTickTimeMilliseconds();
            Assert.AreEqual(sut.TickIntervalMilliseconds, nextTickSpan);
        }

        [Test]
        public void GetNextTickTimeWithEarlierEndTimeTest()
        {
            var sut = new ProgressiveTimerShunt();
            sut.DateTimeNowFunc = new Func<DateTime>(() => new DateTime(2000, 1, 1, 0, 0, 0)); // to facilitate tests we use a frozen time 
            sut.StartTime = sut.DateTimeNowFunc();
            sut.TickIntervalMilliseconds = 3000; // Time ticks every 3 seconds. 
            var runTimeMilliseconds = 1000;
            sut.EndTime = sut.StartTime.AddMilliseconds(runTimeMilliseconds);
            var nextTickSpan = sut.GetNextTickTimeMilliseconds();
            Assert.AreEqual(runTimeMilliseconds, nextTickSpan);
        }

        [Test]
        public void GetNextTickTimeWithLaterEndTimeTest()
        {
            var sut = new ProgressiveTimerShunt();
            sut.DateTimeNowFunc = new Func<DateTime>(() => new DateTime(2000, 1, 1, 0, 0, 0)); // to facilitate tests we use a frozen time 
            sut.StartTime = sut.DateTimeNowFunc();
            sut.TickIntervalMilliseconds = 3000; // Time ticks every 3 seconds. 
            var runTimeMilliseconds = 10000;
            sut.EndTime = sut.StartTime.AddMilliseconds(runTimeMilliseconds);
            var nextTickSpan = sut.GetNextTickTimeMilliseconds();
            Assert.AreEqual(sut.TickIntervalMilliseconds, nextTickSpan);
        }

        [Test]
        public void GetNextTickTimeWithLaterEndTimeTestUsingCurrentTimeTest()
        {
            var sut = new ProgressiveTimerShunt();
            sut.TickIntervalMilliseconds = 3000; // Time ticks every 3 seconds. 
            var runTimeMilliseconds = 10000;
            sut.StartTime = sut.DateTimeNowFunc();
            sut.EndTime = sut.StartTime.AddMilliseconds(runTimeMilliseconds);
            var nextTickSpan = sut.GetNextTickTimeMilliseconds();
            Assert.That(nextTickSpan, Is.EqualTo(sut.TickIntervalMilliseconds).Within(10));
        }

        [Test]
        public void GetDateTimeNowTest()
        {
            var sut = new ProgressiveTimerShunt();
            var expected = sut.DateTimeNowFunc();
            Assert.IsTrue(expected == DateTime.Now);
        }

        [Test]
        public async Task RunningTimerWhileRunningTest()
        {
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 200;
            var tickTimes = new List<int>();
            sut.Tick += (s, tickTuple) => tickTimes.Add((int)tickTuple.Item1.TotalMilliseconds);
            sut.StopAfter(1000);
            await sut.StartAsync();
            Assert.IsTrue(tickTimes.Count >= 4);
            await Task.Delay(600);
            Assert.IsTrue(tickTimes.Count >= 4); // this confirms that the Timer stopped
        }

        [Test]
        public async Task PauseTest()
        {
            var sut = new ProgressiveTimer();
            var runningTime = 3000;
            sut.StopAfter(runningTime);
            sut.Start();

            var expectedRunTimeMillisecond = 1000;
            await Task.Delay(expectedRunTimeMillisecond);
            sut.Pause();

            var actualRunTimeMillisecond = (int)sut.TotalRunningTime.TotalMilliseconds;
            Assert.That(actualRunTimeMillisecond, Is.EqualTo(expectedRunTimeMillisecond).Within(500));

            await Task.Delay(1000);

            await sut.StartAsync(); //re-start
            actualRunTimeMillisecond = (int)sut.TotalRunningTime.TotalMilliseconds;
            Assert.That(actualRunTimeMillisecond, Is.EqualTo(runningTime).Within(500));
        }

        [Test]
        public async Task SubsequentTimerCallsTest()
        {
            var sut = new ProgressiveTimerShunt();
            var timerRunMilliseconds = 1000;
            var startTime = DateTime.Now;
            
            sut.StopAfter(timerRunMilliseconds);
            await sut.StartAsync();
            sut.StopAfter(timerRunMilliseconds);
            await sut.StartAsync();

            var actualTestTime = DateTime.Now - startTime;
            var expectedTestTimeMilliseconds = 2 * timerRunMilliseconds;
            Assert.That(actualTestTime.TotalMilliseconds, Is.EqualTo(expectedTestTimeMilliseconds).Within(expectedTestTimeMilliseconds * 0.25));

        }
    }

    #region Shunt
    public class ProgressiveTimerShunt : ProgressiveTimer
    {

        public new Func<DateTime> DateTimeNowFunc
        {
            get { return base.DateTimeNowFunc; }
            set { base.DateTimeNowFunc = value; }
        }

        public new DateTime StartTime
        {
            get { return base.StartTime; }
            set { base.StartTime = value; }
        }

        public new DateTime EndTime
        {
            get { return base.EndTime; }
            set { base.EndTime = value; }
        }

        public new int GetNextTickTimeMilliseconds()
        {
            return base.GetNextTickTimeMilliseconds();
        }
    }
    #endregion Shunt

}
