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
        [Test]
        public void Verify()
        {
            CreateMethodContexts();
            VerifyContexts();

            System.Diagnostics.Debug.WriteLine("------------ FULL RESULTS ------------");
            System.Diagnostics.Debug.WriteLine(finalOutput);

            if (exceptions.Any())
                throw new Exception("Specifications failed!", exceptions[0]);
        }
        
        protected void AssertWasThrown<T>() where T : Exception
        {
            if (expectedException is T)
            {
                expectedException = null;
                return;
            }

            throw new Exception("Expected exception was not thrown");
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

        private readonly List<Exception> exceptions = new List<Exception>();
        private readonly StringBuilder finalOutput = new StringBuilder();
        private Exception expectedException;

        private void CreateMethodContexts()
        {
            var type = GetType();
            var methods = type.GetMethods();
            var baseMethods = type.BaseType != null ? type.BaseType.GetMethods() : new MethodInfo[] {};
            var declaredMethods = methods.Where(m => baseMethods.All(bm => bm.Name != m.Name));
            foreach (var m in declaredMethods)
            {
                var method = m;
                Given(method.Name).Verify(() => method.Invoke(this, null));
            }
        }

        private void VerifyContexts()
        {
            VerifyContexts(new List<KeyValuePair<string, Context>>(), 0);
        }

        private void VerifyContexts(List<KeyValuePair<string, Context>> contextList, int depth)
        {
            var cachedWhen = when;
            foreach (var namedContext in contexts.Select(kvp => kvp))
            {
                var givenContext = namedContext.Value;
                then = new Dictionary<string, Action>();
                contexts = new Dictionary<string, Context>();
                when = cachedWhen;

                InitializeContext(contextList);
                givenContext.EnterContext();

                if (depth > 0)
                    contextList.Add(namedContext);
                VerifySpecs(contextList);
                VerifyContexts(contextList, depth + 1);
                if (contextList.Any())
                    contextList.Remove(namedContext);
            }
        }

        private void VerifySpecs(List<KeyValuePair<string, Context>> contextList)
        {
            if (!then.Any()) return;

            var output = new StringBuilder();

            var givenDescriptions = contextList.Select(kvp => kvp.Key).ToList();
            var givenText = "given ";
            foreach (var description in givenDescriptions.Where(IsNamedContext))
            {
                output.AppendLine(givenText + description);
                if (givenText == "given ")
                    givenText = Indent("and ", 1);
            }

            output.AppendLine("when " + when.Key);

            var thenText = "then ";
            var failurePrefix = output.ToString();

            foreach (var spec in then)
            {
                Before();
                output.AppendLine(thenText + spec.Key);
                if (thenText == "then ")
                    thenText = Indent("and ", 1);

                try
                {
                    var exceptionThrownAndAsserted = false;
                    InitializeContext(contextList);

                    try
                    {
                        expectedException = null;
                        when.Value();
                    }
                    catch (Exception ex)
                    {
                        expectedException = ex;
                        spec.Value();

                        if (expectedException != null)
                        {
                            throw;
                        }

                        exceptionThrownAndAsserted = true;
                    }

                    if (exceptionThrownAndAsserted)
                    {
                        continue;
                    }

                    spec.Value();
                }
                catch (Exception ex)
                {
                    var failureMessage = Indent("FAILED! " + ex.Message, 1);
                    Console.Write(failurePrefix);
                    Console.WriteLine("then " + spec.Key);
                    Console.WriteLine(failureMessage);
                    Console.WriteLine();

                    output.AppendLine(failureMessage);
                    exceptions.Add(ex);
                }
                finally
                {
                    After();
                }
            }

            output.AppendLine();
            finalOutput.Append(output);
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
            InitializeTest();
            foreach (var action in contextList.Select(kvp => kvp.Value))
            {
                Before();
                action.SetupContext();
            }
        }

        protected virtual void InitializeTest()
        {
        }

        private static string Indent(string text, int depth)
        {
            return new string(' ', depth * 2) + text;
        }
    }
}