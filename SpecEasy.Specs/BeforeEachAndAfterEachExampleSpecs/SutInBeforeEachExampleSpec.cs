using Should;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    class SutInBeforeEachExampleSpec : Spec<SutInBeforeEachExampleSpec.SutWithValueTypeDependency>
    {
        protected override void BeforeEachExample()
        {
            SUT = new SutWithValueTypeDependency(123);
        }

        public void Run()
        {
            var value = 0;

            When("getting value from SUT", () => value = SUT.Value);
            Given("SUT was set up in BeforeEachExample").Verify(() =>
                Then("it should get the value from the SUT set up in BeforeEachExample", () => value.ShouldEqual(123)));
        }

        public class SutWithValueTypeDependency
        {
            public int Value { get; private set; }

            public SutWithValueTypeDependency(int value)
            {
                Value = value;
            }
        }
    }
}
