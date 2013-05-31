namespace SpecEasy
{
    public interface IContext
    {
        void Verify(Action addSpecs);
    }

    internal class Context : IContext
    {
        private bool setupActionRun = false;
        private readonly Action setupAction = delegate { };
        private Action enterAction = delegate { };

        public Context()
        {
        }

        public Context(Action setup)
        {
            setupAction = setup;            
        }

        public Context(Action setup, Action addSpecs)
        {
            setupAction = setup;
            enterAction = addSpecs;
        }

        public void Verify(Action addSpecs)
        {
            var cachedEnterAction = enterAction;
            enterAction = () => { cachedEnterAction(); addSpecs(); };
        }
        
        public void EnterContext()
        {
            enterAction();
        }

        public void SetupContext()
        {
            if (setupActionRun) return;

            setupAction();
            setupActionRun = true;
        }
    }
}