using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;
using codeEditor.Data;

namespace pluginVerilog.Data
{
    public class VerilogHeaderInstance : InstanceTextFile, IVerilogRelatedFile
    {
        protected VerilogHeaderInstance(codeEditor.Data.TextFile sourceTextFile) : base(sourceTextFile)
        {

        }

        public static VerilogHeaderInstance Create(
            string relativePath,
            IVerilogRelatedFile parentFile,
            codeEditor.Data.Project project,
            string id)
        {
            ProjectProperty projectPropery = project.ProjectProperties[Plugin.StaticID] as ProjectProperty;
            codeEditor.Data.Item fileItem = project.GetItem(relativePath);
            if ((fileItem as VerilogHeaderFile) == null) return null;

            VerilogHeaderInstance instance = new VerilogHeaderInstance(fileItem as VerilogHeaderFile);
            instance.Project = project;
            instance.RelativePath = relativePath;
            if (instance.RelativePath.Contains('\\'))
            {
                instance.Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
            }
            else
            {
                instance.Name = relativePath;
            }
            instance.id = id;
            instance.RootFile = parentFile;

            //if (file is Data.VerilogFile)
            //{
            //    Data.VerilogFile vfile = file as Data.VerilogFile;
            //    vfile.RegisterModuleInstance(fileItem);
            //}

            return instance;
        }
        public IVerilogRelatedFile RootFile { get; protected set; }

        private string id;
        public override string ID
        {
            get
            {
                return id;
            }
        }

        public bool ReplaceBy(
            VerilogHeaderInstance file
            //Verilog.ModuleItems.ModuleInstantiation moduleInstantiation,
            //codeEditor.Data.Project project
            )
        {
            //ProjectProperty projectPropery = project.ProjectProperties[Plugin.StaticID] as ProjectProperty;
            //Data.IVerilogRelatedFile file = projectPropery.GetFileOfModule(moduleInstantiation.ModuleName);
            if (file == null) return false;
            if (!IsSameAs(file as File)) return false;
            //if (Project != project) return false;
            //if (ModuleName != moduleInstantiation.ModuleName) return false;

            ParsedDocument = file.ParsedDocument;

            return true;
        }

        public override codeEditor.CodeEditor.CodeDocument CodeDocument
        {
            get
            {
                if (SourceVerilogFile == null) return null;
                return SourceVerilogFile.CodeDocument;
            }
        }
        public override void Dispose()
        {
            disposeItems();
        }

        private void disposeItems()
        {
            if (ParsedDocument != null && ParameterOverrides.Count != 0)
            {
                foreach (var incFile in VerilogParsedDocument.IncludeFiles.Values)
                {
                    incFile.Dispose();
                }
            }
            parsedDocument = null;
            //SourceVerilogFile.RemoveModuleInstance(this);
        }

        public string ModuleName { set; get; }


