using NUnit.Framework;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    internal sealed class IntegersSpec : Spec<Integers>
    {
        public void Increment()
        {
            Integers incremented = null;

            When("incrementing", () => incremented = ++SUT);

            Given($"{Integers.Min}", () => SUT = Integers.Min).Verify(() =>
                Then($"{Integers.One} is returned", () => Assert.AreEqual(Integers.One, incremented)));

            Given($"{Integers.One}", () => SUT = Integers.One).Verify(() =>
                Then($"{Integers.Two} is returned", () => Assert.AreEqual(Integers.Two, incremented)));

            Given($"{Integers.Two}", () => SUT = Integers.Two).Verify(() =>
                Then($"{Integers.Three} is returned", () => Assert.AreEqual(Integers.Three, incremented)));

            Given($"{Integers.Three}", () => SUT = Integers.Three).Verify(() =>
                Then($"{Integers.Max} is returned", () => Assert.AreEqual(Integers.Max, incremented)));

            Given($"{Integers.Max}", () => SUT = Integers.Max).Verify(() =>
                Then($"{Integers.Max} is returned", () => Assert.AreEqual(Integers.Max, incremented)));
        }

        public void Decrement()
        {
            Integers decremented = null;

            When("decrementing", () => decremented = --SUT);

            Given($"{Integers.Max}", () => SUT = Integers.Max).Verify(() =>
                Then($"{Integers.Three} is returned", () => Assert.AreEqual(Integers.Three, decremented)));

            Given($"{Integers.Three}", () => SUT = Integers.Three).Verify(() =>
                Then($"{Integers.Two} is returned", () => Assert.AreEqual(Integers.Two, decremented)));

            Given($"{Integers.Two}", () => SUT = Integers.Two).Verify(() =>
                Then($"{Integers.One} is returned", () => Assert.AreEqual(Integers.One, decremented)));

            Given($"{Integers.One}", () => SUT = Integers.One).Verify(() =>
                Then($"{Integers.Min} is returned", () => Assert.AreEqual(Integers.Min, decremented)));

            Given($"{Integers.Min}", () => SUT = Integers.Min).Verify(() =>
                Then($"{Integers.Min} is returned", () => Assert.AreEqual(Integers.Min, decremented)));
        }
    }
}