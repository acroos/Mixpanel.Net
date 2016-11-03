using System;
using Mixpanel.NET.PCL.Converters;
using NUnit.Framework;

namespace Mixpanel.NET.PCL.UnitTests.Converters
{
    [TestFixture]
    public class PropertyNameConverterTests
    {
        string expectedPropertyName = "Property Name 1";

        [Test]
        [TestCase("propertyName1")]
        [TestCase("PropertyName1")]
        [TestCase("_propertyName1")]
        public void SuccessfulConvertTest(string propertyName)
        {
            var actual = PropertyNameConverter.ToSpacedString (propertyName);

            Assert.AreEqual (expectedPropertyName, actual);
        }
    }
}
