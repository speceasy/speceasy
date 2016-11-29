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
            Assert.Greater(result.FailedTests().Count(), 0, "Test cases should have failed");
            Assert.IsTrue(result.FailedTests().All(ft => ft.Message.StartsWith("System.ArgumentOutOfRangeException")), "At least one failed spec reported an assertion error when it should have reported an exception.");
        }

        [Test]
        public void ExpectedExceptionDifferentTypeShouldFail()
        {
            var result = SpecRunner.Run<ExpectedExceptionDifferentTypeSpec>();
            Assert.Greater(result.FailedTests().Count(), 0, "Test cases should have failed");
            Assert.IsTrue(result.FailedTests().All(ft =>
                ft.Message.StartsWith("System.Exception") &&
                ft.Message.Contains("of type System.InvalidOperationException was not thrown") &&
                ft.Message.Contains("of type System.ArgumentOutOfRangeException was thrown instead")));
        }

        [Test]
        public void ExpectedExceptionNothingThrownShouldFail()
        {
            var result = SpecRunner.Run<ExpectedExceptionNothingThrownSpec>();
            Assert.Greater(result.FailedTests().Count(), 0, "Test cases should have failed");
            Assert.IsTrue(result.FailedTests().All(ft =>
                ft.Message.StartsWith("System.Exception") &&
                ft.Message.Contains("of type System.InvalidOperationException was not thrown") &&
                ft.Message.Contains("no exception was thrown")));
        }

        [Test]
        public void ExpectedExceptionExpectationFailureShouldFail()
        {
            var result = SpecRunner.Run<ExpectedExceptionExpectationFailureSpec>();
            Assert.Greater(result.FailedTests().Count(), 0, "Test cases should have failed");
            Assert.IsTrue(result.FailedTests().All(ft =>
                ft.Message.StartsWith("System.Exception") &&
                ft.Message.Contains("the specified constraint failed")));
        }
    }
}