using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Execution;

namespace SpecEasy.Specs
{
    public static class TestBuilder
    {
        public static ITestResult RunParameterizedMethodSuite(Type type, string methodName) => RunTest(MakeParameterizedMethodSuite(type, methodName), Reflect.Construct(type));

        private static TestSuite MakeParameterizedMethodSuite(Type type, string methodName) => MakeTestFromMethod(type, methodName) as TestSuite;

        private static Test MakeTestFromMethod(Type type, string methodName) => new DefaultTestCaseBuilder().BuildFrom(new MethodWrapper(type, type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)));

        private static ITestResult RunTest(ITest test, object testObject) => ExecuteWorkItem(PrepareWorkItem(test, testObject));

        private static WorkItem PrepareWorkItem(ITest test, object testObject)
        {
            var testExecutionContext = new TestExecutionContext
            {
                TestObject = testObject,
                Dispatcher = new SuperSimpleDispatcher()
            };

            var workItem = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty, true);
            workItem.InitializeContext(testExecutionContext);

            return workItem;
        }

        private static ITestResult ExecuteWorkItem(WorkItem workItem)
        {
            workItem.Execute();

            while (workItem.State != WorkItemState.Complete)
            {
                Thread.Sleep(1);
            }

            return workItem.Result;
        }

        private sealed class SuperSimpleDispatcher : IWorkItemDispatcher
        {
            public void Start(WorkItem topLevelWorkItem) => topLevelWorkItem.Execute();

            public void Dispatch(WorkItem work) => work.Execute();

            public void CancelRun(bool force) => throw new NotSupportedException();
        }
    }
}