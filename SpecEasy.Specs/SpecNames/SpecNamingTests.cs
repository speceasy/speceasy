using System.Linq;
using NUnit.Framework;
using SpecEasy.Specs.SpecNames.SupportingExamples;

namespace SpecEasy.Specs.SpecNames
{
    [TestFixture]
    public class SpecNamingTests
    {
        [Test]
        public void RunSpecsAndCheckNames()
        {
            var result = SpecRunner.Run<SpecNamingSpec>();
            var allTests = result.AllTests();
            Assert.IsTrue(allTests.All(t => t.Name == SpecNamingSpec.ExpectedSpecName));
        }
    }
}
