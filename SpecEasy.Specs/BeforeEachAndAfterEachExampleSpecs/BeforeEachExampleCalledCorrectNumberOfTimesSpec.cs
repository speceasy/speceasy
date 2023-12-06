using Shouldly;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public class BeforeEachExampleCalledCorrectNumberOfTimesSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;
        private int verifyCall = 1;

        public void CountBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => timesCalled.ShouldBe(verifyCall));

            Then("it should only call BeforeEachExample once to start with", () => timesCalled.ShouldBe(verifyCall++));
            Then("it should call BeforeEachExample a second time for the next test", () => timesCalled.ShouldBe(verifyCall++));

            Given("there is a first level of given methods", () => timesCalled.ShouldBe(verifyCall)).Verify(() => {
                Then("it should should call BeforeEachExample a third time", () => timesCalled.ShouldBe(verifyCall++));
                Then("it should should call BeforeEachExample a fourth time", () => timesCalled.ShouldBe(verifyCall++));
                Then("it should should call BeforeEachExample a fifth time", () => timesCalled.ShouldBe(verifyCall++));
                Then("it should should call BeforeEachExample a sixth time", () => timesCalled.ShouldBe(verifyCall++));
                Given("there is a second level of given methods", () => timesCalled.ShouldBe(verifyCall)).Verify(() =>
                    Then("it should call BeforeEachExample a seventh time.", () => timesCalled.ShouldBe(verifyCall++)));
            });
        }

        public void NestedGivenBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => timesCalled.ShouldBe(verifyCall));

            Given("there is a first level of given methods", () => timesCalled.ShouldBe(verifyCall)).Verify(() =>
                Given("there is a second level of given methods", () => timesCalled.ShouldBe(verifyCall)).Verify(() =>
                    Then("it should call BeforeEachExample one time.", () => timesCalled.ShouldBe(verifyCall++))));
        }

        protected override void BeforeEachExample()
        {
            timesCalled++;
        }
    }
}
