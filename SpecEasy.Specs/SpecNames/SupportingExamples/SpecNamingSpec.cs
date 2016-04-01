namespace SpecEasy.Specs.SpecNames.SupportingExamples
{
    [SupportingExample]
    internal class SpecNamingSpec : Spec
    {
        public static string ExpectedSpecName = "given a context\r\n  and a sub context\r\n  and another sub context\r\n  but yet another sub context\r\nwhen running the test\r\nthen the test passes\r\n";

        public void RunSpec()
        {
            When("running the test", () => { });

            Given("a context", () => { }).Verify(() =>
            Given("a sub context", () => { }).Verify(() =>
            And("another sub context", () => { }).Verify(() =>
            But("yet another sub context", async () => { }).Verify(() =>
                Then("the test passes", () => AnExampleTestThatPasses())))));
        }

        private void AnExampleTestThatPasses()
        {
        }
    }
}