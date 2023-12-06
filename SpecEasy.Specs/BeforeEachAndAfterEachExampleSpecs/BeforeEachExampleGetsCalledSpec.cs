using Shouldly;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public class BeforeEachExampleGetsCalledSpec : Spec<FakeClass>
    {
        private int testValue = 100;

        public void BeforeEachExampleSpec()
        {
            When("running a test with BeforeEachExample overridden", () => SUT.DoNothing());

            Then("it should run BeforeEachExample", () => testValue.ShouldBe(50));

            Given("a given changes the value set in BeforeEachExample", () => testValue = 75).Verify(() =>
                Then("it should not have the value from BeforeEachExample", () => testValue.ShouldBe(75)));
        }

        protected override void BeforeEachExample()
        {
            testValue = 50;
        }
    }

    public class FakeClass
    {
        public void DoNothing() { }
    }
}
