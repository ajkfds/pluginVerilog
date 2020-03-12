using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;
using codeEditor.Data;

namespace pluginVerilog.Data
{
    public class VerilogModuleInstance : InstanceTextFile , IVerilogRelatedFile
    {
        protected VerilogModuleInstance(codeEditor.Data.TextFile sourceTextFile): base(sourceTextFile)
        {

        }
        public static VerilogModuleInstance Create(
            Verilog.ModuleItems.ModuleInstantiation moduleInstantiation,
            codeEditor.Data.Project project
            )
        {
            ProjectProperty projectPropery = project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            Data.IVerilogRelatedFile file = projectPropery.GetFileOfModule(moduleInstantiation.ModuleName);
            if (file == null) return null;

            VerilogModuleInstance fileItem = new VerilogModuleInstance(file as codeEditor.Data.TextFile);
            fileItem.ParameterOverrides = moduleInstantiation.ParameterOverrides;
            fileItem.Project = project;
            fileItem.RelativePath = file.RelativePath;
            fileItem.Name = moduleInstantiation.Name;
            fileItem.ModuleName = moduleInstantiation.ModuleName;
            fileItem.ParseRequested = true;

            if(file is Data.VerilogFile)
            {
                Data.VerilogFile vfile = file as Data.VerilogFile;
                vfile.RegisterModuleInstance(fileItem);
            }

            return fileItem;
        }


        public override codeEditor.CodeEditor.CodeDocument CodeDocument
        {
            get
            {
                return SourceVerilogFile.CodeDocument;
            }
        }
        public override void Dispose()
        {
            if (ParsedDocument != null && ParameterOverrides.Count != 0)
            { 
                foreach (var incFile in VerilogParsedDocument.IncludeFiles.Values)
                {
                    incFile.Dispose();
                }
            }
            //            CodeDocument = null;
            parsedDocument = null;
//            if (ParsedDocument != null) ParsedDocument.Dispose();
            SourceVerilogFile.RemoveModuleInstance(this);

            // base.Dispose(); do not call
        }

        public string ModuleName { set; get; }


        public Dictionary<string, Verilog.Expressions.Expression> ParameterOverrides;
        public string ParameterId { 
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach(var kvp in ParameterOverrides)
                {
                    sb.Append(kvp.Key);
                    sb.Append("=");
                    sb.Append(kvp.Value.CreateString());
                    sb.Append(",");
                }
                return sb.ToString();
            }
        }

        private Data.VerilogFile SourceVerilogFile
        {
            get
            {
                return SourceTextFile as VerilogFile;
            }
        }

        private volatile bool parseRequested = false;
        public override bool ParseRequested
        {
            get
            {
                if (ParameterOverrides.Count == 0)
                {
                    return SourceVerilogFile.ParseRequested;
                }
                else
                {
                    return parseRequested;
                }
            }
            set
            {
                if (ParameterOverrides.Count == 0)
                {
                    SourceVerilogFile.ParseRequested = value;
                }
                else
                {
                    parseRequested = value;
                }
            }
        }

        private volatile bool reloadRequested = false;
        public override bool CloseRequested
        {
            get
            {
                if (ParameterOverrides.Count == 0)
                {
                    return SourceVerilogFile.CloseRequested;
                }
                else
                {
                    return reloadRequested;
                }
            }
            set
            {
                if (ParameterOverrides.Count == 0)
                {
                    SourceVerilogFile.CloseRequested = value;
                }
                else
                {
                    reloadRequested = value;
                }
            }
        }


        public override void Close()
        {
            if (VerilogParsedDocument != null) VerilogParsedDocument.ReloadIncludeFiles();
            SourceVerilogFile.Close();
        }

        private codeEditor.CodeEditor.ParsedDocument parsedDocument = null;

        public override codeEditor.CodeEditor.ParsedDocument ParsedDocument
        {
            get
            {
                if(parsedDocument == null)
                {
                    if (ParameterOverrides.Count == 0)
                    {
                        Data.VerilogFile file = SourceVerilogFile;
                        if (file == null) return null;
                        parsedDocument = file.ParsedDocument;
                    }
                    else
                    {
                        Data.VerilogFile source = SourceVerilogFile;
                        parsedDocument = source.GetInstancedParsedDocument(ParameterId);
                    }
                }

                return parsedDocument;
            }
            set
            {
                parsedDocument = value;
            }
        }

