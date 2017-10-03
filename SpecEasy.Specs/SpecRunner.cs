using NUnit.Framework.Interfaces;

namespace SpecEasy.Specs
{
    public static class SpecRunner
    {
        public static ITestResult Run<T>() where T : Spec => TestBuilder.RunParameterizedMethodSuite(typeof(T), "Verify");
    }
}
