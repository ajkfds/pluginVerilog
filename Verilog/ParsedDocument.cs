using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    
    public class ParsedDocument : codeEditor.CodeEditor.ParsedDocument
    {
        public ParsedDocument(codeEditor.Data.Project project,string itemID,int editID): base(project,itemID,editID)
        {

        }
        public Dictionary<string, Module> Modules = new Dictionary<string, Module>();
        public Dictionary<string, Data.VerilogHeaderFile> IncludeFiles = new Dictionary<string, Data.VerilogHeaderFile>();
        public Dictionary<string, string> Macros = new Dictionary<string, string>();

        public ProjectProperty ProjectProperty
        {
            get
            {
                return Project.ProjectProperties[Plugin.StaticID] as ProjectProperty; 
            }
        }

        public override void Accept()
        {
            base.Accept();

            Data.VerilogFile verilogFile = Project.GetRegisterdItem(ItemID) as Data.VerilogFile;
            foreach(Verilog.Module module in Modules.Values)
            {
                bool suceed = verilogFile.ProjectProperty.RegisterModule(verilogFile.RelativePath,module.Name);
                if (!suceed)
                {
                    // add module name error
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            Data.VerilogFile verilogFile = Project.GetRegisterdItem(ItemID) as Data.VerilogFile;
            foreach (Verilog.Module module in Modules.Values)
            {
                verilogFile.ProjectProperty.RemoveModule(verilogFile.RelativePath, module.Name);
            }
        }

        public List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int index,string text)
        {
            List<codeEditor.CodeEditor.PopupItem> ret = new List<codeEditor.CodeEditor.PopupItem>();
            foreach (Message message in Messages)
            {
                if (index < message.Index) continue;
                if (index > message.Index + message.Length) continue;
                switch (message.Type)
                {
                    case Message.MessageType.Error:
                        ret.Add(new codeEditor.CodeEditor.PopupItem(message.Text, System.Drawing.Color.Red, Global.Icons.ExclamationBox, ajkControls.IconImage.ColorStyle.Red));
                        break;
                    case Message.MessageType.Warning:
                        ret.Add(new codeEditor.CodeEditor.PopupItem(message.Text, System.Drawing.Color.Orange, Global.Icons.ExclamationBox, ajkControls.IconImage.ColorStyle.Orange));
                        break;
                    case Message.MessageType.Hint:
                        ret.Add(new codeEditor.CodeEditor.PopupItem(message.Text, System.Drawing.Color.Blue, Global.Icons.ExclamationBox, ajkControls.IconImage.ColorStyle.Blue));
                        break;
                    case Message.MessageType.Notice:
                        ret.Add(new codeEditor.CodeEditor.PopupItem(message.Text, System.Drawing.Color.Green, Global.Icons.ExclamationBox, ajkControls.IconImage.ColorStyle.Green));
                        break;
                }
            }

            NameSpace space = null;
            foreach(Module module in Modules.Values)
            {
                if (index < module.BeginIndex) continue;
                if (index > module.LastIndex) continue;
                space = module.GetHierNameSpace(index);
                break;
            }
            if (space == null) return ret;
            if (space.Variables.ContainsKey(text))
            {
                ret.Add(new Variables.VariablePopup(space.Variables[text]));
            }

            return ret;
        }


        private static List<codeEditor.CodeEditor.AutocompleteItem> verilogKeywords = new List<codeEditor.CodeEditor.AutocompleteItem>()
        {
            new codeEditor.CodeEditor.AutocompleteItem("always",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("and",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("assign",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("automatic",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new Snippets.BeginAutoCompleteItem("begin",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("case",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("casex",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("casez",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("deassign",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("default",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("defparam",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("design",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("disable",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("edge",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("else",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("end",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endcase",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endfunction",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endgenerate",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endmodule",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endspecify",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endtask",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("endprimitive",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("for",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("force",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("forever",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("fork",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new Snippets.FunctionAutocompleteItem("function",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("generate",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("genvar",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("if",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("incdir",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("include",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("initial",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("inout",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("input",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("integer",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("join",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("localparam",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new Snippets.ModuleAutocompleteItem("module",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("nand",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("negedge",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("nor",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("not",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("or",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("output",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("parameter",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("posedge",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("pulldown",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("pullup",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("real",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("realtime",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("reg",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("release",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("repeat",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("signed",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("time",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new Snippets.TaskAutocompleteItem("task",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("tri0",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("tri1",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("trireg",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("unsigned",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("vectored",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("wait",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("weak0",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("weak1",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("while",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new codeEditor.CodeEditor.AutocompleteItem("wand",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("wire",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("wor",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

            new Snippets.NonBlockingAssignmentAutoCompleteItem("<=",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Normal), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
        };

        public List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index,int lineStartIndex,int line)
        {
            


            List<codeEditor.CodeEditor.AutocompleteItem> items = verilogKeywords.ToList();

            NameSpace space = null;
            foreach (Module module in Modules.Values)
            {
                if (lineStartIndex < module.BeginIndex) continue;
                if (lineStartIndex > module.LastIndex) continue;
                space = module.GetHierNameSpace(lineStartIndex);
                break;
            }
            if (space == null) return items;

            appendAutoCompleteItems(items, space);

            List<string> moduleNameList = ProjectProperty.GetModuleNameList();
            foreach(string moduleName in moduleNameList)
            {
                items.Add(new Snippets.ModuleInstanceAutocompleteItem(
                    moduleName, 
                    CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Identifier), 
                    CodeDrawStyle.Color(CodeDrawStyle.ColorType.Identifier),
                    Project
                    )
                );
            }

            return items;
        }

        private void appendAutoCompleteItems(List<codeEditor.CodeEditor.AutocompleteItem> items, NameSpace nameSpace)
        {
            foreach (Variables.Variable variable in nameSpace.Variables.Values)
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

            foreach (Variables.Parameter parameter in nameSpace.Module.Parameters.Values)
            {
                items.Add(newItem(parameter.Name, CodeDrawStyle.ColorType.Paramater));
            }

            foreach (Variables.Parameter parameter in nameSpace.Module.LocalParameters.Values)
            {
                items.Add(newItem(parameter.Name, CodeDrawStyle.ColorType.Paramater));
            }

            foreach (Function function in nameSpace.Module.Functions.Values)
            {
                items.Add(newItem(function.Name, CodeDrawStyle.ColorType.Identifier));
            }

            foreach (Task task in nameSpace.Module.Tasks.Values)
            {
                items.Add(newItem(task.Name, CodeDrawStyle.ColorType.Identifier));
            }
        }

        private codeEditor.CodeEditor.AutocompleteItem newItem(string text, CodeDrawStyle.ColorType colorType)
        {
            return new codeEditor.CodeEditor.AutocompleteItem(text, CodeDrawStyle.ColorIndex(colorType), CodeDrawStyle.Color(colorType));
        }


        public new class Message : codeEditor.CodeEditor.ParsedDocument.Message
        {
            public Message(string text, MessageType type, int index, int lineNo,int length,string itemID,codeEditor.Data.Project project)
            {
                this.Text = text;
                this.Length = length;
                this.Index = index;
                this.LineNo = lineNo;
                this.Type = type;
                this.ItemID = itemID;
                this.Project = project;
            }
            public int LineNo { get; protected set; }

            public enum MessageType
            {
                Error,
                Warning,
                Notice,
                Hint
            }
            public MessageType Type { get; protected set; }

            public override codeEditor.MessageView.MessageNode CreateMessageNode()
            {
                MessageView.MessageNode node = new MessageView.MessageNode(this);

                return node;
            }

        }
    }

}
