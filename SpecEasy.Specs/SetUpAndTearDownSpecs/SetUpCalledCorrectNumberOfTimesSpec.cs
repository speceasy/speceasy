using NUnit.Framework;

namespace SpecEasy.Specs.SetUpAndTearDownSpecs
{
    public class SetUpCalledCorrectNumberOfTimesSpec : Spec<FakeClass>
    {
        private int timesCalled = 0;
        public void CountSetUpCalls()
        {
            When("running a test that overrides SetUp", () => SUT.DoNothing());
            Then("it should only call SetUp once to start with", () => Assert.That(timesCalled, Is.EqualTo(1)));
            Then("it should call SetUp a second time for the next test", () => Assert.That(timesCalled, Is.EqualTo(2)));
        }

        protected override void SetUp()
        {
            base.SetUp();
            timesCalled++;
        }
         
    }
}