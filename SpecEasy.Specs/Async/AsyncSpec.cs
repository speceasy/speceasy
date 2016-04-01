using System;
using System.Threading.Tasks;
using Rhino.Mocks;
using Should;

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

            Given("a value has been saved", () => Get<IPersistence>().Stub(persistence => persistence.LoadAsync(Name)).Return(Task.Factory.StartNew(() => Value))).Verify(() =>
            {
                Then("the desired value is loaded", () => result.ShouldEqual(Value));

                Given("the persistence tracks the load count", ()=> Get<IPersistence>().Stub(persistence => persistence.GetLoadCountAsync()).Return(Task.Factory.StartNew(()=>count))).Verify(()=>
                    Then("the number of loads can be retrieved", async () => count.ShouldEqual(await SUT.GetLoadCountAsync().ConfigureAwait(false))));
            });

            Given("there is an error loading the value", () => Get<IPersistence>().Stub(persistence => persistence.LoadAsync(Name)).Throw(new InvalidOperationException())).Verify(() =>
                Then("an exception is thrown", () => AssertWasThrown<InvalidOperationException>()));

            Given("the data was previously persisted", async () =>
            {
                Get<IPersistence>().Stub(persistence => persistence.SaveAsync(Arg<string>.Is.Equal(Name), Arg<string>.Is.Anything)).Return(Task.Factory.StartNew(() => { })).WhenCalled(x => intermediate = (string)x.Arguments[1]);
                await SUT.SaveAsync(Name, Value).ConfigureAwait(false);
            }).Verify(() =>
                Given("the data is then loaded", () => Get<IPersistence>().Stub(persistence => persistence.LoadAsync(Name)).Return(Task.Factory.StartNew(() => intermediate))).Verify(() =>
                    Then("the previously stored data is retrieved", () => result.ShouldEqual(Value))));
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
