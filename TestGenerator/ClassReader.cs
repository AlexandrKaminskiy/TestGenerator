using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator
{
    public class ClassReader
    {
        string Dest { get; set; }   
        string Src { get; set; }
        public ClassReader(string dest, string src) 
        {
            Dest = dest;
            Src = src;
        }

        public List<string> fillFiles()
        {
            Directory.CreateDirectory(Dest);

            List<string> files = new List<string>();
            foreach (string path in Directory.GetFiles(Src, "*.cs"))
            {
                files.Add(path);
            }
            return files;
        }
    }
}
