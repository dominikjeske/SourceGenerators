﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace HomeCenter.SourceGenerators
{
    internal class MethodDescription
    {
        public string MethodName { get; set; }

        public string ReturnType { get; set; }

        public string ReturnTypeGenericArgument { get; set; }

        public string ParameterType { get; set; }

        public bool IsReturnTask => string.CompareOrdinal(ReturnType, nameof(Task)) == 0;

        public AttributeSyntax Attribute { get; set; }
    }
}