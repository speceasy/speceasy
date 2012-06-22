using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly Action setupAction;
        private Action enterAction;

        public Context()
            : this(() => { })
        {
        }

        public Context(Action setupAction)
        {
            this.setupAction = setupAction;            
        }

        public void Verify(Action verify)
        {
            enterAction = verify;
        }
        
        public void EnterContext()
        {
            enterAction();
        }

        public void SetupContext()
        {
            setupAction();
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

        protected Context Given(string description, Action setup)
        {
            if (contexts.ContainsKey(description)) throw new Exception("Reusing a given description");
            return contexts[description] = new Context(setup);
        }

        protected Context Given(Action setup)
        {
            return contexts[CreateUnnamedContextName()] = new Context(setup);
        }

        protected Context Given(string description)
        {
            return Given(description, () => { });
        }

        protected virtual void When(string description, Action action)
        {
            when = new KeyValuePair<string, Action>(description, action);
        }

        protected void Then(string description, Action specification)
        {
            then[description] = specification;
        }

        protected TUnit SUT 
        {
            get { return Get<TUnit>(); }
            set { Set(value); }
        }

        private int nuSpecContextId;
        private string CreateUnnamedContextName()
        {
            return "NUSPEC" + (nuSpecContextId++).ToString(CultureInfo.InvariantCulture);
        }

        private static bool IsNamedContext(string name)
        {
            return !name.StartsWith("NUSPEC");
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

                givenContext.EnterContext();

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
            var givenText = "given ";
            foreach (var description in givenDescriptions.Where(IsNamedContext))
            {
                output.AppendLine(givenText + description);
                if (givenText == "given ")
                    givenText = Indent("and ", 1);
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
                        action.SetupContext();
                    when.Value();
                    spec.Value();
                }
                catch (Exception ex)
                {
                    var failureMessage = Indent("FAILED! " + ex.Message, 1);
                    Console.Write(failurePrefix);
                    Console.WriteLine("then " + spec.Key);
                    Console.WriteLine(failureMessage);
                    Console.WriteLine();

                    output.AppendLine(failureMessage);
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