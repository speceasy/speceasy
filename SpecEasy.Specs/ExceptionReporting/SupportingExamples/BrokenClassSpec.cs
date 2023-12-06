using Shouldly;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    [SupportingExample]
    internal class BrokenClassSpec : Spec<BrokenClass>
    {
        private bool input;
        private bool result;

        public void RunSpec()
        {
            When("inverting a boolean value", () => result = SUT.InvertThatThrowsArgumentOutOfRangeException(input));

            Given("a false input", () => input = false).Verify(() =>
                Then("the result is true", () => result.ShouldBeTrue()));

            Given("a true input", () => input = true).Verify(() =>
                Then("the result is false", () => result.ShouldBeFalse()));
        }
    }
}