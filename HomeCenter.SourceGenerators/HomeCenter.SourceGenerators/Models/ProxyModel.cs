using System.Collections.Generic;

namespace HomeCenter.SourceGenerators
{
    internal record ProxyModel
    {
        public string ClassName { get; set; }

        public string Namespace { get; set; }

        public string ClassBase { get; set; }

        public List<string> Usings { get; set; } = new List<string>();

        public List<MethodDescription> Commands { get; set; } = new List<MethodDescription>();

        public List<MethodDescription> Queries { get; set; } = new List<MethodDescription>();

        public List<MethodDescription> Events { get; set; } = new List<MethodDescription>();

    };
}