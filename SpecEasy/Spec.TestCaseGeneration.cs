using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SpecEasy.Internal;

namespace SpecEasy
{
    [TestFixture]
    public partial class Spec
    {
        private List<Context> contexts = new List<Context>();
        private KeyValuePair<string, Func<Task>> when;
        private Dictionary<string, Func<Task>> thens = new Dictionary<string, Func<Task>>();

        private Exception thrownException;
        private bool exceptionAsserted;
        private bool hasCalledBefore;

        [Test, SpecTestCaseSource]
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
            CreateSpecMethodContexts();
            return BuildTestCasesForContexts(new List<Context>());
        }

        private void CreateSpecMethodContexts()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType != null ? type.BaseType.GetMethods() : new MethodInfo[] { };
            var declaredMethods = methods
				.Where(m => baseMethods.All(bm => bm.Name != m.Name))
                .Where(m => !m.GetParameters().Any() && m.ReturnType == typeof(void));

            foreach (var m in declaredMethods)
            {
                var method = m;
                Given(method.Name).Verify(() => method.Invoke(this, null));
            }
        }

        private IList<TestCaseData> BuildTestCasesForContexts(IList<Context> parentContexts)
        {
            var testCases = new List<TestCaseData>();

            var cachedWhen = when;
            foreach (var currentContext in contexts.ToList()) // make a copy of contexts list so we can reassign in the loop
            {
                contexts = new List<Context>();
                when = cachedWhen;
                thens = new Dictionary<string, Func<Task>>();

                currentContext.EnterContext();
                parentContexts.Add(currentContext);

                testCases.AddRange(BuildTestCasesForThens(parentContexts));
                testCases.AddRange(BuildTestCasesForContexts(parentContexts));

                parentContexts.Remove(currentContext);
            }

            return testCases;
        }

        private IList<TestCaseData> BuildTestCasesForThens(IList<Context> parentContexts)
        {
            var testCases = new List<TestCaseData>();

            if (!thens.Any()) return testCases;

            var setupText = new StringBuilder();

            setupText.AppendLine(parentContexts.First().Description + ":"); // start with the spec method's name

            var first = true;
            foreach (var context in parentContexts.Skip(1).Where(c => c.IsNamedContext))
            {
                setupText.AppendLine(context.Conjunction(first) + context.Description);
                first = false;
            }

            setupText.AppendLine("when " + when.Key);

            const string thenText = "then ";

            foreach (var spec in thens)
            {
                var parentContextsCapture = new List<Context>(parentContexts);
                var whenCapture = new KeyValuePair<string, Func<Task>>(when.Key, when.Value);

                Func<Task> executeTest = async () =>
                {
                    BeforeIfNotAlreadyRun();

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

        private async Task InitializeContext(IEnumerable<Context> contextList)
        {
            foreach (var context in contextList)
            {
                BeforeIfNotAlreadyRun();
                await context.SetupContext().ConfigureAwait(false);
            }
        }

        private void BeforeIfNotAlreadyRun()
        {
            if (hasCalledBefore)
            {
                return;
            }

            BeforeEachInit();
            BeforeEachExample();

            hasCalledBefore = true;
        }

        private void After()
        {
            if (hasCalledBefore)
            {
                AfterEachExample();
                hasCalledBefore = false;
            }
        }
    }
}