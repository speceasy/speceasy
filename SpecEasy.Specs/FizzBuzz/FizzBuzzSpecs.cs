using NUnit.Framework;

namespace SpecEasy.Specs.FizzBuzz
{
    public class FizzBuzzSpec : Spec<FizzBuzzImpl>
    {
        public void Run()
        {
            var input = 0;
            var result = string.Empty;

            When("running FizzBuzz", () => result = SUT.Run(input));

            Given("an input of 1", () => input = 1).Verify(() => 
                Then("it should return a stringified 1", () => Assert.That(result, Is.EqualTo("1"))));

            Given("an input of 2", () => input = 2).Verify(() => 
                Then("it should return a stringified 2", () => Assert.That(result, Is.EqualTo("2"))));

            Given("an input of 3", () => input = 3).Verify(() =>
                Then("it should return fizz", () => Assert.That(result, Is.EqualTo("fizz"))));

            Given("an input of a multiple of 3", () => input = 9).Verify(() =>
                Then("it should return fizz", () => Assert.That(result, Is.EqualTo("fizz"))));

            Given("an input of 5", () => input = 5).Verify(() =>
                Then("it should return buzz", () => Assert.That(result, Is.EqualTo("buzz"))));

            Given("an input of a multiple of 5", () => input = 20).Verify(() =>
                Then("it should return buzz", () => Assert.That(result, Is.EqualTo("buzz"))));

            Given("an input of a multiple of 3 and 5", () => input = 30).Verify(() =>
                Then("it should return fizzbuzz", () => Assert.That(result, Is.EqualTo("fizzbuzz"))));
        }
    }

    public class FizzBuzzImpl
    {
        public string Run(int input)
        {
            string retVal = null;

            if (input%3 == 0)
            {
                retVal += "fizz";
            }

            if (input%5 == 0)
            {
                retVal += "buzz";
            }

            return retVal ?? input.ToString();
        }
    }
}
