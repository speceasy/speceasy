<h1 align="center"><img src="https://raw.github.com/trackabout/speceasy/master/speceasy_logo.png" alt="SpecEasy" height="85"></h1>

SpecEasy is a BDD-based unit testing framework that allows you to easily and quickly write tests using a fluid interface that reduces the amount of code needed to create tests.

## What does SpecEasy require?

SpecEasy is currently built on and requires the NUnit test framework. If you're using NUnit, then it's as simple as:

1. [Install SpecEasy from NuGet][NuGet]
2. Start writing specs the easy way!

SpecEasy also uses [RhinoMocks][]; this dependency will be installed via NuGet if needed.

## Quickstart Guide

* Inherit from `Spec<T>`, where T is the type you want to test. SpecEasy give you access to an instance of T through a base variable called SUT (System Under Test), and automatically new it up on demand.
* No need to decorate your code with test attributes. `Spec<T>` handles that for you.
* Use `When(string, Action)` to test what you want to test (Action).
* Use `Given(string, Action).Verify(Action)` for set up code (Arrange).
* Use `Then(string, Action)` to verify the results (Assert).
* Nest `Given(string Action)` method calls to handle more complicated set up.
* Use `Set<T>(T)` to set dependencies that will be injected into your SUT.
* Use `Get<T>()` to get access to dependencies automatically created for your SUT. Chain calls to `Get<T>()` with calls to `Stub<T>(Action)` to set up mocks for dependencies.
* Use `AssertWasCalled<T>(Action)` and `AssertWasNotCalled<T>(Action)` to check if methods or properties were called on dependencies.
 
## Going a bit deeper

Let's use FizzBuzz as a sample implementation to test. As a reminder, the rules for FizzBuzz are as follows:

* The method should take in a number and return a string
* For most numbers, return the string value for that number, i.e., 1 returns "1"
* For multiples of 3, it should return "fizz"
* For multiples of 5, it should return "buzz"
* For multiples of 3 and 5, it should return "fizzbuzz"

We'll start out with a base FizzBuzz implementation:

    public class FizzBuzz 
    {
        public string Do(int number) 
        {
            return "";
        } 
    }

Let's get started writing specs.

### 1.) Inherit from Spec

    public class FizzBuzzpecs : SpecEasy.Spec<FizzBuzz> { }

### 2.) Test the simplest case first

SpecEasy doesn't require that you decorate your code with attributes to run tests. It uses  Given/When/Then syntax, and every call to Then will result in a test being run. The first step to check if I pass in 1, does it return "1".

    public void FizzBuzzDo()
    {
        string result = string.Empty;
        int input = 0;

        When("running FizzBuzz", () => result = SUT.Do(input));

        Given("an input of 1", () => input = 1).Verify(() =>
            Then("it should return a stringified 1", () => Assert.That(result, Is.EqualTo("1"))));
    }

