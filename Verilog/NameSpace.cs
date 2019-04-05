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

        private Dictionary<string, Variables.Variable> variables = new Dictionary<string, Variables.Variable>();
        private Dictionary<string, Variables.Parameter> parameters = new Dictionary<string, Variables.Parameter>();
        private Dictionary<string, Variables.Parameter> localParameters = new Dictionary<string, Variables.Parameter>();
        private Dictionary<string, NameSpace> nameSpaces = new Dictionary<string, NameSpace>();

        public Dictionary<string, Variables.Variable> Variables { get { return variables; } }
        public NameSpace Parent { get; protected set; }
        public Dictionary<string, Variables.Parameter> Parameters { get { return parameters; } }
        public Dictionary<string, Variables.Parameter> LocalParameters { get { return localParameters; } }
        public Module Module { get; protected set; }
        public Dictionary<string, NameSpace> NameSpaces { get { return nameSpaces;  } }

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
