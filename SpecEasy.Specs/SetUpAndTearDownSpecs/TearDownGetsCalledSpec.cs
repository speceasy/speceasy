using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class TearDownGetsCalledSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;

        public void TearDownSpec()
        {
            timesCalled = 0;

            When("running a test in a class that overrides TearDown", () => SUT.DoNothing());
            Then("it should not TearDown before the first Then", () => Assert.That(timesCalled, Is.EqualTo(0)));
            Then("it should call TearDown before the second Then", () => Assert.That(timesCalled, Is.EqualTo(1)));
        }

        public void TearDownWithGivenSpec()
        {
            timesCalled = 0;

            When("running a test in a class that overrides TearDown", () => SUT.DoNothing());

            Given("there is a given", () => SUT.DoNothing()).Verify(() => {
                Then("it should not call TearDown before the first Then", () => Assert.That(timesCalled, Is.EqualTo(0)));
                Then("it should call TearDown before the second Then", () => Assert.That(timesCalled, Is.EqualTo(1)));
            });

            Given("there is another given", () => SUT.DoNothing()).Verify(() =>
                {
                    Then("it should call TearDown again before the third Then", () => Assert.That(timesCalled, Is.EqualTo(2)));
                    Given("we have nested givens", () => SUT.DoNothing()).Verify(() => 
                        Then("it should call TearDown again before the nested Then", () => Assert.That(timesCalled, Is.EqualTo(3))));
                });
        }

        protected override void TearDown()
        {
            base.TearDown();
            timesCalled++;
        }
    }
}