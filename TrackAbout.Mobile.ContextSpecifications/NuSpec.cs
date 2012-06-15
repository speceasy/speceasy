using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ninject;
using Ninject.Activation.Providers;
using Ninject.Planning.Bindings;
using Ninject.RhinoMocks;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace TrackAbout.Mobile.NuSpec
{
    public class WhenAction
    {
        public WhenAction(Action before)
        {
            this.before = before;
        }
        public Action before { get; set; }
        public Action verify { get; set; }
    }

    public class WhenDictionary : Dictionary<string, WhenAction>
    {
        public WhenAction this[string key, Action before]
        {
            get
            {
                if (base[key] == null) throw new Exception("Reusing a when description");
                return base[key] = new WhenAction(before);
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

        protected Dictionary<string, Action> it = new Dictionary<string, Action>();
        protected Dictionary<string, Action> when = new Dictionary<string, Action>();
        protected Action before = delegate { };
        protected Action act = delegate { };

        protected TUnit SUT;

        private readonly List<Exception> exceptions = new List<Exception>();

        [Test]
        public void Examples()
        {
            SetupWhenMethods();
            RunWhenSpecs(before);

            if (exceptions.Any())
                throw new Exception("Specifications failed!", exceptions[0]);
        }

        private void SetupWhenMethods()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType.GetMethods();
            var declaredMethods = methods.Where(m => baseMethods.All(bm => bm.Name != m.Name));
            foreach (var m in declaredMethods)
            {
                var method = m;
                when[method.Name] = () => method.Invoke(this, null);
            }
        }

        private void RunWhenSpecs(Action beforeStack)
        {
            RunWhenSpecs(beforeStack, 0);
        }

        private void RunWhenSpecs(Action beforeStack, int depth)
        {
            var whenContexts = when.Select(kvp => kvp);
            var cachedAct = act;
            foreach (var ctx in whenContexts)
            {
                it = new Dictionary<string, Action>();
                when = new Dictionary<string, Action>();
                before = delegate { };
                act = cachedAct;
                ctx.Value();
                Console.WriteLine(Indent("when " + ctx.Key, depth));
                var cachedBefore = before;
                Action newBeforeStack = () => { beforeStack(); cachedBefore(); };
                RunSpecs(newBeforeStack, depth + 1);
                RunWhenSpecs(newBeforeStack, depth + 1);
            }
        }

        private void RunSpecs(Action beforeStack, int depth)
        {
            foreach (var spec in it)
            {
                Console.WriteLine(Indent("it " + spec.Key, depth));
                MockingKernel = new MockingKernel();
                try
                {
                    beforeStack();
                    SUT = Get<TUnit>();
                    act();
                    spec.Value();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Indent("FAILED!", depth + 1));
                    exceptions.Add(ex);
                }
            }
        }

        private static string Indent(string text, int depth)
        {
            return new string(' ', depth * 2) + text;
        }
    }
}