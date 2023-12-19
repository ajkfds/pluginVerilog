using pluginVerilog.Verilog.BuildingBlocks;
using pluginVerilog.Verilog.DataObjects.Nets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class NameSpace : Item
    {
        private NameSpace() { }
        protected NameSpace(BuildingBlocks.BuildingBlock buildingBlock,NameSpace parent)
        {
            BuildingBlock = buildingBlock;
            Parent = parent;
        }

        public int BeginIndex = -1;
        public int LastIndex = -1;

        private Dictionary<string, DataObjects.IVariableOrNet> variables = new Dictionary<string, DataObjects.IVariableOrNet>();
        private Dictionary<string, Net> nets = new Dictionary<string, Net>();
        private Dictionary<string, DataObjects.Parameter> parameters = new Dictionary<string, DataObjects.Parameter>();
        private Dictionary<string, DataObjects.Parameter> localParameters = new Dictionary<string, DataObjects.Parameter>();
        private Dictionary<string, DataObjects.Variables.Enum> enums = new Dictionary<string, DataObjects.Variables.Enum>();
        private Dictionary<string, DataObjects.Variables.Typedef> typedefs = new Dictionary<string, DataObjects.Variables.Typedef>();

        private Dictionary<string, NameSpace> nameSpaces = new Dictionary<string, NameSpace>();

        public Dictionary<string, DataObjects.IVariableOrNet> Variables { get { return variables; } }


        public NameSpace Parent { get; protected set; }
        public Dictionary<string, DataObjects.Parameter> Parameters { get { return parameters; } }
        public Dictionary<string, DataObjects.Parameter> LocalParameters { get { return localParameters; } }
        public Dictionary<string, DataObjects.Variables.Enum> Enums { get { return enums; } }
        public Dictionary<string, DataObjects.Variables.Typedef> Typedefs { get { return typedefs; } }
        public BuildingBlocks.BuildingBlock BuildingBlock { get; protected set; }
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
            return new codeEditor.CodeEditor.AutocompleteItem(text, CodeDrawStyle.ColorIndex(colorType), Global.CodeDrawStyle.Color(colorType));
        }
        public virtual void AppendAutoCompleteItem( List<codeEditor.CodeEditor.AutocompleteItem> items)
        {
            //foreach (Net net in Nets.Values)
            //{
            //    if (net is Net)
            //    {
            //        items.Add(newItem(net.Name, CodeDrawStyle.ColorType.Net));
            //    }
            //}

            foreach (DataObjects.IVariableOrNet variable in Variables.Values)
            {
                if(variable is DataObjects.Nets.Net)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Net));
                }
                else if (variable is DataObjects.Variables.Reg)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Register));
                }
                else if (variable is DataObjects.Variables.Integer)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Variable));
                }
                else if (variable is DataObjects.Variables.Time || variable is DataObjects.Variables.Real || variable is DataObjects.Variables.Realtime || variable is DataObjects.Variables.Integer || variable is DataObjects.Variables.Genvar)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Variable));
                }
            }

            foreach (DataObjects.Parameter parameter in BuildingBlock.Parameters.Values)
            {
                items.Add(newItem(parameter.Name, CodeDrawStyle.ColorType.Paramater));
            }

            foreach (DataObjects.Parameter parameter in BuildingBlock.LocalParameters.Values)
            {
                items.Add(newItem(parameter.Name, CodeDrawStyle.ColorType.Paramater));
            }

            foreach (Function function in BuildingBlock.Functions.Values)
            {
                items.Add(newItem(function.Name, CodeDrawStyle.ColorType.Identifier));
            }

            foreach (Task task in BuildingBlock.Tasks.Values)
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

        public DataObjects.IVariableOrNet GetVariable(string identifier)
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

        public DataObjects.Parameter GetParameter(string identifier)
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

        private DataObjects.Parameter getParameterHier(string identifier)
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
