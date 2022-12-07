namespace Analyser
{
    public class ClassInfo
    {
        public string Name { get; set; }
        public List<MethodInfo> Methods { get; set; }

        public ClassInfo(string name, List<MethodInfo> methods)
        {
            Name = name;
            Methods = methods;
        }
    }
}
