using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerTests
{
    public class A
    {
        public B? b; 
    }
    public class B
    {
        public C? c;
    }
    public class C
    {
        public A? a;
    }
    public class D
    {
        public D? d;
    }
    public class CyclicalListClass
    {
        public List<CyclicalListClass>? cyclicals;
    }
}

