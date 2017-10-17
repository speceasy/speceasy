using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecEasy.Internal;

namespace SpecEasy
{
    public partial class Spec
    {
        internal virtual void BeforeEachInit() { }

        protected virtual void BeforeEachExample() { }

        protected virtual void AfterEachExample() { }

        protected IContext Given(string description)
        {
            return Given(description, () => { });
        }

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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var context = new Context(async () => { }, () => When(description, action));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
            if (thens.ContainsKey(description))
            {
                thens[description] = ThrowDuplicateDescriptionException("then", description);
            }
            else
            {
                thens[description] = specification;
            }

            return new VerifyContext(Then);
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
            {
                var message = string.Format("Expected exception of type {0} was not thrown: {1}",
                    typeof(T).FullName,
                    thrownException != null ? string.Format("an exception of type {0} was thrown instead (see inner exception for details).", thrownException.GetType().FullName) : "no exception was thrown."
                );
                throw new Exception(message, thrownException);
            }

            if (expectation != null)
            {
                try
                {
                    expectation(expectedException);
                }
                catch (Exception expectationException)
                {
                    var message = string.Format(
                        "The expected exception of type {0} was thrown but the specified constraint failed. Constraint Exception: {1}{2}",
                        typeof(T).FullName,
                        Environment.NewLine,
                        expectationException.Message
                    );
                    throw new Exception(message, expectationException);
                }
            }

            thrownException = null;
        }

        private static Func<Task> ThrowDuplicateDescriptionException(string typeOfDuplicate, string description)
        {
            var stackTrace = Environment.StackTrace;
            return () => { throw new DuplicateDescriptionException(typeOfDuplicate, description, stackTrace); };
        }

        private static Func<Task> WrapAction(Action action)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            return async () => action();
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }
    }
}