Now, if we run the test, it fails. 

    Test 'TrackAbout.Mobile.Test.Unit.FizzBuzz.FizzBuzzSpecs.Spec.Verify' failed: System.Exception : Specifications failed!
    ----> NUnit.Framework.AssertionException :   Expected string length 1 but was 0. Strings differ at index 0.
    Expected: "1"
    But was:  <string.Empty>
    -----------^
        at SpecEasy.Spec.Verify()
        --AssertionException
        FizzBuzzSpecs.cs(16,0): at FizzBuzzSpecs.<>c__DisplayClass4.<FizzBuzzDo>b__2()
        at SpecEasy.Spec.VerifySpecs(List`1 contextList)

    0 passed, 1 failed, 0 skipped

Once we change the FizzBuzz class to allow the test to pass, we see the following output:

    ------------ FULL RESULTS ------------
    given an input of 1
    when running FizzBuzz
    it should return a stringified 1

    1 passed, 0 failed, 0 skipped

### 3.) Test another case

We can have multiple Given/Then combinations, where we can set up different expectations and assert different things. So if we wanted to test an input of 2, we can add another given/then combination:

public void FizzBuzzDo()
    {
        string result = string.Empty;
        int input = 0;

        When("running FizzBuzz", () => result = SUT.Do(input));

        Given("an input of 1", () => input = 1).Verify(() =>
            Then("it should return a stringified 1", () => Assert.That(result, Is.EqualTo("1"))));

        Given("an input of 2", () => input = 2).Verify(() => 
            Then("it should return a stringified 2", () => Assert.That(result, Is.EqualTo("2"))));
    }

Once we fix up FizzBuzz, we get this output:

    ------------ FULL RESULTS ------------
    given an input of 1
    when running FizzBuzz
    then it should return a stringified 1

    given an input of 2
    when running FizzBuzz
    then it should return a stringified 2

    1 passed, 0 failed, 0 skipped

### 4.) Finish up the tests

You can continue the implementation of FizzBuzz this way, and end up with tests like so:

    public void FizzBuzzDo()
    {
        string result = string.Empty;
        int input = 0;

        When("running FizzBuzz", () => result = SUT.Do(input));

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

Once FizzBuzz is implemented, you get an output like so

    ------------ FULL RESULTS ------------
    given an input of 1
    when running FizzBuzz
    then it should return a stringified 1

    given an input of 2
    when running FizzBuzz
    then it should return a stringified 2

    given an input of 3
    when running FizzBuzz
    then it should return fizz

    given an input of a multiple of 3
    when running FizzBuzz
    then it should return fizz

    given an input of 5
    when running FizzBuzz
    then it should return buzz

    given an input of a multiple of 5
    when running FizzBuzz
    then it should return buzz

    given an input of a multiple of 3 and 5
    when running FizzBuzz
    then it should return fizzbuzz

    1 passed, 0 failed, 0 skipped

## Dependencies

One thing that wasn't mentioned in the above spec is that SpecEasy automatically instantiated your class under test for you, and gave you access to it through a variable called SUT (system under test). But what if your SUT has dependencies? SpecEasy can handle that for you too, either automatically, or you can determine what values to use for your class.

Let's say I have a class that takes in two variables

    public class CarService 
    {
        private readonly ICar car;
        private readonly IDriver driver;

        public CarService(ICar car, IDriver driver) 
        {
            this.car = car;
            this.driver = driver;
        }

        public void Drive() 
        {
            // Make the driver drive the car
        }
    }

If we want to test the Drive method, most likely what we want to do is ensure that certain methods are called on the car and driver. To do that, we need to be able to have access to the car and driver objects in our test. You can do this manually, using the `Set<T>(T)` method:

    public class CarServiceSpec
    {
        public Drive()
        {
            ICar car = new FakeCar();
            IDriver driver = FakeDriver();

            Set<ICar>(car);
            Set<IDriver>(driver);

            When("driving a car", () => SUT.Drive());

            Then("the driver should start the car", () => driver.AssertStartWasCalledWithParameter(car));
        }
    }

This requires you to make a fake for every parameter and then implement code to track method calls. It works, but can be a lot of extra test code. SpecEasy can eliminate this code by using mocks. If we allow SpecEasy to create mocks for us, the above code becomes:

    public class CarServiceSpec
    {
        public Drive()
        {
            When("driving a car", () => SUT.Drive());

            Then("the driver should start the car", () => AssertWasCalled<IDriver>(d => d.Start(Get<ICar>()));
        }
    }

The Spec<T> base class has a method AssertWasCalled<T>() that can be used to determine if a method is called or a property was set on a dependency. 

## Stubs on Dependencies

Since SpecEasy handles creating your dependencies for you, it also has a way to set up stubs on those dependencies. For example, in the above scenario, let's say that you need to be able to get keys from the driver. You can stub that in a Given() call.

    public class CarServiceSpec
    {
        public Drive()
        {
            var keys = new CarKey();
            When("driving a car", () => SUT.Drive());

            Given("the driver is carrying keys", => () => Get<IDriver>().Stub(d => d.GetKeys()).Return(keys)).Verify(() => 
                Then("the driver should start the car", () => AssertWasCalled<IDriver>(d => d.Start(Get<ICar>())));
        }
    }

## Multiple Givens

If SpecEasy was just an easier way to write standard tests, it wouldn't be all that useful. Where SpecEasy shines is in its ability to allow nested Given statements and allow tests to be put at any level of the Given hierarchy. This allows you to have complicated set up code and not have to duplicate that for each test that needs that set up code.

Continuing with the above scenario, let's imagine we're testing the following code on CarService's Drive() method.

    public void Drive()
    {
        var keys = driver.GetKeys();
        if (car.Accepts(keys)) 
        {
            driver.Start(car);
        }
        else 
        {
            driver.FindKeysFor(car);
        }
    }

We have three tests we want to write to cover all of the things going on here:

1. driver.GetKeys() is called
2. if the car accepts the keys, that the driver starts the car
3. If the car doesn't accept the keys, that the driver doesn't start the car

We can do this using multiple nested calls to Given() for the different setups we need:

    public class CarServiceSpec
    {
        public Drive()
        {
            var keys = new CarKey();
            When("driving a car", () => SUT.Drive());

            Given("the driver is carrying keys", => () => Get<IDriver>().Stub(d => d.GetKeys()).Return(keys)).Verify(() => {
                Then("the driver should show her keys", () => AssertWasCalled<IDriver>(d => d.GetKeys()));

                Given("the car accepts the keys", () => Get<ICar>().Stub(c => c.Accepts(keys)).Return(true)).Verify(() => 
                    Then("it should start the car", () => AssertWasCalled<IDriver>(d => d.Start(Get<ICar>()))));

                Given("the car does not accept the keys", () => Get<ICar>().Stub(c => c.Accepts(keys)).Return(false)).Verify(() => 
                    Then("it should not start the car", () => AssertWasNotCalled<IDriver>(d => d.Start(Arg<ICar>.Is.Anything))));
            });
        }
    }

For each call to Then(), SpecEasy walks up the call stack, running each Given(), then the When(), and finally, the Then(). So, for the above tests, it would output the following:

    ------------ FULL RESULTS ------------
    given the driver is carrying keys
    when driving a car
    then the driver should show her keys

    given the driver is carrying keys
    and the car accepts the keys
    when driving a car
    then it should start the car

    given the driver is carrying keys
    and the car does not accept the keys
    when driving a car
    then it should not start the car

    1 passed, 0 failed, 0 skipped

## Multiple Expectations

You may find the need to make multiple assertions, or verify multiple expectations, for a single Given. This can be done by using multiple `Then` method calls, either by listing them as multiple statements within a function provided to `Verify`, or as a single expression of chained `Then` methods provided to `Verify`.

    public void MultipleStatements()
    {
        string result = string.Empty;
        int input = 0;

        When("running FizzBuzz", () => result = SUT.Do(input));
        Given("an input of a multiple of 3 and 5", () => input = 30).Verify(() =>
        {
            Then("it should return a string starting with fizz", () => Assert.That(result, Is.StringStarting("fizz")));
            Then("it should return a string ending with buzz", () => Assert.That(result, Is.StringEnding("buzz")));
        });
    }

    public void ChainedExpression()
    {
        string result = string.Empty;
        int input = 0;
        
        When("running FizzBuzz", () => result = SUT.Do(input));
        Given("an input of a multiple of 3 and 5", () => input = 30).Verify(() =>
            Then("it should return a string starting with fizz", () => Assert.That(result, Is.StringStarting("fizz")))
            .Then("it should return a string ending with buzz", () => Assert.That(result, Is.StringEnding("buzz"))));
    }

Either form will result in the following output:

    ------------ FULL RESULTS ------------
    given an input of a multiple of 3 and 5
    when running FizzBuzz
    then it should return a string starting with fizz
      and it should return a string ending with buzz

## License

SpecEasy is released under the [MIT license](https://raw.github.com/trackabout/speceasy/master/LICENSE).

[NuGet]: https://www.nuget.org/packages/SpecEasy
[RhinoMocks]: http://www.nuget.org/packages/RhinoMocks/
