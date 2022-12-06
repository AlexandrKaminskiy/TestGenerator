using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator.ClassComposite
{
    internal class Composite : Element
    {
        public string Info { set; get; }
        public List<Element> Elements { set; get; } = new List<Element>();

        public void add(Element element)
        {
            Elements.Add(element);
        }

        public void clear()
        {
            throw new NotImplementedException();
        }

        public string perform()
        {
            throw new NotImplementedException();
        }

        public void remove()
        {
            throw new NotImplementedException();
        }
    }
}
