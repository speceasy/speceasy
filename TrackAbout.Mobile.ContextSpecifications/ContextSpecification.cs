using System;
using Ninject;
using Ninject.Activation.Providers;
using Ninject.Planning.Bindings;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace TrackAbout.Mobile.ContextSpecifications
{
    [TestFixture]
    public abstract class ContextSpecification : BaseContextSpecification
    {
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
			T mock = Get<T>();
			mock.Raise(eventSubscription, args);
		}

		protected void AssertWasCalled<T>(Action<T> action)
		{
			T mock = Get<T>();
			mock.AssertWasCalled(action);
		}
	
		protected void AssertWasCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
		{
			T mock = Get<T>();
			mock.AssertWasCalled(action, methodOptions);
		}

		protected void AssertWasNotCalled<T>(Action<T> action)
		{
			T mock = Get<T>();
			mock.AssertWasNotCalled(action);
		}

		protected void AssertWasNotCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
		{
			T mock = Get<T>();
			mock.AssertWasNotCalled(action, methodOptions);
		}

        protected void Set<T>(T item)
        {
            var binding = new Binding(typeof (T))
                              {
                                  ProviderCallback = ctx => new ConstantProvider<T>(item)
                              };
            MockingKernel.AddBinding(binding);
        }
	}

	[TestFixture]
	public abstract class ContextSpecification<T>: ContextSpecification
	{
		protected T SUT;
		protected override void EstablishContext()
		{
			base.EstablishContext();
			SUT = Get<T>();
		}
	}
}