using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HomeCenter.SourceGenerators
{
    internal class MethodDescription
    {
        public string MethodName { get; set; }

        public string ReturnType { get; set; }

        public string ReturnTypeGenericArgument { get; set; }

        public string ParameterType { get; set; }
        
        public AttributeSyntax Attribute { get; set; }
    }
}