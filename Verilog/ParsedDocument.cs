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
                ret.Add(new codeEditor.CodeEditor.PopupItem(message.Text, System.Drawing.Color.Red, Style.ExclamationBoxIcon,ajkControls.Icon.ColorStyle.Red));
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
 //           new codeEditor.CodeEditor.AutocompleteItem("begin",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

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
//            new codeEditor.CodeEditor.AutocompleteItem("begin",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("force",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("forever",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("fork",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
            new codeEditor.CodeEditor.AutocompleteItem("function",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
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
            new codeEditor.CodeEditor.AutocompleteItem("module",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
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
            new codeEditor.CodeEditor.AutocompleteItem("task",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),

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
            new codeEditor.CodeEditor.AutocompleteItem("wor",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword))
        };

        public List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index)
        {
            List<codeEditor.CodeEditor.AutocompleteItem> items = new List<codeEditor.CodeEditor.AutocompleteItem>();
            items = verilogKeywords.ToList();

            /*
            buf
            bufif0
            bufif1

            cell
            cmos
            config

            
              
              endconfig 
                endtable
                
                highz0 highz1
             ifnone 
              
             instance   large liblist library  macromodule medium    
             nmos  noshowcancelled  
             notif0 notif1    pmos  primitive pull0 pull1   
             pulsestyle_onevent pulsestyle_ondetect rcmos 
              
              rnmos rpmos rtran rtranif0 rtranif1 scalared showcancelled
             small specify specparam strong0 strong1 supply0 supply1 table  
             tran tranif0 tranif1 tri   triand trior 
              use
                    xnor xor
            */
            NameSpace space = null;
            foreach (Module module in Modules.Values)
            {
                if (index < module.BeginIndex) continue;
                if (index > module.LastIndex) continue;
                space = module.GetHierNameSpace(index);
                break;
            }
            if (space == null) return items;

            foreach(Variables.Variable variable in space.Variables.Values)
            {
                if(variable is Variables.Net)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Net));
                }else if(variable is Variables.Reg)
                {
                    items.Add(newItem(variable.Name, CodeDrawStyle.ColorType.Register));
                }
            }

            return items;
        }
        private codeEditor.CodeEditor.AutocompleteItem newItem(string text, CodeDrawStyle.ColorType colorType)
        {
            return new codeEditor.CodeEditor.AutocompleteItem(text, CodeDrawStyle.ColorIndex(colorType), CodeDrawStyle.Color(colorType));
        }


        public new class Message : codeEditor.CodeEditor.ParsedDocument.Message
        {
            public Message(string text, MessageType type, int index, int length,string itemID,codeEditor.Data.Project project)
            {
                this.Text = text;
                this.Length = length;
                this.Index = index;
                this.Type = type;
                this.ItemID = itemID;
                this.Project = project;
            }

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
