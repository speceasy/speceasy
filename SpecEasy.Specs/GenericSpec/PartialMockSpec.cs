using NUnit.Framework;
using Rhino.Mocks;

namespace SpecEasy.Specs.GenericSpec
{
    public class PartialMockSpec : Spec<AbstractClass>
    {
        public void Add10ToCalculatedInteger()
        {
            var result = 0;

            When("adding 10 to the calculated integer", () => result = SUT.Add10ToCalculatedInteger());

            Given("the calculated integer is 10", () => SUT.Stub(s => s.CalculateInteger()).Return(10)).Verify(() =>
                Then("the result is 20", () => Assert.AreEqual(20, result)));
        }
    }

    public abstract class AbstractClass
    {
        public int Add10ToCalculatedInteger()
        {
            return CalculateInteger() + 10;
        }

        public abstract int CalculateInteger();
    }
}
