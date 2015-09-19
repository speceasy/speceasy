using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;

namespace SpecEasy.Specs.ThenInEachExample
{
    internal sealed class ThenInEachExampleSpec : Spec<FooBarFinder>
    {
        private const string FirstFileName  = "990802E27C7F42D9A7F2AEA70A8B7EF0";
        private const string SecondFileName = "36A50420BBC84CB1BB3E27E1DE43A64C";
        private const string ThirdFileName  = "36E1295292F34D8DA465E6D00D5323D8";

        private IList<string> fileNames;

        public void WithThenInEachExample()
        {
            When("running tests", () => SUT.Run(fileNames));

            Given("there are no file names in the list", () => {}).Verify(() =>
            {
                Then("the sink is not started", () => AssertWasNotCalled<ISink>(sink => sink.Start()));
                Then("foo is not called", () => AssertWasNotCalled<ISink>(sink => sink.Foo()));
                Then("bar is not called", () => AssertWasNotCalled<ISink>(sink => sink.Bar()));
                Then("stop is not called", () => AssertWasNotCalled<ISink>(sink => sink.Stop()));
            });

            Given("there are three file names in the list", () => fileNames = new []{FirstFileName, SecondFileName, ThirdFileName}).Verify(() =>
            {
                ThenInEachExample("start is called on the sink", () => AssertWasCalled<ISink>(sink => sink.Start()));
                ThenInEachExample("stop is called on the sink", () => AssertWasCalled<ISink>(sink => sink.Stop()));

                Given("the first file's contents are foo", () => Get<IFileSystem>().Stub(fileSystem => fileSystem.Read(FirstFileName)).Return("foo")).Verify(() =>
                {
                    ThenInEachExample("foo is called on the sink only once", () => AssertWasCalled<ISink>(sink => sink.Foo(), options => options.Repeat.Once()));

                    Given("the second file's contents are bar", () => Get<IFileSystem>().Stub(fileSystem => fileSystem.Read(SecondFileName)).Return("bar")).Verify(() =>
                    {
                        Then("bar is called on the sink", () => AssertWasCalled<ISink>(sink => sink.Bar()));
                        Then("the third file is not read", () => AssertWasNotCalled<IFileSystem>(fileSystem => fileSystem.Read(ThirdFileName)));
                    });

                    Given("the second file's contents are foo", () => Get<IFileSystem>().Stub(fileSystem => fileSystem.Read(SecondFileName)).Return("foo")).Verify(() =>
                    {
                        Given("the third file is empty", () => Get<IFileSystem>().Stub(fileSystem => fileSystem.Read(ThirdFileName)).Return(string.Empty)).Verify(() =>
                            Then("bar is not called on the sink", () => AssertWasNotCalled<ISink>(sink => sink.Bar())));

                        Given("the third file's contents are bar", () => Get<IFileSystem>().Stub(fileSytem => fileSytem.Read(ThirdFileName)).Return("bar")).Verify(() =>
                            Then("bar is called on the sink", () => AssertWasCalled<ISink>(sink => sink.Bar())));
                    });
                });
            });
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();

            fileNames = new List<string>();
        }
    }

    internal class FooBarFinder
    {
        private readonly ISink sink;
        private readonly IFileSystem fileSystem;

        public FooBarFinder(ISink sink, IFileSystem fileSystem)
        {
            this.sink = sink;
            this.fileSystem = fileSystem;
        }

        public void Run(IEnumerable<string> fileNames)
        {
            var hasFooBeenSeen = false;
            var hasBarBeenSeen = false;

            if (!fileNames.Any())
            {
                return;
            }

            sink.Start();

            try
            {
                foreach (var fileName in fileNames)
                {
                   var fileContents = fileSystem.Read(fileName);

                    var foo = fileContents.Contains("foo");
                    var bar = fileContents.Contains("bar");

                    if (!hasFooBeenSeen && foo)
                    {
                        sink.Foo();
                        hasFooBeenSeen = true;
                    }

                    if (!hasBarBeenSeen && bar)
                    {
                        sink.Bar();
                        hasBarBeenSeen = true;
                    }

                    if (hasFooBeenSeen && hasBarBeenSeen)
                    {
                        return;
                    }
                }
            }
            finally
            {
                sink.Stop();
            }
        }
    }

    public interface IFileSystem
    {
        string Read(string fileName);
    }

    public interface ISink
    {
        void Start();

        void Foo();

        void Bar();

        void Stop();
    }
}