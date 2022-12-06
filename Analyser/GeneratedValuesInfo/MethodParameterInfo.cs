using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGeneratorLibrary.GeneratedValuesInfo
{
    public class MethodParameterInfo
    {
        public string Name { get; set; }
        public TypeSyntax Type { get; }

        public MethodParameterInfo(string name, TypeSyntax type)
        {
            Name = name;
            Type = type;
        }
    }
}
