using NSubstitute;
using Shouldly;
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
                Given("the magic number associated with 3 is 7", () => Get<IMagicNumberLookup>().Lookup(3).Returns(7)).Verify(() =>
                    Given("the second number is 2", () => b = 2).Verify(() =>
                        Given("the magic number associated with 2 is 3", () => Get<IMagicNumberLookup>().Lookup(2).Returns(3)).Verify(() =>
                            Then("the result should be 10", () => result.ShouldBe(10))))));
        }
    }
}
