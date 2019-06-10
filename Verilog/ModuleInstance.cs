using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class ModuleInstance :　IDisposable 
    {
        protected ModuleInstance() { }

        public ModuleInstance Create(
            Module targetModule,
            Dictionary<string,Expressions.Expression> parameterAssignments)
        {
            ModuleInstance mi = new ModuleInstance();
            return mi;
        }

        public void Dispose()
        {

        }
    }
}
