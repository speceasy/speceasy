using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace SpecEasy
{
    [TestFixture]
    public class Spec
    {
        [Test, TestCaseSource("TestCases")]
        public void Verify(Action test)
        {
            test();
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
                then = new Dictionary<string, Action>();
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
                var whenCapture = new KeyValuePair<string, Action>(when.Key, when.Value);

                Action executeTest = () =>
                {
                    Before();

                    try
                    {
                        var exceptionThrownAndAsserted = false;
                        InitializeContext(contextListCapture);

                        try
                        {
                            thrownException = null;
                            whenCapture.Value();
                        }
                        catch (Exception ex)
                        {
                            thrownException = ex;
                            spec.Value();

                            if (thrownException != null)
                            {
                                throw;
                            }

                            exceptionThrownAndAsserted = true;
                        }

                        if (!exceptionThrownAndAsserted)
                        {
                            spec.Value();
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

        protected void AssertWasThrown<T>(Action<T> expectation = null) where T : Exception
        {
            AssertWasThrown<T>(null);
        }
        
        protected void AssertWasThrown<T>(Action<T> expectation) where T : Exception
        {
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

        private Dictionary<string, Action> then = new Dictionary<string, Action>();
        private Dictionary<string, Context> contexts = new Dictionary<string, Context>();
        private KeyValuePair<string, Action> when;

        protected IContext Given(string description, Action setup)
        {
            if (contexts.ContainsKey(description)) throw new Exception("Reusing a given description");
            return contexts[description] = new Context(setup);
        }

        protected IContext Given(Action setup)
        {
            return contexts[CreateUnnamedContextName()] = new Context(setup);
        }

        protected IContext Given(string description)
        {
            return Given(description, () => { });
        }

        protected virtual IContext ForWhen(string description, Action action)
        {
            return contexts[CreateUnnamedContextName()] = new Context(() => { }, () => When(description, action));
        }

        protected virtual void When(string description, Action action)
        {
            when = new KeyValuePair<string, Action>(description, action);
        }

        protected IVerifyContext Then(string description, Action specification)
        {
            then[description] = specification;
            return new VerifyContext(Then);
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

        private void CreateMethodContexts()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType != null ? type.BaseType.GetMethods() : new MethodInfo[] {};
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

        private void InitializeContext(IEnumerable<KeyValuePair<string, Context>> contextList)
        {
            foreach (var action in contextList.Select(kvp => kvp.Value))
            {
                Before();
                action.SetupContext();
            }
        }

        private static string Indent(string text, int depth)
        {
            return new string(' ', depth * 2) + text;
        }
    }
}