using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace SpecEasy.Specs.GenericSpec
{
    public class TestSkippingSpec : Spec<TestSkippingSpec.IgnoreTestClass>
    {
        private readonly MemoryStream stream = new MemoryStream();
        private const string IgnoreSpectOutput = "inside a test method ignored by the IgnoreSpec attribute";

        public void IgnoreSpecAttribute()
        {
            When("running all tests in a class", () => SUT.Verify());

            Given("a test method that is marked ignored with the IgnoreSpec attribute").Verify(() =>
                Then("it should not run the test", () => OutputShouldNotContain(IgnoreSpectOutput)));
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();

            var listener = new TextWriterTraceListener(stream);
            Debug.Listeners.Add(listener);
            Debug.AutoFlush = true;
        }

        private void OutputShouldNotContain(string outputText)
        {
            var output = Encoding.ASCII.GetString(stream.ToArray());
            Assert.That(output, Is.Not.StringContaining(outputText));
        }

        public class IgnoreTestClass : Spec
        {
            [IgnoreSpec]
            public virtual void IgnoredTest()
            {
                When(IgnoreSpectOutput, () => { });
                Then("this should not show up in the test output", () => { });
            }
        }
    }
}
