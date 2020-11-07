using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HomeCenter.SourceGenerators
{
    internal class SourceGeneratorLogger<T> : IDisposable, ISourceGeneratorLogger where T : ISourceGenerator
    {
        private const int LineSurfixLenght = 20;
        private const int LineLenght = 100;

        private readonly Stopwatch _loggerStopwatch;
        private readonly GeneratorExecutionContext _executionContext;
        private readonly SourceGeneratorOptions _options;

        public string LogPath { get; }

        public SourceGeneratorLogger(GeneratorExecutionContext generatorExecutionContext, SourceGeneratorOptions options)
        {
            _executionContext = generatorExecutionContext;
            _options = options;

            if (!options.EnableLogging) return;

            _loggerStopwatch = new Stopwatch();
            _loggerStopwatch.Start();
            LogPath = GetLogFlileLocation();

            WriteHeader();
        }

        public void LogGeneratedSource(ClassDeclarationSyntax classDeclaration, GeneratedSource generatedSource)
        {
            if (!_options.EnableLogging) return;

            var sb = new StringBuilder();
            sb.AppendLine($"-> Generated class for '{classDeclaration.Identifier.Text}':{Environment.NewLine}");

            if (generatedSource.Exception != null)
            {
                sb.AppendLine(generatedSource.Exception.ToString());
            }
            else
            {
                sb.AppendLine(generatedSource.SourceCode);
            }

            sb.AppendLine("");

            WriteLog(sb.ToString());
        }

        public void LogMessage(string message)
        {
            if (!_options.EnableLogging) return;

            WriteLog(message);
        }

        public void Dispose()
        {
            if (!_options.EnableLogging) return;

            _loggerStopwatch.Stop();

            var summary = GetTextWithLine($"END [{typeof(T).Name} | {_loggerStopwatch.Elapsed:g}] ");

            WriteLog(summary);
        }

        private string GetLogFlileLocation()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "obj");
            var logfile = Path.Combine(directory, $"{typeof(T).Name}.log");
            return logfile;
        }

        private void WriteHeader()
        {
            var sb = new StringBuilder();
            var header = GetTextWithLine($" [{typeof(T).Name} | {DateTime.Now:g}] ");

            sb.AppendLine(header);
            sb.AppendLine();

            sb.AppendLine($"-> Language: {_executionContext.ParseOptions.Language}");
            sb.AppendLine($"-> Kind: {_executionContext.ParseOptions.Kind}");
            TryReadOptions(sb);

            sb.Append("-> Additional files:");
            if (_executionContext.AdditionalFiles.Length > 0)
            {
                sb.AppendLine();
                foreach (var additionalFile in _executionContext.AdditionalFiles)
                {
                    sb.AppendLine(additionalFile.Path);
                }
            }
            else
            {
                sb.Append(" NONE");
            }

            WriteLog(sb.ToString());
        }

        private void TryReadOptions(StringBuilder sb)
        {
            try
            {
                var values = ReadOptions(_executionContext.AnalyzerConfigOptions.GlobalOptions);

                sb.AppendLine("-> Global options:");
                foreach (var value in values)
                {
                    sb.AppendLine($"\t{value.Key}:{value.Value}");
                }

                var info2 = _executionContext.AnalyzerConfigOptions
                                             .GetType()
                                             .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                             .FirstOrDefault(g => g.Name == "_treeDict");

                if (info2.GetValue(_executionContext.AnalyzerConfigOptions) is ImmutableDictionary<object, AnalyzerConfigOptions> options)
                {
                    sb.AppendLine("-> Options:");

                    foreach (var optionKey in options.Keys)
                    {
                        string keyContext = "";
                        if (optionKey is AdditionalText text)
                        {
                            keyContext = text.Path;
                        }
                        else if (optionKey is SyntaxTree syntaxTree)
                        {
                            keyContext = syntaxTree.FilePath;
                        }

                        sb.AppendLine($"- {optionKey.GetType().Name}[{keyContext}]:");

                        var optionsList = ReadOptions(options[optionKey]);
                        foreach (var value in optionsList)
                        {
                            sb.AppendLine($"\t{value.Key}:{value.Value}");
                        }
                    }
                }
            }
            catch (Exception)
            {
                sb.Append("-> Options: Error when try use reflection to load");
            }
        }

        private ImmutableDictionary<string, string> ReadOptions(AnalyzerConfigOptions analyzerConfigOptions)
        {
            var backing = analyzerConfigOptions.GetType()
                                               .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                               .FirstOrDefault(g => g.Name == "_backing");

            return backing.GetValue(analyzerConfigOptions) as ImmutableDictionary<string, string>;
        }

        private void WriteLog(string logtext)
        {
            File.AppendAllText(LogPath, $"{logtext}{Environment.NewLine}");
        }

        private string GetTextWithLine(string context)
        {
            return new string('-', LineSurfixLenght) + context + new string('-', LineLenght - LineSurfixLenght - context.Length);
        }
    }
}