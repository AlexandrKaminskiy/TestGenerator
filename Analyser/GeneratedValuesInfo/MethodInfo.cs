using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGeneratorLibrary.GeneratedValuesInfo
{
    public class MethodInfo
    {
        public string Name { get; set; }
        public TypeSyntax ReturnType { get; set; }
        public List<MethodParameterInfo> Parameters { get; set; }

        public MethodInfo(string name, TypeSyntax returnType, List<MethodParameterInfo> parameters)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
        }
    }
}
