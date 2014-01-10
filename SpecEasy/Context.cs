using System.Threading.Tasks;
using System;

namespace SpecEasy
{
    public interface IContext
    {
        void Verify(Func<Task> addSpecs);

        void Verify(Action addSpecs);
    }

    internal class Context : IContext
    {
        private readonly Func<Task> setupAction = async delegate { };
        private Action enterAction = delegate { };

        public Context()
        {
        }

        public Context(Func<Task> setup)
        {
            setupAction = setup;            
        }

        public Context(Func<Task> setup, Action addSpecs)
        {
            setupAction = setup;
            enterAction = addSpecs;
        }

        public void Verify(Func<Task> addSpecs)
        {
            var cachedEnterAction = enterAction;
            enterAction = async () => { cachedEnterAction(); addSpecs(); };
        }

        public void Verify(Action addSpecs)
        {
            Verify(async () => addSpecs());
        }
        
        public void EnterContext()
        {
            enterAction();
        }

        public async Task SetupContext()
        {
            await setupAction().ConfigureAwait(false);
        }
    }
}