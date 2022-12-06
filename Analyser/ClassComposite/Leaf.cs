using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator.ClassComposite
{
    internal class Leaf : Element
    {
        public string Info { get; set; }

        public void add(Element element)
        {
            throw new NotImplementedException();
        }

        public string perform()
        {
            return Info;
        }

    }
}
