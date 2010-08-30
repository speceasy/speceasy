using System;
using Ninject.RhinoMocks;
using NUnit.Framework;

namespace TrackAbout.Mobile.ContextSpecifications
{
    [TestFixture]
    public abstract class BaseContextSpecification
    {
        private bool _shouldThrow = true;
        private Exception _exceptionThrown;
    	protected MockingKernel MockingKernel;

        [SetUp]
        public void BaseSetup()
        {
			MockingKernel = new MockingKernel();
            BeforeEachSpec();
            EstablishContext();
            try
            {
                When();
            }
            catch (Exception e)
            {
                _exceptionThrown = e;
                if (_shouldThrow) throw;
            }
        }

        [TearDown]
        public void BaseTearDown()
        {
            AfterEachSpec();
			MockingKernel.Reset();
        }

        protected virtual void BeforeEachSpec()
        {
        }

        protected virtual void AfterEachSpec()
        {
        }

        protected virtual Exception ExceptionThrown
        {
            get { return _exceptionThrown; }
        }

        protected virtual void DoNotThrowExceptions()
        {
            _shouldThrow = false;
        }

        protected virtual void EstablishContext()
        {
        }

        protected abstract void When();
    }
}