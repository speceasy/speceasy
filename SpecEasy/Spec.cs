using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SpecEasy
{
    [TestFixture]
    public class Spec : AssertionHelper
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
            return BuildTestCases(new List<KeyValuePair<string, Context>>(), 0);
        }

        private IList<TestCaseData> BuildTestCases(List<KeyValuePair<string, Context>> contextList, int depth)
        {
            var testCases = new List<TestCaseData>();

            var cachedWhen = when;
            foreach (var namedContext in contexts.Select(kvp => kvp))
            {
                var givenContext = namedContext.Value;
                then = new Dictionary<string, Func<Task>>();
                contexts = new Dictionary<string, Context>();
                when = cachedWhen;

                givenContext.EnterContext();

                if (depth > 0)
                    contextList.Add(namedContext);

                testCases.AddRange(BuildTestCases(contextList));
                testCases.AddRange(BuildTestCases(contextList, depth + 1));
                if (contextList.Any())
                    contextList.Remove(namedContext);
            }

            return testCases;
        }

        private IList<TestCaseData> BuildTestCases(List<KeyValuePair<string, Context>> contextList)
        {
            var testCases = new List<TestCaseData>();

            if (!then.Any()) return testCases;

            var setupText = new StringBuilder();

            var givenDescriptions = contextList.Select(kvp => kvp.Key).ToList();
            var givenText = "given ";
            foreach (var description in givenDescriptions.Where(IsNamedContext))
            {
                setupText.AppendLine(givenText + description);
                if (givenText == "given ")
                    givenText = Indent("and ", 1);
            }

            setupText.AppendLine("when " + when.Key);

            const string thenText = "then ";

            foreach (var spec in then)
            {
                var contextListCapture = new List<KeyValuePair<string, Context>>(contextList);
                var whenCapture = new KeyValuePair<string, Func<Task>>(when.Key, when.Value);

                Func<Task> executeTest = async () =>
                {
                    Before();

                    try
                    {
                        var exceptionThrownAndAsserted = false;

                        await InitializeContext(contextListCapture).ConfigureAwait(false);

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
        private Dictionary<string, Context> contexts = new Dictionary<string, Context>();
        private KeyValuePair<string, Func<Task>> when;

        protected IContext Given(string description, Action setup)
        {
            return Given(description, WrapAction(setup));
        }

        protected IContext Given(string description, Func<Task> setup)
        {
            if (contexts.ContainsKey(description))
            {
                return contexts[description] = new Context(ThrowDuplicateDescriptionException("given", description));
            }

            return contexts[description] = new Context(setup);
        }

        protected IContext Given(Action setup)
        {
            return Given(WrapAction(setup));
        }

        protected IContext Given(Func<Task> setup)
        {
            return contexts[CreateUnnamedContextName()] = new Context(setup);
        }

        protected IContext Given(string description)
        {
            return Given(description, () => { });
        }

        protected virtual IContext ForWhen(string description, Action action)
        {
            return contexts[CreateUnnamedContextName()] = new Context(async () => { }, () => When(description, action));
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

        private int nuSpecContextId;
        private string CreateUnnamedContextName()
        {
            return "SPECEASY" + (nuSpecContextId++).ToString(CultureInfo.InvariantCulture);
        }

        private static bool IsNamedContext(string name)
        {
            return !name.StartsWith("SPECEASY");
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

        private async Task InitializeContext(IEnumerable<KeyValuePair<string, Context>> contextList)
        {
            foreach (var action in contextList.Select(kvp => kvp.Value))
            {
                Before();
                await action.SetupContext().ConfigureAwait(false);
            }
        }

        private static string Indent(string text, int depth)
        {
            return new string(' ', depth * 2) + text;
        }
    }
}