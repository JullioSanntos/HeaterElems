using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
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
        public void StopTest()
        {
            var sut = new ProgressiveTimer();
            var isCompleted = false;
            sut.RunCompleted += (s, e) => isCompleted = true;
            Assert.IsFalse(sut.IsRunning);
            sut.Start();
            Assert.IsTrue(sut.IsRunning);
            sut.StopNow();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public async Task TicksTest()
        {
            var testStartTime = DateTime.Now;
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 500;
            var newValues = new ConcurrentQueue<double>();
            sut.PropertyChanged += (s, e) =>
            { if (e.PropertyName == nameof(sut.ProgressTick)) newValues.Enqueue(sut.ProgressTick.TotalSeconds); };
            sut.StopAfter(3000);
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any(), "ProgressTicks not received");
            var testEndTime = DateTime.Now;
            var testTimeMilliseconds = (int)(testEndTime - testStartTime).TotalMilliseconds;
            Assert.IsTrue(testTimeMilliseconds.IsInBetweenValues(sut.StopAfterMilliseconds - sut.TickIntervalMilliseconds, (int)(sut.StopAfterMilliseconds * 1.1)), $"Test time: Expected: {sut.StopAfterMilliseconds}. Actual: {testTimeMilliseconds}");
            var expectedNumberOfTicks = sut.StopAfterMilliseconds / sut.TickIntervalMilliseconds;
            var minTolerance = (int)(expectedNumberOfTicks * 0.5); // We need tolerance level because the testing threads may be occupied with other tests. Windows is not a real time OS.
            Assert.IsTrue(newValues.Count.IsInBetweenValues(minTolerance, expectedNumberOfTicks + 1), $"Expected: {expectedNumberOfTicks}, Actual: {newValues.Count}");
        }

        [Test]
        public async Task StopAtTest()
        {
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 1000;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(sut.ProgressTick)) newValues.Add(sut.ProgressTick.TotalSeconds);
            };
            sut.StopAt(DateTime.Now + new TimeSpan(0, 0, 3));
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any());
            Assert.IsTrue(newValues.Count.IsInBetweenValues(2, 3));
        }

        [Test]
        public async Task LargeRefreshRateTest()
        {
            var sut = new ProgressiveTimer();
            sut.TickIntervalMilliseconds = 300;
            sut.StopAfter(1000);
            int duration = 0;
            sut.RunCompleted += (s, dt) => duration = (int)((DateTime)dt - sut.StartTime).TotalMilliseconds;
            await sut.StartAsync();
            var durationInMilliseconds = (int)sut.ProgressTick.TotalMilliseconds;
            var expectedDuration = sut.StopAfterMilliseconds;
            var tolerance = (int)(expectedDuration * 1.25); // 25% longer tolerance when threads are busy
            Assert.That(expectedDuration.IsEqualWithinTolerance(duration, tolerance));
        }

        [Test]
        public void GetNextTickTimeWithNoEndTimeTest()
        {
            var sut = new ProgressiveTimerShunt();
            sut.DateTimeNowFunc = new Func<DateTime>(() => new DateTime(2000, 1, 1, 0, 0, 0)); // to facilitate tests we use a frozen time 
            sut.TickIntervalMilliseconds = 3000; // Time ticks every 3 seconds. 
            var nextTickSpan = sut.GetNextTickTimeMilliseconds();
            Assert.AreEqual(sut.TickIntervalMilliseconds, nextTickSpan);
        }

        [Test]
        public void GetNextTickTimeWithEarlierEndTimeTest()
        {
            var sut = new ProgressiveTimerShunt();
            sut.DateTimeNowFunc = new Func<DateTime>(() => new DateTime(2000, 1, 1, 0, 0, 0)); // to facilitate tests we use a frozen time 
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
            sut.EndTime = sut.StartTime.AddMilliseconds(runTimeMilliseconds);
            var nextTickSpan = sut.GetNextTickTimeMilliseconds();
            Assert.IsTrue(nextTickSpan.IsEqualWithinTolerance(sut.TickIntervalMilliseconds, 10));
        }

        [Test]
        public void GetDateTimeNowTest()
        {
            var sut = new ProgressiveTimerShunt();
            var expected = sut.DateTimeNowFunc();
            Assert.IsTrue(expected == DateTime.Now);
        }
    }

    public class ProgressiveTimerShunt : ProgressiveTimer
    {

        public new Func<DateTime> DateTimeNowFunc
        {
            get => base.DateTimeNowFunc;
            set => base.DateTimeNowFunc = value;
        }

        public new DateTime EndTime
        {
            get => base.EndTime;
            set => base.EndTime = value;
        }

        public new int GetNextTickTimeMilliseconds()
        {
            return base.GetNextTickTimeMilliseconds();
        }
    }

    public static class MyPrecisionExtension
    {
        public static bool IsInBetweenValues(this int testingValue, int minValue, int maxValue)
        {
            if (testingValue >= minValue && testingValue <= maxValue) return true;
            else return false;
        }
        public static bool IsEqualWithinTolerance(this int value1, int value2, int tolerance)
        {
            var difference = Math.Abs(value1 - value2);
            if (difference <= tolerance) return true;
            else return false;
        }
    }

}
