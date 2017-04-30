using Rhino.Mocks;
using Should;
using SpecEasy.ExternalLib;

namespace SpecEasy.Specs.ExternalSpec
{
    internal class MagicNumberAdderSpec : Spec<MagicNumberAdder>
    {
        public void Add()
        {
            int result = -1;
            int a = 0;
            int b = 0;

            When("adding two numbers together", () => result = SUT.AddNumbers(a, b));

            Given("the first number is 3", () => a = 3).Verify(() =>
                Given("the magic number associated with 3 is 7", () => Get<IMagicNumberLookup>().Stub(l => l.Lookup(3)).Return(7)).Verify(() =>
                    Given("the second number is 2", () => b = 2).Verify(() =>
                        Given("the magic number associated with 2 is 3", () => Get<IMagicNumberLookup>().Stub(l => l.Lookup(2)).Return(3)).Verify(() =>
                            Then("the result should be 10", () => result.ShouldEqual(10))))));
        }
    }
}
