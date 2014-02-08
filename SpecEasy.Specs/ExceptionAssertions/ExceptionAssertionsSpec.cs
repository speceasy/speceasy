using System;
using Should;

namespace SpecEasy.Specs.ExceptionAssertions
{
    public class ExceptionAssertionsSpec : Spec<SimpleCalculator>
    {
        public void Run()
        {
            var result = 0;
            var number1 = 0;
            var number2 = 0;

            When("using the simple calculator", () => result = SUT.Add(number1, number2));

            Given("the first number is 1", () => number1 = 1).Verify(() => {
                Given("the second number is 2", () => number2 = 2).Verify(() => 
                    Then("the result should be 3", () => result.ShouldEqual(3)));

                Given("the second number is 11", () => number2 = 11).Verify(() => {
                    Then("it should throw an argument out of range exception", () => AssertWasThrown<ArgumentOutOfRangeException>());
                    Then("the thrown exception should indicate what argument is in error", 
                        () => AssertWasThrown<ArgumentOutOfRangeException>(ex => ex.ParamName.ShouldEqual("number2")));});});
        }    
    }

    public class SimpleCalculator
    {
        public int Add(int number1, int number2)
        {
            if (number2 > 10)
                throw new ArgumentOutOfRangeException("number2");

            return number1 + number2;
        }
    }
}
