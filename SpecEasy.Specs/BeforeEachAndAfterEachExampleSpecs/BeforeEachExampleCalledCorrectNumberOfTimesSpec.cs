using FluentAssertions;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public class BeforeEachExampleCalledCorrectNumberOfTimesSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;
        private int verifyCall = 1;

        public void CountBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => timesCalled.Should().Be(verifyCall));

            Then("it should only call BeforeEachExample once to start with", () => timesCalled.Should().Be(verifyCall++));
            Then("it should call BeforeEachExample a second time for the next test", () => timesCalled.Should().Be(verifyCall++));

            Given("there is a first level of given methods", () => timesCalled.Should().Be(verifyCall)).Verify(() => {
                Then("it should should call BeforeEachExample a third time", () => timesCalled.Should().Be(verifyCall++));
                Then("it should should call BeforeEachExample a fourth time", () => timesCalled.Should().Be(verifyCall++));
                Then("it should should call BeforeEachExample a fifth time", () => timesCalled.Should().Be(verifyCall++));
                Then("it should should call BeforeEachExample a sixth time", () => timesCalled.Should().Be(verifyCall++));
                Given("there is a second level of given methods", () => timesCalled.Should().Be(verifyCall)).Verify(() =>
                    Then("it should call BeforeEachExample a seventh time.", () => timesCalled.Should().Be(verifyCall++)));
            });
        }

        public void NestedGivenBeforeEachExampleCalls()
        {
            When("running a test that overrides BeforeEachExample", () => timesCalled.Should().Be(verifyCall));

            Given("there is a first level of given methods", () => timesCalled.Should().Be(verifyCall)).Verify(() =>
                Given("there is a second level of given methods", () => timesCalled.Should().Be(verifyCall)).Verify(() =>
                    Then("it should call BeforeEachExample one time.", () => timesCalled.Should().Be(verifyCall++))));
        }

        protected override void BeforeEachExample()
        {
            timesCalled++;
        }
    }
}
