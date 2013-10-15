using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class BeforeEachExampleCalledCorrectNumberOfTimesSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;
        private int verifyCall = 1;

        public void CountBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => Assert.That(timesCalled, Is.EqualTo(verifyCall)));

            Then("it should only call BeforeEachExample once to start with", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)));
            Then("it should call BeforeEachExample a second time for the next test", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)));

            Given("there is a first level of given methods", () => Assert.That(timesCalled, Is.EqualTo(verifyCall))).Verify(() => {
                Then("it should should call BeforeEachExample a third time", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)));
                Then("it should should call BeforeEachExample a fourth time", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)));
                Then("it should should call BeforeEachExample a fifth time", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)));
                Then("it should should call BeforeEachExample a sixth time", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)));
                Given("there is a second level of given methods", () => Assert.That(timesCalled, Is.EqualTo(verifyCall))).Verify(() => 
                    Then("it should call BeforeEachExample a seventh time.", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++))));
            });
        }

        public void NestedGivenBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => Assert.That(timesCalled, Is.EqualTo(verifyCall)));

            Given("there is a first level of given methods", () => Assert.That(timesCalled, Is.EqualTo(verifyCall))).Verify(() => 
                Given("there is a second level of given methods", () => Assert.That(timesCalled, Is.EqualTo(verifyCall))).Verify(() => 
                    Then("it should call BeforeEachExample one time.", () => Assert.That(timesCalled, Is.EqualTo(verifyCall++)))));
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
            timesCalled++;
        }
         
    }
}