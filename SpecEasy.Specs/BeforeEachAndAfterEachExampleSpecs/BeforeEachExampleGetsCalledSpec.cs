using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class BeforeEachExampleGetsCalledSpec : Spec<FakeClass>
    {
        private int testValue = 100;

        public void BeforeEachExampleSpec()
        {
            When("running a test with BeforeEachExample overridden", () => SUT.DoNothing());

            Then("it should run BeforeEachExample", () => Assert.That(testValue, Is.EqualTo(50)));

            Given("a given changes the value set in BeforeEachExample", () => testValue = 75).Verify(() => 
                Then("it should not have the value from BeforeEachExample", () => Assert.That(testValue, Is.EqualTo(75))));
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
            testValue = 50;
        }
    }

    public class FakeClass
    {
        public void DoNothing() { }
    }
}