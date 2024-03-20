using NSubstitute;
using NUnit.Framework;

namespace SpecEasy.Specs.GenericSpec
{
    public class PartialMockWithConstructorParamsSpec : Spec<AbstractClassWithConstructorParams>
    {
        public void Add10ToCalculatedInteger()
        {
            var result = 0;

            When("adding dependency value to the calculated integer", () => result = SUT.AddDependencyValueToCalculateInteger());

            Given("the dependency value is \"10\"", () => Get<IDependency1>().Value.Returns("10")).Verify(() =>
            Given("the calculated integer is 10", () => SUT.CalculateInteger().Returns(10)).Verify(() =>
                Then("the result is 20", () => Assert.AreEqual(20, result))));
        }
    }

    public abstract class AbstractClassWithConstructorParams
    {
        private readonly IDependency1 _dependency1;

        protected AbstractClassWithConstructorParams(IDependency1 dependency1)
        {
            _dependency1 = dependency1;
        }

        public int AddDependencyValueToCalculateInteger()
        {
            return CalculateInteger() + int.Parse(_dependency1.Value);
        }

        public abstract int CalculateInteger();
    }
}