        public Verilog.ParsedDocument VerilogParsedDocument
        {
            get
            {
                return ParsedDocument as Verilog.ParsedDocument;
            }
        }

        public override void AcceptParsedDocument(ParsedDocument newParsedDocument)
        {
            parsedDocument = newParsedDocument as Verilog.ParsedDocument;

            {
                Data.VerilogFile source = SourceVerilogFile;
                if (source == null) return;
                source.RegisterInstanceParsedDocument(ParameterId, newParsedDocument,this);
            }

            ParseRequested = false;
            Update();
        }



        public ProjectProperty ProjectProperty
        {
            get
            {
                return Project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            }
        }




        public override ajkControls.CodeDrawStyle DrawStyle
        {
            get
            {
                return Global.CodeDrawStyle;
            }
        }

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            NavigatePanel.VerilogModuleInstanceNode node = new NavigatePanel.VerilogModuleInstanceNode(this);
            nodeRef = new WeakReference<codeEditor.NavigatePanel.NavigatePanelNode>(node);
            return node;
        }

        public override codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode)
        {
            if(ParameterOverrides.Count == 0)
            {
                return new Parser.VerilogParser(this, parseMode);
            }
            else
            {
                if (Name == "TOP_0")
                {
                    string a = "";
                }
                return new Parser.VerilogParser(this, ParameterOverrides, parseMode);
            }
        }



        public override void Update()
        {
            if (VerilogParsedDocument == null)
            {
                // dispose all
                foreach (Item item in items.Values) item.Dispose();
                items.Clear();
                return;
            }

            List<Item> currentItems = new List<Item>();
            Dictionary<string, Item> newItems = new Dictionary<string, Item>();

            foreach (Item item in VerilogParsedDocument.IncludeFiles.Values)
            {
                currentItems.Add(item);
            }

            foreach (Verilog.Module module in VerilogParsedDocument.Modules.Values)
            {
                foreach (Verilog.ModuleItems.ModuleInstantiation moduleInstantiation in module.ModuleInstantiations.Values)
                {
                    if (items.Keys.Contains(moduleInstantiation.Name))
                    {
                        currentItems.Add(items[moduleInstantiation.Name]);
                    }
                    else
                    {
                        Item item = Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                        if (item != null & !newItems.ContainsKey(moduleInstantiation.Name))
                        {
                            item.Parent = this;
                            newItems.Add(moduleInstantiation.Name, item);
                            if (moduleInstantiation.ParameterOverrides.Count != 0)
                            {
                                Data.VerilogModuleInstance moduleInstance = item as Data.VerilogModuleInstance;

                                if (moduleInstance.ParsedDocument == null)
                                {
                                    Project.AddReparseTarget(item);
                                }
                                else
                                {
                                    string a = "";
                                }
                            }
                        }


                        //Item item = Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                        //if (item != null & !newItems.ContainsKey(moduleInstantiation.Name)) newItems.Add(moduleInstantiation.Name, item);
                    }
                }
            }

            List<Item> removeItems = new List<Item>();
            foreach (codeEditor.Data.Item item in items.Values)
            {
                if (!currentItems.Contains(item)) removeItems.Add(item);
            }

            foreach (Item item in removeItems)
            {
                items.Remove(item.Name);
                item.Dispose();
            }

            foreach (Item item in newItems.Values)
            {
                items.Add(item.Name, item);
//                Project.AddReparseTarget(item);
            }
        }

        public override void AfterKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (VerilogParsedDocument == null) return;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Return:
                    applyAutoInput();
                    break;
                case System.Windows.Forms.Keys.Space:
                    break;
                default:
                    break;
            }
        }

        public override void AfterKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (VerilogParsedDocument == null) return;
        }

        public override void BeforeKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        public override void BeforeKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
        }

        public override List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int editId, int index)
        {
            if (VerilogParsedDocument == null) return null;
            if (VerilogParsedDocument.EditID != editId) return null;

            int headIndex, length;
            CodeDocument.GetWord(index, out headIndex, out length);
            string text = CodeDocument.CreateString(headIndex, length);
            return VerilogParsedDocument.GetPopupItems(index, text);
        }


        public override List<codeEditor.CodeEditor.ToolItem> GetToolItems(int index)
        {
            List<codeEditor.CodeEditor.ToolItem> toolItems = new List<codeEditor.CodeEditor.ToolItem>();
            toolItems.Add(new Verilog.Snippets.AlwaysFFSnippet());
            toolItems.Add(new Verilog.Snippets.AutoConnectSnippet());
            //            toolItems.Add(new Verilog.Snippets.ConnectionCheckSnippet());
            toolItems.Add(new Verilog.Snippets.AutoFormatSnippet());
            return toolItems;
        }

        public override List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index, out string cantidateWord)
        {
            cantidateWord = null;

            if (VerilogParsedDocument == null) return null;
            int line = CodeDocument.GetLineAt(index);
            int lineStartIndex = CodeDocument.GetLineStartIndex(line);

            List<codeEditor.CodeEditor.AutocompleteItem> items = VerilogParsedDocument.GetAutoCompleteItems(index, lineStartIndex, line, (CodeEditor.CodeDocument)CodeDocument, out cantidateWord);
            return items;
        }



        private void applyAutoInput()
        {
            int index = CodeDocument.CaretIndex;
            int line = CodeDocument.GetLineAt(index);
            if (line == 0) return;

            int lineHeadIndex = CodeDocument.GetLineStartIndex(line);

            int prevTabs = 0;
            if(line != 1)
            {
                int prevLine = line - 1;
                int prevLineHeadIndex = CodeDocument.GetLineStartIndex(prevLine);
                for (int i = prevLineHeadIndex; i < lineHeadIndex; i++)
                {
                    char ch = CodeDocument.GetCharAt(i);
                    if (ch == '\t')
                    {
                        prevTabs++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            int indentLength = 0;
            for (int i = lineHeadIndex; i < CodeDocument.Length; i++)
            {
                char ch = CodeDocument.GetCharAt(i);
                if (ch == '\t')
                {
                    indentLength++;
                }
                else if (ch == ' ')
                {
                    indentLength++;
                }
                else
                {
                    break;
                }
            }


            bool prevBegin = isPrevBegin(lineHeadIndex);
            bool nextEnd = isNextEnd(lineHeadIndex);

            if (prevBegin)
            {
                if (nextEnd) // caret is sandwiched beteen begin and end
                {
                    // BEFORE
                    // begin[enter] end

                    // AFTER
                    // begin
                    //     [caret]
                    // end
                    CodeDocument.Replace(lineHeadIndex, indentLength, 0, new String('\t', prevTabs + 1) + "\r\n" + new String('\t', prevTabs));
                    CodeDocument.CaretIndex = CodeDocument.CaretIndex + prevTabs + 1 + 1 - indentLength;
                    return;
                }
                else
                {   // add indent
                    prevTabs++;
                }
            }

            CodeDocument.Replace(lineHeadIndex, indentLength, 0, new String('\t', prevTabs));
            CodeDocument.CaretIndex = CodeDocument.CaretIndex + prevTabs - indentLength;
        }

        private bool isPrevBegin(int index)
        {
            int prevInex = index;
            if (prevInex > 0) prevInex--;

            if (prevInex > 0 && CodeDocument.GetCharAt(prevInex) == '\n') prevInex--;
            if (prevInex > 0 && CodeDocument.GetCharAt(prevInex) == '\r') prevInex--;

            if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'n') return false;
            prevInex--;
            if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'i') return false;
            prevInex--;
            if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'g') return false;
            prevInex--;
            if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'e') return false;
            prevInex--;
            if (CodeDocument.GetCharAt(prevInex) != 'b') return false;
            return true;
        }

        private bool isNextEnd(int index)
        {
            int prevInex = index;
            if (prevInex < CodeDocument.Length &&
                (
                    CodeDocument.GetCharAt(prevInex) == ' ' || CodeDocument.GetCharAt(prevInex) == '\t'
                )
            ) prevInex++;

            if (prevInex >= CodeDocument.Length || CodeDocument.GetCharAt(prevInex) != 'e') return false;
            prevInex++;
            if (prevInex >= CodeDocument.Length || CodeDocument.GetCharAt(prevInex) != 'n') return false;
            prevInex++;
            if (CodeDocument.GetCharAt(prevInex) != 'd') return false;
            return true;
        }

    }
}
