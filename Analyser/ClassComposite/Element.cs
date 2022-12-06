using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator.ClassComposite
{
    internal interface Element
    {
        public void add(Element element);

        public string perform();
    }
}
