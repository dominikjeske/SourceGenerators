using System;

namespace HomeCenter.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ProxyCodeGeneratorAttribute : Attribute
    {
    }
}