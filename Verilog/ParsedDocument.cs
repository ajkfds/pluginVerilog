﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class ParsedDocument : codeEditor.CodeEditor.ParsedDocument
    {
        public ParsedDocument(Data.IVerilogRelatedFile file): base(file as codeEditor.Data.TextFile,file.CodeDocument.EditID)
        {
            fileRef = new WeakReference<Data.IVerilogRelatedFile>(file);
        }

        private System.WeakReference<Data.IVerilogRelatedFile> fileRef;
        public Data.IVerilogRelatedFile File
        {
            get
            {
                Data.IVerilogRelatedFile ret;
                if (!fileRef.TryGetTarget(out ret)) return null;
                return ret;
            }
        }

        public bool Instance = false;
        public Dictionary<string, Module> Modules = new Dictionary<string, Module>();
        public Dictionary<string, Data.VerilogHeaderFile> IncludeFiles = new Dictionary<string, Data.VerilogHeaderFile>();
        public Dictionary<string, Macro> Macros = new Dictionary<string, Macro>();

        public void ReloadIncludeFiles()
        {
            foreach(var includeFile in IncludeFiles.Values)
            {
                includeFile.Close();
            }
        }

        public ProjectProperty ProjectProperty
        {
            get
            {
                Data.IVerilogRelatedFile file = File;
                if (file == null) return null;
                return file.ProjectProperty;
            }
        }

        public override void Dispose()
        {
            Data.IVerilogRelatedFile file = File;

            if (!Instance)
            {
                if (file is Data.VerilogFile)
                {
                    Data.VerilogFile verilogFile = file as Data.VerilogFile;
                    foreach (Verilog.Module module in Modules.Values)
                    {
                        verilogFile.ProjectProperty.RemoveModule(module.Name, verilogFile);
                    }
                }
                foreach (var includeFile in IncludeFiles.Values)
                {
                    includeFile.Close();
                }
            }
            base.Dispose();
        }

        public int ErrorCount = 0;
        public int WarningCount = 0;


        public List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int index,string text)
        {
            List<codeEditor.CodeEditor.PopupItem> ret = new List<codeEditor.CodeEditor.PopupItem>();

            // add messages
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
            if(text.StartsWith(".") && space is IModuleOrGeneratedBlock)
            {
                IModuleOrGeneratedBlock block = space as IModuleOrGeneratedBlock;
                ModuleItems.ModuleInstantiation inst = null;
                foreach (ModuleItems.ModuleInstantiation i in block.ModuleInstantiations.Values)
                {
                    if (index < i.BeginIndex) continue;
                    if (index > i.LastIndex) continue;
                    inst = i;
                    break;
                }
                if (inst != null)
                {
                    string portName = text.Substring(1);
                    Module originalModule = ProjectProperty.GetModule(inst.ModuleName);
                    if (originalModule == null) return ret;
                    if (!originalModule.Ports.ContainsKey(portName)) return ret;
                    Verilog.Variables.Port port = originalModule.Ports[portName];
                    ret.Add(new Popup.PortPopup(port));
                }
            }


            if (space.Variables.ContainsKey(text))          ret.Add(new Popup.VariablePopup(space.Variables[text]));
            if (space.LocalParameters.ContainsKey(text))    ret.Add(new Popup.ParameterPopup(space.LocalParameters[text]));
            if (space.Parameters.ContainsKey(text))         ret.Add(new Popup.ParameterPopup(space.Parameters[text]));

            if (text.StartsWith("`") && Macros.ContainsKey(text.Substring(1)))
            {
                ret.Add(new Popup.MacroPopup(text.Substring(1), Macros[text.Substring(1)].MacroText));
            }
            if (space.Module.Functions.ContainsKey(text))
            {
                ret.Add(new Popup.FunctionPopup(space.Module.Functions[text]));
            }
            if (space.Module.Tasks.ContainsKey(text))
            {
                ret.Add(new Popup.TaskPopup(space.Module.Tasks[text]));
            }
            return ret;
        }

        public Module GetModule(int index)
        {
            foreach (Module module in Modules.Values)
            {
                if (index < module.BeginIndex) continue;
                if (index > module.LastIndex) continue;
                return module;
            }
            return null;
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
            new Snippets.GenerateAutoCompleteItem("generate",CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword)),
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

        private NameSpace getSearchNameSpace(NameSpace nameSpace,List<string> hier,bool endWithDot)
        {
            if(nameSpace == null) return null;
            if (hier.Count == 0) return nameSpace;

            if (nameSpace.Module.ModuleInstantiations.ContainsKey(hier[0]))
            {
                Module module = ProjectProperty.GetModule(nameSpace.Module.ModuleInstantiations[hier[0]].ModuleName);
                hier.RemoveAt(0);
                return getSearchNameSpace(module,hier, endWithDot);
            }
            else if(nameSpace.NameSpaces.ContainsKey(hier[0]))
            {
                NameSpace space = nameSpace.NameSpaces[hier[0]];
                hier.RemoveAt(0);
                return getSearchNameSpace(space, hier, endWithDot);
            }
            if (endWithDot) return null;
            return nameSpace;
        }

        public List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index,int lineStartIndex,int line,CodeEditor.CodeDocument document,out string cantidateWord)
        {
            cantidateWord = null;
            
            List<codeEditor.CodeEditor.AutocompleteItem> items = null;

            // get current nameSpace
            NameSpace space = null;
            foreach (Module module in Modules.Values)
            {
                if (lineStartIndex < module.BeginIndex) continue;
                if (lineStartIndex > module.LastIndex) continue;
                space = module.GetHierNameSpace(lineStartIndex);
                break;
            }

            if (space == null) // external module/class/program
            {
                items = verilogKeywords.ToList();
                return items;
            }

            bool endWithDot;
            List<string> words = document.GetHierWords(index,out endWithDot);
            if(words.Count == 0)
            {
                return new List<codeEditor.CodeEditor.AutocompleteItem>();
            }
 //           if (document.GetCharAt(index - 1) == '.')
            if(endWithDot)
            { // after dot cantidate
                cantidateWord = "";
                items = new List<codeEditor.CodeEditor.AutocompleteItem>();
            }
            else if(words.Count <= 1)
            { // non hier word
                cantidateWord = words.LastOrDefault();
                if (cantidateWord.StartsWith("$"))
                {
                    items = new List<codeEditor.CodeEditor.AutocompleteItem>();
                    foreach(string key in ProjectProperty.SystemFunctions.Keys)
                    {
                        items.Add(
                            new codeEditor.CodeEditor.AutocompleteItem(key, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword))
                        );
                    }
                    foreach (string key in ProjectProperty.SystemTaskParsers.Keys)
                    {
                        items.Add(
                            new codeEditor.CodeEditor.AutocompleteItem(key, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword))
                        );
                    }
                }
                else
                {
                    items = verilogKeywords.ToList();
                }

                List<string> moduleNames = ProjectProperty.GetModuleNameList();
                foreach (string moduleName in moduleNames)
                {
                    items.Add(
                        new Snippets.ModuleInstanceAutocompleteItem(moduleName, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Keyword), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword), Project)
                    );
                }
            }
            else
            {
                cantidateWord = words.LastOrDefault();
                items = new List<codeEditor.CodeEditor.AutocompleteItem>();
                if(words.Count>2) words.RemoveAt(words.Count - 1);
            }

            for(int i = 0; i < words.Count;i++)
            {
                if (!words[i].StartsWith("`")) continue;
                
                string macroText = words[i].Substring(1);
                if (!Macros.ContainsKey(macroText)) continue;
                Macro macro = Macros[macroText];
                if (macro.MacroText.Contains(' ')) continue;
                if (macro.MacroText.Contains('\t')) continue;

                string[] swapTexts = macro.MacroText.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                words.RemoveAt(i);
                for(int j = swapTexts.Length - 1; j >= 0; j--)
                {
                    words.Insert(i, swapTexts[j]);
                }
            }

            NameSpace target = getSearchNameSpace(space, words, endWithDot);
            if(target != null) target.AppendAutoCompleteItem(items);

            return items;
        }


        private codeEditor.CodeEditor.AutocompleteItem newItem(string text, CodeDrawStyle.ColorType colorType)
        {
            return new codeEditor.CodeEditor.AutocompleteItem(text, CodeDrawStyle.ColorIndex(colorType), CodeDrawStyle.Color(colorType));
        }


        public new class Message : codeEditor.CodeEditor.ParsedDocument.Message
        {
            public Message(Data.IVerilogRelatedFile file,string text, MessageType type, int index, int lineNo,int length,codeEditor.Data.Project project)
            {
                this.fileRef = new WeakReference<Data.IVerilogRelatedFile>(file);
                this.Text = text;
                this.Length = length;
                this.Index = index;
                this.LineNo = lineNo;
                this.Type = type;
                this.Project = project;
            }

            private System.WeakReference<Data.IVerilogRelatedFile> fileRef;
            public Data.IVerilogRelatedFile File
            {
                get
                {
                    Data.IVerilogRelatedFile file;
                    if (!fileRef.TryGetTarget(out file)) return null;
                    return file;
                }
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
                MessageView.MessageNode node = new MessageView.MessageNode(File,this);
                return node;
            }

        }
    }

}
