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

        private codeEditor.CodeEditor.AutocompleteItem newItem(string text, CodeDrawStyle.ColorType colorType)
        {
            return new codeEditor.CodeEditor.AutocompleteItem(text, CodeDrawStyle.ColorIndex(colorType), CodeDrawStyle.Color(colorType));
        }
        public virtual void AppendAutoCompleteItem( List<codeEditor.CodeEditor.AutocompleteItem> items)
        {
            foreach (Variables.Variable variable in Variables.Values)
            {
                if (variable is Variables.Net)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Net));
                }
                else if (variable is Variables.Reg)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Register));
                }
                else if (variable is Variables.Integer)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Variable));
                }
                else if (variable is Variables.Time || variable is Variables.Real || variable is Variables.RealTime || variable is Variables.Integer || variable is Variables.Genvar)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Variable));
                }
            }

            foreach (Variables.Parameter parameter in Module.Parameters.Values)
            {
                items.Add(newItem(parameter.Name, CodeDrawStyle.ColorType.Paramater));
            }

            foreach (Variables.Parameter parameter in Module.LocalParameters.Values)
            {
                items.Add(newItem(parameter.Name, CodeDrawStyle.ColorType.Paramater));
            }

            foreach (Function function in Module.Functions.Values)
            {
                items.Add(newItem(function.Name, CodeDrawStyle.ColorType.Identifier));
            }

            foreach (Task task in Module.Tasks.Values)
            {
                items.Add(newItem(task.Name, CodeDrawStyle.ColorType.Identifier));
            }

            foreach (NameSpace space in NameSpaces.Values)
            {
                if (space.Name == null) System.Diagnostics.Debugger.Break();
                items.Add(newItem(space.Name, CodeDrawStyle.ColorType.Identifier));
            }

            if(Parent != null)
            {
                Parent.AppendAutoCompleteItem(items);
            }
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
