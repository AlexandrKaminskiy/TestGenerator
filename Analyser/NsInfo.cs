namespace Analyser
{
    public class NsInfo
    {
        public string Name { get; set; }
        public List<ClassInfo> ClassInfos { get; set; }

        public NsInfo(string name, List<ClassInfo> classes)
        {
            Name = name;
            ClassInfos = classes;
        }
    }
}
