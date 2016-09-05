using Should;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    class SutSetterAndConstructSutSpec : Spec<SutWithValueTypeDependency>
    {
        protected override SutWithValueTypeDependency ConstructSUT()
        {
            return new SutWithValueTypeDependency(456);
        }

        public void Run()
        {
            var value = 0;

            When("getting value from SUT", () => value = SUT.Value);

            Given("SUT has not been explicitly set").Verify(() =>
                Then("it should get the value used in ConstructSUT", () => value.ShouldEqual(456)));

            Given("SUT has been explicitly set", () => SUT = new SutWithValueTypeDependency(123)).Verify(() =>
                Then("it should get the value provided when the SUT was explicitly set", () => value.ShouldEqual(123)));
        }
    }
}