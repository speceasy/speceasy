using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class SetUpGetsCalledSpec : Spec<FakeClass>
    {
        private int testValue = 100;

        public void SetUpSpec()
        {
            When("running a test with SetUp overridden", () => SUT.DoNothing());

            Then("it should run SetUp", () => Assert.That(testValue, Is.EqualTo(50)));

            Given("a given changes the value set in SetUp", () => testValue = 75).Verify(() => 
                Then("it should not have the value from SetUp", () => Assert.That(testValue, Is.EqualTo(75))));
        }

        protected override void SetUp()
        {
            base.SetUp();
            testValue = 50;
        }
    }

    public class FakeClass
    {
        public void DoNothing() { }
    }
}