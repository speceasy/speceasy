using System;
using Ninject;
using Ninject.Activation.Providers;
using Ninject.MockingKernel;
using Ninject.MockingKernel.RhinoMock;
using Ninject.Planning.Bindings;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace SpecEasy
{
    public class Spec<TUnit> : Spec
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

        protected void Set<T>(T item)
        {
            var binding = new Binding(typeof(T))
            {
                ProviderCallback = ctx => new ConstantProvider<T>(item)
            };
            MockingKernel.AddBinding(binding);
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

        protected override void InitializeTest()
        {
            base.InitializeTest();
            MockingKernel = new RhinoMocksMockingKernel();
        }

        protected TUnit SUT
        {
            get { return Get<TUnit>(); }
            set { Set(value); }
        }
    }
}