using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class RecursionController
    {
        public Dictionary<Type, int> Types { get; }
        public static int MinDepth { get { return 0; } }
        public static int MaxDepth { get { return 10; } }
        public RecursionController()
        {
            Types = new();
        }
        public bool CanGenerate(Type t, int maxDepth)
        {
            bool canGenerate = true;
            if (!Types.ContainsKey(t))
            {
                Types.Add(t, 0);
            }
            if (this.Types[t] > maxDepth)
            {
                canGenerate = false;
            }
            else { Types[t]++; }
            return canGenerate;
        }
        public void GenerationFinished(Type t)
        {
            this.Types[t]--;
            if (this.Types[t] == 0)
            {
                this.Types.Remove(t);
            }
        }
    }
}
