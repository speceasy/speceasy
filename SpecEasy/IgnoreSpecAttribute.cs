using System;

namespace SpecEasy
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IgnoreSpecAttribute : Attribute
    {
    }
}
