using System;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    [SupportingExample]
    internal class ExpectedExceptionDifferentTypeSpec : Spec<BrokenClass>
    {
        public void ExpectedException()
        {
            When("calling a method that throws an exception", () => SUT.InvertThatThrowsArgumentOutOfRangeException(true));

            Then("asserting the wrong exception type will fail the test with a descriptive error message", () => AssertWasThrown<InvalidOperationException>());
        }
    }
}