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
    public class Context
    {
        public Context()
        {
            given = () => { };
        }

        public Context(Action given)
        {
            this.given = given;            
        }
        public Action given { get; set; }

        private Action setup;

        public void Verify(Action verify)
        {
            setup = verify;
        }
        
        public void SetupContext()
        {
            setup();
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
        private Dictionary<string, Context> contexts = new Dictionary<string, Context>();
        private KeyValuePair<string, Action> when;

        protected void Then(string description, Action specification)
        {
            then[description] = specification;
        }
        protected Context Given(string description, Action setup)
        {
            if (contexts.ContainsKey(description)) throw new Exception("Reusing a given description");
            return contexts[description] = new Context(setup);
        }
        protected Context Given(string description)
        {
            return Given(description, () => { });
        }

        protected void When(string description, Action action)
        {
            when = new KeyValuePair<string, Action>(description, action);
        }

        protected TUnit SUT 
        {
            get { return Get<TUnit>(); }
            set { Set(value); }
        }


        private readonly List<Exception> exceptions = new List<Exception>();
        private readonly StringBuilder finalOutput = new StringBuilder();

        [Test]
        public void Verify()
        {
            CreateMethodContexts();
            VerifyContexts();

            Console.WriteLine("------------ FULL RESULTS ------------");
            Console.WriteLine(finalOutput);
            if (exceptions.Any())
                throw new Exception("Specifications failed!", exceptions[0]);
        }

        private void CreateMethodContexts()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType.GetMethods();
            var declaredMethods = methods.Where(m => baseMethods.All(bm => bm.Name != m.Name));
            foreach (var m in declaredMethods)
            {
                var method = m;
                Given(method.Name).Verify(() => method.Invoke(this, null));
            }
        }

        private void VerifyContexts()
        {
            VerifyContexts(new Stack<KeyValuePair<string, Context>>(), 0);
        }

        private void VerifyContexts(Stack<KeyValuePair<string, Context>> contextStack, int depth)
        {
            var cachedWhen = when;
            foreach (var namedContext in contexts.Select(kvp => kvp))
            {
                var givenContext = namedContext.Value;
                then = new Dictionary<string, Action>();
                contexts = new Dictionary<string, Context>();
                when = cachedWhen;

                givenContext.SetupContext();

                if (depth > 0)
                    contextStack.Push(namedContext);
                VerifySpecs(contextStack);
                VerifyContexts(contextStack, depth + 1);
                if (contextStack.Any())
                    contextStack.Pop();
            }
        }

        private void VerifySpecs(Stack<KeyValuePair<string, Context>> contextStack)
        {
            if (!then.Any()) return;

            var output = new StringBuilder();

            var givenDescriptions = contextStack.Reverse().Select(kvp => kvp.Key).ToList();
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
                    foreach (var action in contextStack.Select(kvp => kvp.Value))
                        action.given();
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