using FluentAssertions;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace SpecEasy.Specs.Async
{
    public class AsyncSpec : Spec<AsyncTester>
    {
        private const string Name = "placeholder";
        private const string Value = "The quick brown fox jumped over the lazy dog.";

        public void ReturnOne()
        {
            string result = string.Empty;
            string intermediate = string.Empty;
            int count = 1;

            When("loading", async () => result = await SUT.LoadAsync(Name).ConfigureAwait(false));

            Given("a value has been saved", () => Get<IPersistence>().LoadAsync(Name).Returns(Task.Factory.StartNew(() => Value))).Verify(() =>
            {
                Then("the desired value is loaded", () => result.Should().Be(Value));

                Given("the persistence tracks the load count", () => Get<IPersistence>().GetLoadCountAsync().Returns(Task.Factory.StartNew(() => count))).Verify(() =>
                    Then("the number of loads can be retrieved", async () => count.Should().Be(await SUT.GetLoadCountAsync().ConfigureAwait(false))));
            });

            Given("there is an error loading the value", () => Get<IPersistence>().LoadAsync(Name).Returns<Task<string>>(s => throw new InvalidOperationException())).Verify(() =>
                Then("an exception is thrown", AssertWasThrown<InvalidOperationException>));

            Given("the data was previously persisted", async () =>
            {
                Get<IPersistence>().SaveAsync(Name, Arg.Any<string>()).Returns(Task.Factory.StartNew(() => { }));
                Get<IPersistence>().When(x => x.SaveAsync(Name, Arg.Any<string>())).Do(x =>  intermediate = (string)x[1]);
                await SUT.SaveAsync(Name, Value).ConfigureAwait(false);
            }).Verify(() =>
                Given("the data is then loaded", () => Get<IPersistence>().LoadAsync(Name).Returns(Task.Factory.StartNew(() => intermediate))).Verify(() =>
                    Then("the previously stored data is retrieved", () => result.Should().Be(Value))));
        }
    }

    public class AsyncTester
    {
        private readonly IPersistence stubInterface;
        public AsyncTester(IPersistence stubInterface)
        {
            this.stubInterface = stubInterface;
        }

        public async Task<string> LoadAsync(string name)
        {
            return await stubInterface.LoadAsync(name).ConfigureAwait(false);
        }

        public async Task<int> GetLoadCountAsync()
        {
            return await stubInterface.GetLoadCountAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(string name, string value)
        {
            await stubInterface.SaveAsync(name, value).ConfigureAwait(false);
        }
    }

    public interface IPersistence
    {
        Task<string> LoadAsync(string name);

        Task SaveAsync(string name, string value);

        Task<int> GetLoadCountAsync();
    }
}
