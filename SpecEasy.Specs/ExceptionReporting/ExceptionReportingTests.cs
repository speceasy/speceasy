using System.Linq;
using NUnit.Framework;
using SpecEasy.Specs.ExceptionReporting.SupportingExamples;

namespace SpecEasy.Specs.ExceptionReporting
{
    [TestFixture]
    public class ExceptionReportingTests
    {
        [Test]
        public void ExceptionsShouldTrumpAssertionErrors()
        {
            var result = SpecRunner.Run<BrokenClassSpec>();
            Assert.IsTrue(result.FailedTests().All(ft => ft.Message.StartsWith("System.Exception")), "At least one failed spec reported an assertion error when it should have reported an exception.");
        }
    }
}