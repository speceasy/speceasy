using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class AfterEachExampleGetsCalledSpec : Spec<FakeClass>
    {
        private int timesSet = 0;

        public void AfterEachExampleSpec()
        {
            When("running a test in a class that resets a variable in AfterEachExample", () => SUT.DoNothing());
            Then("the variable should be reset before the first Then, which sets it", () => Assert.That(timesSet++ == 0));
            Then("the variable should be reset before the second Then, which sets it", () => Assert.That(timesSet++ == 0));
        }

        public void AfterEachExampleWithGivenSpec()
        {
            When("running a test in a class that resets a variable in AfterEachExample", () => SUT.DoNothing());

            Given("there is a given", () => SUT.DoNothing()).Verify(() => {
                Then("the variable should be reset before the first Then, which sets it", () => Assert.That(timesSet++ == 0));
                Then("the variable should be reset before the second Then, which sets it", () => Assert.That(timesSet++ == 0));
            });

            Given("there is another given", () => SUT.DoNothing()).Verify(() =>
                {
                    Then("the variable should be reset before the third Then, which sets it", () => Assert.That(timesSet++ == 0));
                    Given("we have nested givens", () => SUT.DoNothing()).Verify(() =>
                        Then("the variable should be reset before the fourth Then, which sets it", () => Assert.That(timesSet++ == 0)));
                });
        }

        protected override void AfterEachExample()
        {
            base.AfterEachExample();
            timesSet = 0; 
        }
    }
}