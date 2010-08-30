using NUnit.Framework;
using Rhino.Mocks;

namespace TrackAbout.Mobile.ContextSpecifications
{
    [TestFixture]
    public abstract class RecordReplayContextSpecification : BaseContextSpecification
    {
        private MockRepository _mockRepositoryInstance;

        protected override void BeforeEachSpec()
        {
            base.BeforeEachSpec();
            ReplayAllIfNeeded();
        }

        private void ReplayAllIfNeeded()
        {
            if (_mockRepositoryInstance != null)
                _mockRepositoryInstance.ReplayAll();
        }

        public MockRepository MockRepositoryInstance
        {
            get
            {
                if (_mockRepositoryInstance == null)
                    _mockRepositoryInstance = new MockRepository();
                return _mockRepositoryInstance;
            }
        }
    }
}