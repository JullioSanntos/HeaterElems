using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HeaterElems.Tests.Common
{
    [TestFixture()]
    
    public class StopWatchTests
    {
        [Test]
        public void InstantiationTest() {
            var sut = new StopWatch();
            Assert.IsNotNull(sut);
        }

        [Test]
        public async Task StopAterTest() {
            var sut = new StopWatch();
            var isCompleted = false;
            sut.Completed += (s, e) => isCompleted = true;
            sut.StopAfter(1000);
            await sut.StartAsync();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public async Task StopTest() {
            var sut = new StopWatch();
            var isCompleted = false;
            sut.Completed += (s, e) => isCompleted = true;
            await sut.StartAsync();
            sut.Stop();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public async Task ProgressTest() {
            var sut = new StopWatch();
            sut.RefreshFrequencyInMilliseconds = 1000;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(sut.RunDuration)) newValues.Add(sut.RunDuration.TotalSeconds);
            };
            sut.StopAfter(3000);
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(3, newValues.Count);
        }



        [Test]
        public async Task StopAtTest() {
            var sut = new StopWatch();
            sut.RefreshFrequencyInMilliseconds = 1000;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(sut.RunDuration)) newValues.Add(sut.RunDuration.TotalSeconds);
            };
            sut.StopAt(DateTime.Now + new TimeSpan(0, 0, 3)); //Stop in three seconds from now
            await sut.StartAsync();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(3, newValues.Count);
        }

        [Test]
        public async Task CancelTest() {
            var sut = new StopWatch();
            sut.RefreshFrequencyInMilliseconds = 500;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(sut.RunDuration)) newValues.Add(sut.RunDuration.TotalSeconds);
            };
            sut.StopAfter(3000);
#pragma warning disable 4014
            sut.StartAsync();
#pragma warning restore 4014
            var cancelDelay = 1000;
            await Task.Delay(1000);
            sut.Cancel();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(cancelDelay / sut.RefreshFrequencyInMilliseconds, newValues.Count);
        }


        [Test]
        public async Task LargeRefreshRateTest()
        {
            var sut = new StopWatch();
            sut.RefreshFrequencyInMilliseconds = 300;
            sut.StopAfter(1000);
            await sut.StartAsync();
            var durationInMilliSeconds = (int)sut.RunDuration.TotalMilliseconds;
            Assert.That((300 * 4).IsCloseTo(durationInMilliSeconds,200));
        }

        [Test]
        public void GetAdjustedStopTimeTestWithEarlierEndTime() {
            var sut = new StopWatch();
            var endTime = DateTime.Now.AddMilliseconds(2000);
            var refreshRate = 3000;
            var newEndTime = sut.GetAdjustedStopTime(endTime, refreshRate);
            Assert.AreEqual(endTime, newEndTime);
        }

        [Test]
        public void GetAdjustedStopTimeTestWithLaterEndTime() {
            var sut = new StopWatch();
            var dateTimeNowFrozen = DateTime.Now;
            sut.DateTimeNowFunc = () => dateTimeNowFrozen;
            var endTime = dateTimeNowFrozen.AddMilliseconds(4000);
            var refreshRate = 3000;
            var newEndTime = sut.GetAdjustedStopTime(endTime, refreshRate);
            var expectedTime = dateTimeNowFrozen.AddMilliseconds(refreshRate);
            Assert.AreEqual(expectedTime, newEndTime);
        }
    }

    public static class MyMath {
        public static bool IsCloseTo(this int value1, int value2, int maxDifference) {
            var difference = Math.Abs(value1 - value2);
            if (difference <= maxDifference) return true;
            else return false;
        }
    }
}
