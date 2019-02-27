using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class NameSpace : Item
    {
        private NameSpace() { }
        protected NameSpace(Module module,NameSpace parent)
        {
            Module = module;
            Parent = parent;
        }

        public int BeginIndex = -1;
        public int LastIndex = -1;

        public Dictionary<string, Variables.Variable> Variables = new Dictionary<string, Variables.Variable>();
        public NameSpace Parent { get; protected set; }
        public Dictionary<string, Variables.Parameter> Parameters = new Dictionary<string, Variables.Parameter>();
        public Dictionary<string, Variables.Parameter> LocalParameters = new Dictionary<string, Variables.Parameter>();
        public Module Module { get; protected set; }
        public Dictionary<string, NameSpace> NameSpaces = new Dictionary<string, NameSpace>();

        public NameSpace GetHierNameSpace(int index)
        {
            foreach(NameSpace subSpace in NameSpaces.Values)
            {
                if (index < subSpace.BeginIndex) continue;
                if (index > subSpace.LastIndex) continue;
                return subSpace.GetHierNameSpace(index);
            }
            return this;
        }

        public Variables.Variable GetVariable(string identifier)
        {
            if (Variables.ContainsKey(identifier))
            {
                return Variables[identifier];
            }

            if (Parent != null)
            {
                return Parent.GetVariable(identifier);
            }

            return null;
        }


        public Variables.Parameter GetParameter(string identifier)
        {
            if (Parameters.ContainsKey(identifier))
            {
                return Parameters[identifier];
            }
            if (LocalParameters.ContainsKey(identifier))
            {
                return LocalParameters[identifier];
            }

            if (Parent != null)
            {
                return Parent.getParameterHier(identifier);
            }

            return null;
        }

        private Variables.Parameter getParameterHier(string identifier)
        {
            if (Parameters.ContainsKey(identifier))
            {
                return Parameters[identifier];
            }
            if (LocalParameters.ContainsKey(identifier))
            {
                return LocalParameters[identifier];
            }

            if (Parent != null)
            {
                return Parent.getParameterHier(identifier);
            }

            return null;
        }
    }
}
