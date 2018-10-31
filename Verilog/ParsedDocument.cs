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
