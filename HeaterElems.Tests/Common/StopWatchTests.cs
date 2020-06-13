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
        public void InstantiationTest()
        {
            var sut = new StopWatch();
            Assert.IsNotNull(sut);
        }

        [Test]
        public async Task StopAterTest()
        {
            var sut = new StopWatch();
            var isCompleted = false;
            sut.Completed += (s,e) => isCompleted = true;
            sut.StopAfter(1000);
            await sut.Start();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public async Task StopTest()
        {
            var sut = new StopWatch();
            var isCompleted = false;
            sut.Completed += (s, e) => isCompleted = true;
            await sut.Start();
            sut.Stop();
            Assert.IsTrue(isCompleted);
        }

        [Test]
        public async Task ElapsedTotalSecondsTest()
        {
            var sut = new StopWatch();
            sut.RefreshRateInMilliseconds = 1000;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => { if(e.PropertyName == nameof(sut.ElapsedTotalSeconds)) newValues.Add(sut.ElapsedTotalSeconds); };
            sut.StopAfter(3000);
            await sut.Start();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(3, newValues.Count);
        }

        [Test]
        public async Task CancelTest()
        {
            var sut = new StopWatch();
            sut.RefreshRateInMilliseconds = 500;
            var newValues = new List<double>();
            sut.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(sut.ElapsedTotalSeconds)) newValues.Add(sut.ElapsedTotalSeconds); };
            sut.StopAfter(3000);
#pragma warning disable 4014
            sut.Start();
#pragma warning restore 4014
            var cancelDelay = 1000;
            await Task.Delay(1000);
            sut.Cancel();
            Assert.IsTrue(newValues.Any());
            Assert.AreEqual(cancelDelay/sut.RefreshRateInMilliseconds, newValues.Count);
        }
    }
}
