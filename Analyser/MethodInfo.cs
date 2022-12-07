using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyser
{
    public class MethodInfo
    {
        public string Name { get; set; }
        public TypeSyntax ReturnType { get; set; }
        public List<MethodParams> Params { get; set; }

        public MethodInfo(string name, TypeSyntax returnType, List<MethodParams> parameters)
        {
            Name = name;
            ReturnType = returnType;
            Params = parameters;
        }
    }
}
