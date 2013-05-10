using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class AfterEachExampleGetsCalledSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;

        public void AfterEachExampleSpec()
        {
            timesCalled = 0;

            When("running a test in a class that overrides AfterEachExample", () => SUT.DoNothing());
            Then("it should not AfterEachExample before the first Then", () => Assert.That(timesCalled, Is.EqualTo(0)));
            Then("it should call AfterEachExample before the second Then", () => Assert.That(timesCalled, Is.EqualTo(1)));
        }

        public void AfterEachExampleWithGivenSpec()
        {
            timesCalled = 0;

            When("running a test in a class that overrides AfterEachExample", () => SUT.DoNothing());

            Given("there is a given", () => SUT.DoNothing()).Verify(() => {
                Then("it should not call AfterEachExample before the first Then", () => Assert.That(timesCalled, Is.EqualTo(0)));
                Then("it should call AfterEachExample before the second Then", () => Assert.That(timesCalled, Is.EqualTo(1)));
            });

            Given("there is another given", () => SUT.DoNothing()).Verify(() =>
                {
                    Then("it should call AfterEachExample again before the third Then", () => Assert.That(timesCalled, Is.EqualTo(2)));
                    Given("we have nested givens", () => SUT.DoNothing()).Verify(() => 
                        Then("it should call AfterEachExample again before the nested Then", () => Assert.That(timesCalled, Is.EqualTo(3))));
                });
        }

        protected override void AfterEachExample()
        {
            base.AfterEachExample();
            timesCalled++;
        }
    }
}