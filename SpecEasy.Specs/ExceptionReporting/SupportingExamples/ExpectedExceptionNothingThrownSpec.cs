using System;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    [SupportingExample]
    internal class ExpectedExceptionNothingThrownSpec : Spec<BrokenClass>
    {
        public void ExpectedException()
        {
            When("calling a method that does not throw an exception", () => SUT.InvertThatDoesNotThrow(true));

            Then("asserting an exception type will fail the test with a descriptive error message", () => AssertWasThrown<InvalidOperationException>());
        }
    }
}