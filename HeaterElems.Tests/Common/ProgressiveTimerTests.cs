using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
// ReSharper disable UseObjectOrCollectionInitializer

namespace HeaterElems.Tests.Common
{
    [TestFixture()]
    
    public class ProgressiveTimerTests
    {
        [Test]
        public void InstantiationTest() {
            var sut = new ProgressiveTimer();
            Assert.IsNotNull(sut);
        }

        [Test]
        public async Task StopAterTest() {
            var sut = new ProgressiveTimer();
            var isCompleted = false;
            sut.RunCompleted += (s, e) => isCompleted = true;
            sut.StopAfter(1000);
            await sut.StartAsync();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public void StopTest() {
            var sut = new ProgressiveTimer();
            var isCompleted = false;
            sut.RunCompleted += (s, e) => isCompleted = true;
            sut.Start();
            sut.Stop();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public async Task TicksTest() {
            var sut = new ProgressiveTimer();
            sut.TickFrequencyMilliseconds = 1000;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(sut.RunProgress)) newValues.Add(sut.RunProgress.TotalSeconds);
            };
            sut.StopAfter(3000);
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(3, newValues.Count);
        }



        [Test]
        public async Task StopAtTest() {
            var sut = new ProgressiveTimer();
            sut.TickFrequencyMilliseconds = 1000;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(sut.RunProgress)) newValues.Add(sut.RunProgress.TotalSeconds);
            };
            sut.StopAt(DateTime.Now + new TimeSpan(0, 0, 3)); 
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(3, newValues.Count);
        }

//        [Test]
//        public async Task CancelTest() {
//            var sut = new ProgressiveTimer();
//            sut.TickFrequencyMilliseconds = 500;
//            var newValues = new List<double>();
//            sut.PropertyChanged += (s, e) => {
//                if (e.PropertyName == nameof(sut.RunProgress)) newValues.Add(sut.RunProgress.TotalSeconds);
//            };
//            sut.StopAfter(3000);
//#pragma warning disable 4014
//            sut.StartAsync();
//#pragma warning restore 4014
//            var cancelDelay = 1000;
//            await Task.Delay(1000);
//            sut.Cancel();
//            Assert.IsTrue(newValues.Any());
//            Assert.AreEqual(cancelDelay / sut.TickFrequencyMilliseconds, newValues.Count);
//        }


        [Test]
        public async Task LargeRefreshRateTest()
        {
            var sut = new ProgressiveTimer();
            sut.TickFrequencyMilliseconds = 300;
            sut.StopAfter(1000);
            await sut.StartAsync();
            var durationInMilliSeconds = (int)sut.RunProgress.TotalMilliseconds;
            Assert.That((300 * 4).IsCloseTo(durationInMilliSeconds,200));
        }

        [Test]
        public void GetAdjustedStopTimeTestWithEarlierEndTime() {
            var sut = new ProgressiveTimer();
            var endTime = DateTime.Now.AddMilliseconds(2000);
            var refreshRate = 3000;
            var newEndTime = sut.GetAdjustedStopTime(endTime, refreshRate);
            Assert.AreEqual(endTime, newEndTime);
        }

        [Test]
        public void GetAdjustedStopTimeTestWithLaterEndTime() {
            var sut = new ProgressiveTimer();
            var dateTimeNowFrozen = DateTime.Now;
            sut.DateTimeNowFunc = () => dateTimeNowFrozen;
            var endTime = dateTimeNowFrozen.AddMilliseconds(4000);
            var refreshRate = 3000;
            var newEndTime = sut.GetAdjustedStopTime(endTime, refreshRate);
            var expectedTime = dateTimeNowFrozen.AddMilliseconds(refreshRate);
            Assert.AreEqual(expectedTime, newEndTime);
        }

        [Test]
        public void GetNextTickTimeMillisecondsTest1()
        {
            var sut = new ProgressiveTimer();
            var dateTimeNowFrozen = new DateTime(1, 1, 1, 0, 0, 5); // = 5,000 Milliseconds
            sut.DateTimeNowFunc = () => dateTimeNowFrozen;
            var startTime = new DateTime(1, 1, 1, 0, 0, 1); // = 1,000 Milliseconds;
            sut.TickFrequencyMilliseconds = 3000; // ticksSeconds = 4 (= 1 + 3), 7 (= 1 + 2 * 3), 10 (= 1 + 3 * 3), etc...
            var expectedNextTick = 2000; // the wait time until 7,000 from 5,000 Milliseconds
            var actualNextTick = sut.GetNextTickTimeMilliseconds(startTime, sut.TickFrequencyMilliseconds);
            Assert.AreEqual(expectedNextTick, actualNextTick);
        }
    }

    public static class MyPrecisionExtension {
        public static bool IsCloseTo(this int value1, int value2, int maxDifference) {
            var difference = Math.Abs(value1 - value2);
            if (difference <= maxDifference) return true;
            else return false;
        }
    }
}
