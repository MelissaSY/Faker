using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerTests
{
    public class NoConstructorClasses
    {
        private NoConstructorClasses() {}
    }
    public class ExceptionconstructorClass
    {
        public ExceptionconstructorClass() 
        {
            throw new NotImplementedException();
        }
    }
}
