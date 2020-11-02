using System;

namespace HomeCenter.SourceGenerators
{
    internal class GeneratedSource
    {
        public GeneratedSource(string sourceCode, string fileName, Exception exception = null)
        {
            SourceCode = sourceCode;
            FileName = fileName;
            Exception = exception;
        }

        public string SourceCode { get; set; } 
        public string FileName { get; set; }
        public Exception Exception { get; }
    }
}