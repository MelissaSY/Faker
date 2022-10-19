using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerTests
{
    public class TestClass
    {
        public string? TestString { get; }
        public string? TestString2 { get; }
        public string? TestString3 { get; set; }
        public string? TestString4 { get; set; }
        public TestClass()
        {     }
        public TestClass(string? testString)
        {
            this.TestString = testString;
        }
        public TestClass(string? testString, string? testString3, string? testString4) : this(testString)
        {
            TestString3 = testString3;
            TestString4 = testString4;
        }
        public TestClass(string? testString, string? testString2) : this(testString)
        {
            TestString2 = testString2;
        }
    }
}
