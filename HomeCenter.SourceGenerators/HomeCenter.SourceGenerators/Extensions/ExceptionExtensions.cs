using System;

namespace HomeCenter.SourceGenerators
{
    internal static class ExceptionExtensions
    {
        public static string GenerateErrorSourceCode(this Exception exception)
        {
            var templateString = ResourceReader.GetResource("ErrorModel.cs");
            templateString = templateString.Replace("{Placeholder}", exception.ToString());
            return templateString;
        }
    }
}