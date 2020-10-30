namespace HomeCenter.SourceGenerators
{
    internal class ParameterDescriptor
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }

    internal class PropertyAssignDescriptor
    {
        public string Destination { get; set; }

        public string Source { get; set; }

        public string Type { get; set; }
    }

    internal static class ParameterDescriptorExtensions
    {
        public static ParameterDescriptor ToCamelCase(this PropertyAssignDescriptor parameterDescriptor)
        {
            return new ParameterDescriptor
            {
                Name = parameterDescriptor.Source.ToCamelCase(),
                Type = parameterDescriptor.Type
            };
        }
    }

    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}