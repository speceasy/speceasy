using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SpecEasy.Internal
{
    internal class Context : IContext
    {
        internal const string GivenConjunction = "given ";
        internal const string AndConjunction = "  and ";
        internal const string ButConjunction = "  but ";

        private const string UnnamedContextPrefix = "SPECEASY";
        private const string DefaultFirstConjunction = GivenConjunction;
        private const string DefaultJoiningConjunction = AndConjunction;

        private static int specContextId;
        private static string CreateUnnamedContextName()
        {
            return UnnamedContextPrefix + (specContextId++).ToString(CultureInfo.InvariantCulture);
        }

        internal string Description
        {
            get;
            private set;
        }

        internal bool IsNamedContext
        {
            get
            {
                return !Description.StartsWith(UnnamedContextPrefix);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private readonly Func<Task> setupAction = async delegate { };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        private readonly string conjunction;
        private Action enterAction = delegate { };

        public Context(Func<Task> setup, Action addSpecs, string description = null, string conjunction = null)
            : this(setup, description, conjunction)
        {
            enterAction = addSpecs;
        }

        public Context(Func<Task> setup, string description = null, string conjunction = null)
        {
            setupAction = setup;
            this.conjunction = conjunction;

            Description = description ?? CreateUnnamedContextName();
        }

        public void Verify(Action addSpecs)
        {
            var cachedEnterAction = enterAction;
            enterAction = () => { cachedEnterAction(); addSpecs(); };
        }

        internal void EnterContext()
        {
            enterAction();
        }

        internal async Task SetupContext()
        {
            await setupAction().ConfigureAwait(false);
        }

        internal string Conjunction(bool first)
        {
            return conjunction ?? (first ? DefaultFirstConjunction : DefaultJoiningConjunction);
        }
    }
}