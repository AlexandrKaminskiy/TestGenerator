using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyser.GeneratedValuesInfo
{
    public class MethodParams
    {
        public string Name { get; set; }
        public TypeSyntax Type { get; }

        public MethodParams(string name, TypeSyntax type)
        {
            Name = name;
            Type = type;
        }
    }
}
