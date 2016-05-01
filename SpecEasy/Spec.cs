using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SpecEasy
{
    [TestFixture]
    public class Spec
    {
        [Test, TestCaseSource("TestCases")]
        public async Task Verify(Func<Task> test)
        {
            await test().ConfigureAwait(false);
        }

        public IList<TestCaseData> TestCases
        {
            get
            {
                return BuildTestCases();
            }
        }

        private IList<TestCaseData> BuildTestCases()
        {
            CreateMethodContexts();
            return BuildTestCasesForContexts(new List<Context>(), 0);
        }

        private IList<TestCaseData> BuildTestCasesForContexts(IList<Context> parentContexts, int depth)
        {
            var testCases = new List<TestCaseData>();

            var cachedWhen = when;
            foreach (var currentContext in contexts.ToList()) // make a copy of contexts list so we can reassign in the loop
            {
                then = new Dictionary<string, Func<Task>>();
                contexts = new List<Context>();
                when = cachedWhen;

                currentContext.EnterContext();

                if (depth > 0)
                    parentContexts.Add(currentContext);

                testCases.AddRange(BuildTestCasesForThens(parentContexts));
                testCases.AddRange(BuildTestCasesForContexts(parentContexts, depth + 1));

                if (parentContexts.Any())
                    parentContexts.Remove(currentContext);
            }

            return testCases;
        }

        private IList<TestCaseData> BuildTestCasesForThens(IList<Context> parentContexts)
        {
            var testCases = new List<TestCaseData>();

            if (!then.Any()) return testCases;

            var setupText = new StringBuilder();

            var first = true;
            foreach (var context in parentContexts.Where(c => c.IsNamedContext))
            {
                setupText.AppendLine(context.Conjunction(first) + context.Description);
                first = false;
            }

            setupText.AppendLine("when " + when.Key);

            const string thenText = "then ";

            foreach (var spec in then)
            {
                var parentContextsCapture = new List<Context>(parentContexts);
                var whenCapture = new KeyValuePair<string, Func<Task>>(when.Key, when.Value);

                Func<Task> executeTest = async () =>
                {
                    Before();

                    try
                    {
                        var exceptionThrownAndAsserted = false;

                        await InitializeContext(parentContextsCapture).ConfigureAwait(false);

                        try
                        {
                            thrownException = null;
                            await whenCapture.Value().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            thrownException = ex;
                        }

                        try
                        {
                            exceptionAsserted = false;
                            await spec.Value().ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            if (thrownException == null || exceptionAsserted)
                                throw;
                        }

                        if (thrownException != null)
                        {
                            throw thrownException;
                        }

                        exceptionThrownAndAsserted = true;

                        if (!exceptionThrownAndAsserted)
                        {
                            await spec.Value().ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        After();
                    }
                };

                var description = setupText + thenText + spec.Key + Environment.NewLine;
                testCases.Add(new TestCaseData(executeTest).SetName(description));
            }

            return testCases;
        }

        protected void AssertWasThrown<T>() where T : Exception
        {
            AssertWasThrown<T>(null);
        }

        protected void AssertWasThrown<T>(Action<T> expectation) where T : Exception
        {
            exceptionAsserted = true;
            var expectedException = thrownException as T;
            if (expectedException == null)
                throw new Exception("Expected exception was not thrown");

            if (expectation != null)
            {
                try
                {
                    expectation(expectedException);
                }
                catch (Exception exc)
                {
                    var message = string.Format("The expected exception type was thrown but the specified constraint failed. Constraint Exception: {0}{1}",
                        Environment.NewLine, exc.Message);
                    throw new Exception(message, exc);
                }
            }

            thrownException = null;
        }

        private Dictionary<string, Func<Task>> then = new Dictionary<string, Func<Task>>();
        private List<Context> contexts = new List<Context>();
        private KeyValuePair<string, Func<Task>> when;

        protected IContext Given(string description, Action setup)
        {
            return Given(description, WrapAction(setup));
        }

        protected IContext Given(string description, Func<Task> setup)
        {
            return Given(description, setup, null);
        }

        private IContext Given(string description, Func<Task> setup, string conjunction)
        {
            Context context;
            var duplicateDescriptionContext = contexts.FirstOrDefault(c => c.Description == description);
            if (duplicateDescriptionContext != null)
            {
                context = new Context(ThrowDuplicateDescriptionException("context", description), description, conjunction);
                contexts.Remove(duplicateDescriptionContext);
            }
            else
            {
                context = new Context(setup, description, conjunction);
            }

            contexts.Add(context);
            return context;
        }

        protected IContext Given(Action setup)
        {
            return Given(WrapAction(setup));
        }

        protected IContext Given(Func<Task> setup)
        {
            var context = new Context(setup);
            contexts.Add(context);
            return context;
        }

        protected IContext Given(string description)
        {
            return Given(description, () => { });
        }

        protected IContext And(string description, Action setup)
        {
            return And(description, WrapAction(setup));
        }

        protected IContext And(string description, Func<Task> setup)
        {
            return Given(description, setup, Context.AndConjunction);
        }

        protected IContext And(string description)
        {
            return And(description, () => { });
        }

        protected IContext But(string description, Action setup)
        {
            return But(description, WrapAction(setup));
        }

        protected IContext But(string description, Func<Task> setup)
        {
            return Given(description, setup, Context.ButConjunction);
        }

        protected IContext But(string description)
        {
            return But(description, () => { });
        }

        protected virtual IContext ForWhen(string description, Action action)
        {
            var context = new Context(async () => { }, () => When(description, action));
            contexts.Add(context);
            return context;
        }

        protected virtual void When(string description, Action action)
        {
            When(description, WrapAction(action));
        }

        protected virtual void When(string description, Func<Task> func)
        {
            when = new KeyValuePair<string, Func<Task>>(description, func);
        }

        protected IVerifyContext Then(string description, Action specification)
        {
            return Then(description, WrapAction(specification));
        }

        protected IVerifyContext Then(string description, Func<Task> specification)
        {
            if (then.ContainsKey(description))
            {
                then[description] = ThrowDuplicateDescriptionException("then", description);
            }
            else
            {
                then[description] = specification;
            }

            return new VerifyContext(Then);
        }

        private static Func<Task> ThrowDuplicateDescriptionException(string typeOfDuplicate, string description)
        {
            var stackTrace = Environment.StackTrace;
            return () => { throw new DuplicateDescriptionException(typeOfDuplicate, description, stackTrace); };
        }

        private static Func<Task> WrapAction(Action action)
        {
            return async () => action();
        }

        private Exception thrownException;
        private bool exceptionAsserted;

        private void CreateMethodContexts()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType != null ? type.BaseType.GetMethods() : new MethodInfo[] { };
            var declaredMethods = methods.Where(m => baseMethods.All(bm => bm.Name != m.Name))
                .Where(m => !m.GetParameters().Any() && m.ReturnType == typeof(void));
            foreach (var m in declaredMethods)
            {
                var method = m;
                Given(method.Name).Verify(() => method.Invoke(this, null));
            }
        }

        private bool hasCalledBefore;
        private void Before()
        {
            if (!hasCalledBefore)
            {
                BeforeEachExample();
                hasCalledBefore = true;
            }
        }

        private void After()
        {
            if (hasCalledBefore)
            {
                AfterEachExample();
                hasCalledBefore = false;
            }
        }

        protected virtual void BeforeEachExample() { }
        protected virtual void AfterEachExample() { }

        private async Task InitializeContext(IEnumerable<Context> contextList)
        {
            foreach (var context in contextList)
            {
                Before();
                await context.SetupContext().ConfigureAwait(false);
            }
        }
    }
}