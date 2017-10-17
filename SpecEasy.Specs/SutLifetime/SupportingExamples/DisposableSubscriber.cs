using System;

namespace SpecEasy.Specs.SutLifetime.SupportingExamples
{
    public sealed class DisposableSubscriber : IDisposable
    {
        public int UpdatedCount
        {
            get;
            private set;
        }

        public bool Disposed
        {
            get;
            private set;
        }

        private readonly IUpdateable updateable;

        public DisposableSubscriber(IUpdateable updateable)
        {
            this.updateable = updateable;

            this.updateable.Updated += UpdateableOnUpdated;
        }

        public void Dispose()
        {
            updateable.Updated -= UpdateableOnUpdated;
            Disposed = true;
        }

        private void UpdateableOnUpdated(object sender, EventArgs eventArgs)
        {
            UpdatedCount++;
        }
    }
}