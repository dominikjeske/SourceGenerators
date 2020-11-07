using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace HomeCenter.SourceGenerators
{
    internal class SourceGeneratorOptions
    {
        public bool EnableLogging { get; set; }

        public bool DetailedLogging { get; set; }

        public List<AdditionalFilesOptions> AdditionalFilesOptions { get; set; } = new List<AdditionalFilesOptions>();

        public SourceGeneratorOptions(GeneratorExecutionContext context)
        {
            if (TryReadGlobalOption(context, "SourceGenerator_EnableLogging", out string enableLogging))
            {
                EnableLogging = bool.Parse(enableLogging);
            }

            if (TryReadGlobalOption(context, "SourceGenerator_DetailedLog", out string detailedLog))
            {
                DetailedLogging = bool.Parse(detailedLog);
            }

            foreach (var file in context.AdditionalFiles)
            {
                if (TryReadAdditionalFilesOption(context, file, "Type", out var type))
                {
                    AdditionalFilesOptions.Add(new SourceGenerators.AdditionalFilesOptions
                    {
                        Type = type,
                        AdditionalText = file
                    });
                }
            }
        }

        public bool TryReadGlobalOption(GeneratorExecutionContext context, string property, out string value)
        {
            return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{property}", out value);
        }

        public bool TryReadAdditionalFilesOption(GeneratorExecutionContext context, AdditionalText additionalText, string property, out string value)
        {
            return context.AnalyzerConfigOptions.GetOptions(additionalText).TryGetValue($"build_metadata.AdditionalFiles.{property}", out value);
        }
    }
}