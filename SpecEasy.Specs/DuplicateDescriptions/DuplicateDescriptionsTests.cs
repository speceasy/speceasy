using System.Linq;
using NUnit.Framework;
using SpecEasy.Specs.DuplicateDescriptions.SupportingExamples;

namespace SpecEasy.Specs.DuplicateDescriptions
{
    [TestFixture]
    public class DuplicateDescriptionsTests
    {
        [Test]
        public void ThrowSpecificExceptionForDuplicateGivenDescriptions()
        {
            var result = SpecRunner.Run<DuplicateDescriptionsSpec>();
            Assert.Greater(result.FailedTests().Count(), 0, "Test cases should have failed");
            Assert.IsTrue(result.FailedTests().All(ft => ft.Message.StartsWith(typeof(DuplicateDescriptionException).FullName)), "Every failure should have reported an exception specific to a description having been repeated");
            Assert.IsTrue(result.FailedTests().All(ft => ft.StackTrace.Contains(typeof(DuplicateDescriptionsSpec).FullName)), "Every failure's stack trace should include a line number from the spec that failed");
        }
    }
}
