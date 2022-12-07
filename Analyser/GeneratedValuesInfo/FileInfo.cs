namespace Analyser.GeneratedValuesInfo
{
    public class FileInfo
    {
        public string Name { get; set; }
        public string InnerData { get; set; }

        public FileInfo(string name, string content)
        {
            Name = name;
            InnerData = content;
        }
    }
}
