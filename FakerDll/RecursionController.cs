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
        public RecursionController()
        {
            Types = new();
        }
        public bool CanGenerate(Type t)
        {
            bool canGenerate = true;
            if (!Types.ContainsKey(t))
            {
                Types.Add(t, 0);
            }
            if (this.Types[t] > 3)
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
