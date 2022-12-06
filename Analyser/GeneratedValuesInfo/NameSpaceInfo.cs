namespace TestGeneratorLibrary.GeneratedValuesInfo
{
    public class NameSpaceInfo
    {
        public string Name { get; set; }
        public List<ClassInfo> Classes { get; set; }

        public NameSpaceInfo(string name, List<ClassInfo> classes)
        {
            Name = name;
            Classes = classes;
        }   
    }
}
