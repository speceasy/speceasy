using NUnit.Framework;

namespace SpecEasy.Specs.DuplicateDescriptions.SupportingExamples
{
    [SupportingExample]
    internal class DuplicateDescriptionsSpec : Spec
    {
        private const string DuplicatedGivenDescription = "my given description";
        private const string DuplicatedThenDescription  = "my then description";

        public void DuplicateGivenDescriptions()
        {
            When("running a spec", () => {});

            Given(DuplicatedGivenDescription).Verify(() =>
                Then("assert 1", () => Assert.Pass()));

            Given(DuplicatedGivenDescription).Verify(() =>
                Then("assert 2", () => Assert.Pass()));
        }

        public void DuplicateNestedGivenDescriptions()
        {
            When("running a spec", () => {});

            Given("outer given").Verify(() =>
            {
                Given(DuplicatedGivenDescription).Verify(() =>
                    Then("assert 1", () => Assert.Pass()));

                Given(DuplicatedGivenDescription).Verify(() =>
                    Then("assert 2", () => Assert.Pass()));
            });
        }

        public void DuplicateThenDescriptions()
        {
            When("running a spec", () => {});

            Then(DuplicatedThenDescription, () => Assert.Pass());
            Then(DuplicatedThenDescription, () => Assert.Pass());
        }

        public void DuplicateNestedThenDescriptions()
        {
            When("running a spec", () => { });

            Given("outer given").Verify(() =>
            {
                Then(DuplicatedThenDescription, () => Assert.Pass());
                Then(DuplicatedThenDescription, () => Assert.Pass());
            });
        }
    }
}