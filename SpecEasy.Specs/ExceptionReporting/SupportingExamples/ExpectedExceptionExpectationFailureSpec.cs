using System;
using FluentAssertions;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    [SupportingExample]
    internal class ExpectedExceptionExpectationFailureSpec : Spec<BrokenClass>
    {
        public void ExpectedExceptionExpectationFailure()
        {
            When("calling a method that throws an exception", () => SUT.InvertThatThrowsArgumentOutOfRangeException(true));

            Then("asserting the correct exception type with an expectation that fails will fail the test with a descriptive error message", () => AssertWasThrown<ArgumentOutOfRangeException>(ex => ex.ParamName.Should().Be("bogus param name")));
        }
    }
}