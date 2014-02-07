using Should;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public class BeforeEachExampleCalledCorrectNumberOfTimesSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;
        private int verifyCall = 1;

        public void CountBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => timesCalled.ShouldEqual(verifyCall));

            Then("it should only call BeforeEachExample once to start with", () => timesCalled.ShouldEqual(verifyCall++));
            Then("it should call BeforeEachExample a second time for the next test", () => timesCalled.ShouldEqual(verifyCall++));

            Given("there is a first level of given methods", () => timesCalled.ShouldEqual(verifyCall)).Verify(() => {
                Then("it should should call BeforeEachExample a third time", () => timesCalled.ShouldEqual(verifyCall++));
                Then("it should should call BeforeEachExample a fourth time", () => timesCalled.ShouldEqual(verifyCall++));
                Then("it should should call BeforeEachExample a fifth time", () => timesCalled.ShouldEqual(verifyCall++));
                Then("it should should call BeforeEachExample a sixth time", () => timesCalled.ShouldEqual(verifyCall++));
                Given("there is a second level of given methods", () => timesCalled.ShouldEqual(verifyCall)).Verify(() => 
                    Then("it should call BeforeEachExample a seventh time.", () => timesCalled.ShouldEqual(verifyCall++)));
            });
        }

        public void NestedGivenBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => timesCalled.ShouldEqual(verifyCall));

            Given("there is a first level of given methods", () => timesCalled.ShouldEqual(verifyCall)).Verify(() => 
                Given("there is a second level of given methods", () => timesCalled.ShouldEqual(verifyCall)).Verify(() => 
                    Then("it should call BeforeEachExample one time.", () => timesCalled.ShouldEqual(verifyCall++))));
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
            timesCalled++;
        }
         
    }
}
