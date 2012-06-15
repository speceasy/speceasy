using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ninject;
using Ninject.Activation.Providers;
using Ninject.Planning.Bindings;
using Ninject.RhinoMocks;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace TrackAbout.Mobile.NuSpec
{
    public class GivenAction
    {
        public GivenAction(Action given)
        {
            this.given = given;
        }
        public Action given { get; set; }
        public Action verify { get; set; }
        public void SetupContext()
        {
            verify();
        }
    }

    public class GivenDictionary : Dictionary<string, GivenAction>
    {
        public GivenAction this[string key, Action given]
        {
            get
            {
                if (ContainsKey(key)) throw new Exception("Reusing a given description");
                return base[key] = new GivenAction(given);
            }
        }
        public new GivenAction this[string key]
        {
            get
            {
                if (ContainsKey(key)) throw new Exception("Reusing a given description");
                return base[key] = new GivenAction(() => { });
            }
        }
    }

    [TestFixture]
    public class NuSpec<TUnit>
    {
        protected MockingKernel MockingKernel;

        protected T Mock<T>() where T : class
        {
            return MockRepository.GenerateMock<T>();
        }

        protected T Get<T>()
        {
            return MockingKernel.Get<T>();
        }

        protected void Raise<T>(Action<T> eventSubscription, params object[] args) where T : class
        {
            var mock = Get<T>();
            mock.Raise(eventSubscription, args);
        }

        protected void AssertWasCalled<T>(Action<T> action)
        {
            var mock = Get<T>();
            mock.AssertWasCalled(action);
        }

        protected void AssertWasCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            var mock = Get<T>();
            mock.AssertWasCalled(action, methodOptions);
        }

        protected void AssertWasNotCalled<T>(Action<T> action)
        {
            var mock = Get<T>();
            mock.AssertWasNotCalled(action);
        }

        protected void AssertWasNotCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            var mock = Get<T>();
            mock.AssertWasNotCalled(action, methodOptions);
        }

        protected void Set<T>(T item)
        {
            var binding = new Binding(typeof(T))
                              {
                                  ProviderCallback = ctx => new ConstantProvider<T>(item)
                              };
            MockingKernel.AddBinding(binding);
        }

        private Dictionary<string, Action> then = new Dictionary<string, Action>();
        private GivenDictionary given = new GivenDictionary();
        private KeyValuePair<string, Action> when;

        protected void Then(string description, Action specification)
        {
            then[description] = specification;
        }
        protected GivenAction Given(string description, Action setup)
        {
            return given[description, setup];
        }
        protected GivenAction Given(string description)
        {
            return given[description, () => { }];
        }
        protected void When(string description, Action action)
        {
            when = new KeyValuePair<string, Action>(description, action);
        }

        protected TUnit SUT;

        private readonly List<Exception> exceptions = new List<Exception>();
        private readonly StringBuilder finalOutput = new StringBuilder();

        [Test]
        public void Examples()
        {
            SetupMethods();
            RunWhenSpecs();

            Console.WriteLine("------------ FULL RESULTS ------------");
            Console.WriteLine(finalOutput);
            if (exceptions.Any())
                throw new Exception("Specifications failed!", exceptions[0]);
        }

        private void SetupMethods()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType.GetMethods();
            var declaredMethods = methods.Where(m => baseMethods.All(bm => bm.Name != m.Name));
            foreach (var m in declaredMethods)
            {
                var method = m;
                given[method.Name, () => { }].verify = () => method.Invoke(this, null);
            }
        }

        private void RunWhenSpecs()
        {
            RunWhenSpecs(new Stack<KeyValuePair<string, GivenAction>>(), 0);
        }

        private void RunWhenSpecs(Stack<KeyValuePair<string, GivenAction>> givenStack, int depth)
        {
            var givenContexts = given.Select(kvp => kvp);
            var cachedWhen = when;
            foreach (var ctx in givenContexts)
            {
                var givenContext = ctx.Value;
                then = new Dictionary<string, Action>();
                given = new GivenDictionary();
                when = cachedWhen;

                givenContext.SetupContext();

                if (depth > 0)
                    givenStack.Push(ctx);
                RunSpecs(givenStack, depth + 1);
                RunWhenSpecs(givenStack, depth + 1);
                if (givenStack.Any())
                    givenStack.Pop();
            }
        }

        private void RunSpecs(Stack<KeyValuePair<string, GivenAction>> givenStack, int depth)
        {
            if (!then.Any()) return;

            var output = new StringBuilder();

            var givenDescriptions = givenStack.Reverse().Select(kvp => kvp.Key).ToList();
            if (givenDescriptions.Any())
            {
                output.AppendLine("given " + givenDescriptions.First());
                foreach (var description in givenDescriptions.Skip(1))
                    output.AppendLine(Indent("and " + description, 1));
            }
            output.AppendLine("when " + when.Key);

            var thenText = "then ";
            var failurePrefix = output.ToString();
            foreach (var spec in then)
            {
                output.AppendLine(thenText + spec.Key);
                if (thenText == "then ")
                    thenText = Indent("and ", 1);

                MockingKernel = new MockingKernel();
                try
                {
                    foreach (var action in givenStack.Select(kvp => kvp.Value))
                        action.given();
                    SUT = Get<TUnit>();
                    when.Value();
                    spec.Value();
                }
                catch (Exception ex)
                {
                    Console.Write(failurePrefix);
                    Console.WriteLine("then " + spec.Key);
                    Console.WriteLine(Indent("FAILED!", 1));
                    Console.WriteLine();

                    output.AppendLine(Indent("FAILED!", 1));
                    exceptions.Add(ex);
                }
            }

            output.AppendLine();
            finalOutput.Append(output.ToString());
        }

        private static string Indent(string text, int depth)
        {
            return new string(' ', depth * 2) + text;
        }
    }
}