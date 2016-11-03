using Mixpanel.NET.PCL.Converters;
using Mixpanel.NET.PCL.Attributes;
using NUnit.Framework;

namespace Mixpanel.NET.PCL.UnitTests.Converters
{
    [TestFixture]
    public class MixpanelEventConverterTests
    {
        [Test]
        public void ConvertSimpleEventTest ()
        {
            var evt = new SampleMixpanelEvent ();
            var dict = MixpanelEventConverter.ToDictionary (evt);

            Assert.AreEqual (5, dict.Count);

            Assert.IsTrue (dict.ContainsKey ("Int Property"));
            Assert.AreEqual (100, dict ["Int Property"]);

            Assert.IsTrue (dict.ContainsKey ("Float Property"));
            Assert.AreEqual (1.234f, dict ["Float Property"]);

            Assert.IsTrue (dict.ContainsKey ("the bool property"));
            Assert.AreEqual (false, dict ["the bool property"]);

            Assert.IsTrue (dict.ContainsKey ("double field"));
            Assert.AreEqual (2.0d, dict ["double field"]);

            Assert.IsTrue (dict.ContainsKey ("Long Field"));
            Assert.AreEqual (100L, dict ["Long Field"]);
        }
    }

    class SampleMixpanelEvent : IMixpanelEvent
    {
        public string EventName => "Sample";

        [MixpanelProperty]
        public int IntProperty => 100;

        [MixpanelIgnore]
        public string StringProperty => "simple string";

        [MixpanelProperty("the bool property")]
        public bool BoolProperty => false;

        public float FloatProperty => 1.234f;

        private int _intField = 2;

        [MixpanelProperty("double field")]
        private double _doubleField = 2.0d;

        [MixpanelProperty]
        private long _longField = 100L;
    }
}
