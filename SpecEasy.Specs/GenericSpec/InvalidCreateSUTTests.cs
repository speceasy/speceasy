using System.Linq;
using NUnit.Framework;
using SpecEasy.Specs.GenericSpec.SupportingExamples;

namespace SpecEasy.Specs.GenericSpec
{
    [TestFixture]
    public class InvalidCreateSUTTests
    {
        [Test]
        public void NullReturningInvalidCreateSUTTest()
        {
            var result = SpecRunner.Run<NullReturningInvalidCreateSUTSpec>();
            Assert.Greater(result.FailedTests().Count(), 0);
            Assert.IsTrue(result.FailedTests().All(ft => ft.Message.StartsWith("System.InvalidOperationException") && ft.Message.Contains("Failed to construct SUT: ConstructSUT returned null")), "At least one failed spec reported an assertion error when it should have reported an exception.");
        }

        [Test]
        public void ExceptionThrowingInvalidCreateSUTTest()
        {
            var result = SpecRunner.Run<ExceptionThrowingInvalidCreateSUTSpec>();
            Assert.Greater(result.FailedTests().Count(), 0);
            Assert.IsTrue(result.FailedTests().All(ft => ft.Message.Equals(
                $"{ExceptionThrowingInvalidCreateSUTSpec.ConstructSUTThrownException.GetType().FullName} : {ExceptionThrowingInvalidCreateSUTSpec.ConstructSUTThrownException.Message}")));
        }
    }
}