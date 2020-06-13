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
            var target = new StopWatch();
            Assert.IsNotNull(target);
        }

        
    }
}