        public Dictionary<string, Verilog.Expressions.Expression> ParameterOverrides;
        public string ParameterId
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in ParameterOverrides)
                {
                    sb.Append(kvp.Key);
                    sb.Append("=");
                    sb.Append(kvp.Value.Value.ToString());
                    sb.Append(",");
                }
                return sb.ToString();
            }
        }

        private Data.VerilogHeaderFile SourceVerilogFile
        {
            get
            {
                return SourceTextFile as VerilogHeaderFile;
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
                return parsedDocument;
            }
            set
            {
                parsedDocument = value;
            }
        }
        public override void Save()
        {
            if (CodeDocument == null) return;

            SourceTextFile.Save();
        }

        public override DateTime? LoadedFileLastWriteTime
        {
            get
            {
                return SourceTextFile.LoadedFileLastWriteTime;
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
            Verilog.ParsedDocument vParsedDocument = newParsedDocument as Verilog.ParsedDocument;
            parsedDocument = vParsedDocument;

            //{
            //    Data.VerilogFile source = SourceVerilogFile;
            //    if (source == null) return;
            //    source.RegisterInstanceParsedDocument(ParameterId, newParsedDocument, this);
            //}
            ReparseRequested = vParsedDocument.ReparseRequested;
            Update();
        }



        public ProjectProperty ProjectProperty
        {
            get
            {
                return Project.ProjectProperties[Plugin.StaticID] as ProjectProperty;
            }
        }




        public override ajkControls.CodeTextbox.CodeDrawStyle DrawStyle
        {
            get
            {
                return Global.CodeDrawStyle;
            }
        }

        protected override codeEditor.NavigatePanel.NavigatePanelNode createNode()
        {
            NavigatePanel.VerilogHeaderInstanceNode node = new NavigatePanel.VerilogHeaderInstanceNode(this,Project);
            nodeRef = new WeakReference<codeEditor.NavigatePanel.NavigatePanelNode>(node);
            return node;
        }

        public override codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode)
        {
            Data.IVerilogRelatedFile parentFile = Parent as Data.IVerilogRelatedFile;
            if (parentFile == null) return null;
            // do not parse again for background parse. header file is parsed with parent file.
            if (parseMode != DocumentParser.ParseModeEnum.EditParse) return null;

            // Use Parent File Parser for Edit Parse
            return parentFile.CreateDocumentParser(parseMode);
        }


        public override void Update()
        {
            lock (Items)
            {
                if (VerilogParsedDocument == null)
                {
                    // dispose all subnodes
                    foreach (Item item in items.Values) item.Dispose();
                    items.Clear();
                    return;
                }

                List<Item> targetItems = new List<Item>();
                Dictionary<string, Item> newItems = new Dictionary<string, Item>();

                // include file
                Dictionary<string, VerilogHeaderInstance> prevIncludes = new Dictionary<string, VerilogHeaderInstance>();
                foreach (Item item in items.Values)
                {
                    if (item is VerilogHeaderInstance)
                    {
                        VerilogHeaderInstance vfile = item as VerilogHeaderInstance;
                        prevIncludes.Add(vfile.ID, vfile);
                    }
                }

                // include file
                foreach (VerilogHeaderInstance vhFile in VerilogParsedDocument.IncludeFiles.Values)
                {
                    if (prevIncludes.ContainsKey(vhFile.ID))
                    {
//                        prevIncludes[vhFile.ID].ReplaceBy(vhFile);
                        targetItems.Add(prevIncludes[vhFile.ID]);
                    }
                    else
                    {
                        string keyname = vhFile.Name;
                        {
                            int i = 0;
                            while (items.ContainsKey(keyname + "_" + i.ToString()))
                            {
                                i++;
                            }
                            keyname = keyname + "_" + i.ToString();
                        }
                        newItems.Add(keyname, vhFile);
                        targetItems.Add(vhFile);
                        vhFile.Parent = this;
                    }
                }

                // module instances
                foreach (Verilog.Module module in VerilogParsedDocument.Modules.Values)
                {
                    foreach (Verilog.ModuleItems.ModuleInstantiation moduleInstantiation in module.ModuleInstantiations.Values)
                    {
                        if (items.ContainsKey(moduleInstantiation.Name))
                        { // already exist item
                            Item oldItem = items[moduleInstantiation.Name];
                            if (oldItem is Data.VerilogModuleInstance && (oldItem as Data.VerilogModuleInstance).ReplaceBy(moduleInstantiation, Project))
                            { // sucessfully replaced
                                targetItems.Add(oldItem);
                            }
                            else
                            { // re-generate (same module instance name, but different file or module name or parameter
                                Item item = Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                                if (item != null & !newItems.ContainsKey(moduleInstantiation.Name))
                                {
                                    item.Parent = this;
                                    newItems.Add(moduleInstantiation.Name, item);
                                    targetItems.Add(item);
                                    if (moduleInstantiation.ParameterOverrides.Count != 0)
                                    {
                                        Data.VerilogModuleInstance moduleInstance = item as Data.VerilogModuleInstance;
                                        if (moduleInstance.ParsedDocument == null)
                                        { // background reparse if not parsed
                                            Project.AddReparseTarget(item);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        { // new item
                            Item item = Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                            if (item != null & !newItems.ContainsKey(moduleInstantiation.Name))
                            {
                                item.Parent = this;
                                newItems.Add(moduleInstantiation.Name, item);
                                targetItems.Add(item);
                                if (moduleInstantiation.ParameterOverrides.Count != 0)
                                {
                                    Data.VerilogModuleInstance moduleInstance = item as Data.VerilogModuleInstance;

                                    if (moduleInstance.ParsedDocument == null)
                                    {   // background reparse
                                        Project.AddReparseTarget(item);
                                    }
                                }
                            }
                        }
                    }
                }

                { // remove unused items
                    List<Item> removeItems = new List<Item>();
                    foreach (codeEditor.Data.Item item in items.Values)
                    {
                        if (!targetItems.Contains(item)) removeItems.Add(item);
                    }

                    foreach (Item item in removeItems)
                    {
                        items.Remove(item.Name);
                    }
                }

                items.Clear();
                foreach (Item item in targetItems)
                {
                    items.Add(item.Name, item);
                }
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

        public override List<codeEditor.CodeEditor.PopupItem> GetPopupItems(ulong version, int index)
        {
            if (VerilogParsedDocument == null) return null;
            if (VerilogParsedDocument.Version != version) return null;

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
            toolItems.Add(new Verilog.Snippets.ModuleInstanceSnippet());
            return toolItems;
        }

        public override List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index, out string cantidateWord)
        {
            cantidateWord = null;

            if (VerilogParsedDocument == null) return null;
            int line = CodeDocument.GetLineAt(index);
            int lineStartIndex = CodeDocument.GetLineStartIndex(line);
            bool endWithDot;
            List<string> words = ((pluginVerilog.CodeEditor.CodeDocument)CodeDocument).GetHierWords(index, out endWithDot);
            if (endWithDot)
            {
                cantidateWord = "";
            }
            else
            {
                cantidateWord = words.LastOrDefault();
                if (words.Count > 0)
                {
                    words.RemoveAt(words.Count - 1);
                }
            }
            if (cantidateWord == null) cantidateWord = "";

            List<codeEditor.CodeEditor.AutocompleteItem> items = VerilogParsedDocument.GetAutoCompleteItems(words, lineStartIndex, line, (CodeEditor.CodeDocument)CodeDocument, cantidateWord);

            return items;
        }



        private void applyAutoInput()
        {
            int index = CodeDocument.CaretIndex;
            int line = CodeDocument.GetLineAt(index);
            if (line == 0) return;

            int lineHeadIndex = CodeDocument.GetLineStartIndex(line);

            int prevTabs = 0;
            if (line != 1)
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

            if (prevTabs != 0) CodeDocument.Replace(lineHeadIndex, indentLength, 0, new String('\t', prevTabs));
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
