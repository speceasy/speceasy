using NUnit.Framework;

namespace SpecEasy.Specs.GenericSpec.SupportingExamples
{
    [SupportingExample]
    internal sealed class NullReturningInvalidCreateSUTSpec : Spec<object>
    {
        public void RunSpec()
        {
            When("constructing SUT", () => EnsureSUT());

            Given("the SUT is constructed manually").Verify(() =>
                Given("the method to construct the SUT returns null").Verify(() =>
                    Then("an appropriate exception is thrown", () => Assert.Pass("We don't actually expect to hit this and pass the test; the assertion of the test runner will be that this test fails."))));
        }

        protected override object ConstructSUT()
        {
            return null;
        }
    }
}