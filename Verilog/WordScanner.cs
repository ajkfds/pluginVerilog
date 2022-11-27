using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordScanner : IDisposable
    {
        public WordScanner( CodeEditor.CodeDocument document, Verilog.ParsedDocument parsedDocument,bool systemVerilog)
        {
            RootParsedDocument = parsedDocument;
            wordPointer = new WordPointer(document, parsedDocument);
            this.systemVerilog = systemVerilog;
        }

        public void GetFirst()
        {
            recheckWord();
        }

        public void Dispose()
        {
            wordPointer.Dispose();
        }

        public Verilog.ParsedDocument RootParsedDocument { get; protected set; }

        public CodeEditor.CodeDocument Document
        {
            get
            {
                return wordPointer.Document;
            }
        }

        public codeEditor.Data.Project Project
        {
            get
            {
                return RootParsedDocument.Project;
            }
        }

        public ProjectProperty ProjectProperty
        {
            get
            {
                if (Project == null) return null;
                return Project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            }
        }
        public WordPointer RootPointer
        {
            get
            {
                if(stock.Count == 0)
                {
                    return wordPointer;
                }
                else
                {
                    return stock[0];
                }
            }
        }

        private int nonGeneratedCount = 0;
        private bool prototype = false;

        public bool Prototype
        {
            set
            {
                prototype = value;
            }
            get
            {
                return prototype;
            }
        }

        private bool cellDefine;
        public bool CellDefine
        {
            get
            {
                return cellDefine;
            }
        }

        private bool systemVerilog;
        public bool SystemVerilog
        {
            get { return systemVerilog; }
        }

        public void StartNonGenenerated()
        {
            nonGeneratedCount++;
        }

        public void EndNonGeneratred()
        {
            if (nonGeneratedCount == 0) return;
            nonGeneratedCount--;
        }

        public string Section { get; protected set; }

        protected WordPointer wordPointer = null;
        protected List<WordPointer> stock = new List<WordPointer>();

        public WordScanner Clone()
        {
            WordScanner ret = new WordScanner(wordPointer.Document, RootParsedDocument,systemVerilog);
            ret.wordPointer = wordPointer.Clone();
            ret.nonGeneratedCount = nonGeneratedCount;
            ret.prototype = prototype;
            foreach (var wp in stock)
            {
                ret.stock.Add(wp.Clone());
            }
            return ret;
        }
        public CommentScanner GetCommentScanner()
        {
            return wordPointer.GetCommentScanner();
        }
            public WordReference GetReference()
        {
            if (stock.Count == 0)
            {
                return new WordReference(wordPointer.Index, wordPointer.Length, wordPointer.ParsedDocument,Document);
            }
            else
            {
                return new WordReference(stock[0].Index, stock[0].Length, wordPointer.ParsedDocument,Document);
            }
            //            return new WordReference(wordPointer.Document, wordPointer.ParsedDocument, wordPointer.Index, wordPointer.Length);
        }

        public WordReference GetReference(WordReference fromReference)
        {
            return new WordReference(fromReference.Index, wordPointer.Index + wordPointer.Length - fromReference.Index, wordPointer.ParsedDocument,Document);
        }

        public void Color(CodeDrawStyle.ColorType colorType)
        {
            if (prototype) return;
//            if (nonGeneratedCount != 0 || prototype) return;
            wordPointer.Color(colorType);
        }

        public void AppendBlock(int startIndex, int lastIndex)
        {
            if (stock.Count != 0) return;
            if (wordPointer.Document.GetLineAt(startIndex) == wordPointer.Document.GetLineAt(lastIndex)) return;
            wordPointer.AppendBlock(startIndex, lastIndex);
        }
        public void AddError(string message)
        {
            if (prototype) return;
            wordPointer.AddError(message);
        }

        public void AddWarning(string message)
        {
            if (prototype) return;
            wordPointer.AddWarning(message);
        }

        public void AddPrototypeError(string message)
        {
            if (prototype) return;
            wordPointer.AddError(message);
        }

        public void AddPrototypeWarning(string message)
        {
            if (prototype) return;
            wordPointer.AddWarning(message);
        }

        public void AddNotice(WordReference reference, string message)
        {
            if (prototype) return;
            wordPointer.AddNotice(reference, message);
        }
        public void AddHint(WordReference reference, string message)
        {
            if (prototype) return;
            wordPointer.AddHint(reference, message);
        }

        public int RootIndex
        {
            get
            {
                if(stock.Count == 0)
                {
                    return wordPointer.Index;
                }
                else
                {
                    return stock[0].Index;
                }
            }
        }

        public bool Active
        {
            get
            {
                if (nonGeneratedCount != 0) return false;
                return true;
            }
        }

        public void SkipToKeyword(string stopWord)
        {
            while (!Eof)
            {
                if (Text == stopWord) return;
                if (General.ListOfStatementStopKeywords.Contains(Text)) return;
                MoveNext();
            }
        }

        public string SectionName
        {
            get
            {
                return wordPointer.SectionName;
            }
        }

        public void MoveNext()
        {
            if (nonGeneratedCount != 0)
            {
                wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
            }

            if (wordPointer.Eof)
            {
                while (wordPointer.Eof && stock.Count != 0)
                {
                    bool error = false;
                    if (wordPointer.ParsedDocument.Messages.Count != 0) error = true;

                    if (wordPointer.ParsedDocument == stock.Last().ParsedDocument)
                    {
                        error = false;
                    }

                    //wordPointer.Dispose(); keep document & parsedData
                    wordPointer = stock.Last();
                    stock.Remove(stock.Last());
                    if (error) wordPointer.AddError("include errors");
                    wordPointer.MoveNext();
                }
                recheckWord();
            }
            else
            {
                wordPointer.MoveNext();
                recheckWord();
            }
        }

        public string GetFollowedComment()
        {
            return wordPointer.GetFollowedComment();
        }

        private void recheckWord()
        {
            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                else if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    parseCompilerDirctive();
                }
                else
                {
                    break;
                }
            }
        }

        public bool Eof
        {
            get
            {
                if(stock.Count == 0) return wordPointer.Eof;
                for(int i= stock.Count - 1; i >= 0; i--)
                {
                    if (!stock[i].Eof) return false;
                }
                return true;
            }
        }

        public string Text
        {
            get
            {
                string ret = wordPointer.Text;
                return ret;
            }
        }

        public WordPointer.WordTypeEnum WordType
        {
            get
            {
                return wordPointer.WordType;
            }
        }

        public string NextText
        {
            get
            {
                WordPointer _wp = wordPointer.Clone();
                int _nonGeneratedCount = nonGeneratedCount;
                bool _prototype = prototype;
                List<WordPointer> _stock = new List<WordPointer>();
                foreach (var wp in stock)
                {
                    _stock.Add(wp.Clone());
                }

                //                WordPointer temp = wordPointer.Clone();

                if (wordPointer.Eof)
                {
                    while (wordPointer.Eof && stock.Count != 0)
                    {
                        bool error = false;
                        if (wordPointer.ParsedDocument.Messages.Count != 0) error = true;

                        if (wordPointer.ParsedDocument == stock.Last().ParsedDocument)
                        {
                            error = false;
                        }

                        wordPointer.Dispose();
                        wordPointer = stock.Last();
                        stock.Remove(stock.Last());
                        if (error) wordPointer.AddError("include errors");
                        wordPointer.MoveNext();
                    }
                }
                else
                {
                    wordPointer.MoveNext();
                }

                recheckWord();
                string text = wordPointer.Text;

                wordPointer = _wp;
                nonGeneratedCount = _nonGeneratedCount;
                prototype = _prototype;
                if(stock.Count != _stock.Count)
                {
                    stock.Clear();
                    foreach (var wp in _stock)
                    {
                        stock.Add(wp);
                    }
                }
                return text;
            }
        }

        public int Length
        {
            get
            {
                return wordPointer.Length;
            }
        }

        public char GetCharAt(int wordIndex)
        {
            return wordPointer.GetCharAt(wordIndex);
        }

        private enum ifdefEnum
        {
            ifdefActive,
            ifdefInActive,
            ElseActive,
            ElseInActive
        }
        private List<ifdefEnum> ifDefs = new List<ifdefEnum>();

        private void parseCompilerDirctive()
        {
            switch (wordPointer.Text)
            {
                case "`include":
                    parseInclude();
                    break;
                case "`define":
                    parseDefine();
                    break;
                case "`celldefine":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    cellDefine = true;
                    wordPointer.MoveNext();
                    break;
                case "`resetall":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    break;
                case "`endcelldefine":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    cellDefine = false;
                    wordPointer.MoveNext();
                    break;
                case "`default_nettype":
                    wordPointer.AddError("not supported");
                    wordPointer.MoveNext();
                    break;
                case "`endif":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    if (ifDefs.Count != 0) ifDefs.Remove(ifDefs.Last());
                    break;
                case "`ifdef":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
                    if (
                        RootParsedDocument.Macros.ContainsKey(wordPointer.Text) ||
                        RootParsedDocument.ProjectProperty.Macros.ContainsKey(wordPointer.Text)
                        )
                    {   // true
                        wordPointer.MoveNext();
                        if (wordPointer.Text == "`else")
                        {
                            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                            wordPointer.MoveNext();
                        }
                    }
                    else
                    {   // false
                        wordPointer.MoveNext();
                        skip();
                    }
                    break;
                case "`ifndef":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
                    if (
                        !RootParsedDocument.Macros.ContainsKey(wordPointer.Text) &&
                        !RootParsedDocument.ProjectProperty.Macros.ContainsKey(wordPointer.Text)
                        )
                    {   // true
                        wordPointer.MoveNext();
                        if (wordPointer.Text == "`else")
                        {
                            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                            wordPointer.MoveNext();
                        }
                    }
                    else
                    {   // false
                        wordPointer.MoveNext();
                        skip();
                    }
                    break;
                case "`else":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    skip();
                    break;
                case "`elsif":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
                    if (
                        RootParsedDocument.Macros.ContainsKey(wordPointer.Text) ||
                        RootParsedDocument.ProjectProperty.Macros.ContainsKey(wordPointer.Text)
                        )
                    {   // true
                        wordPointer.MoveNext();
                        if (wordPointer.Text == "`else")
                        {
                            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                            wordPointer.MoveNext();
                        }
                    }
                    else
                    {   // false
                        wordPointer.MoveNext();
                        skip();
                    }
                    break;
                case "`line":
                case "`nounconnected_drive":
                case "`unconnected_drive":
                case "`undef":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.AddError("unsupported compiler directive");
                    wordPointer.MoveNext();
                    break;
                case "`timescale":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNextUntilEol();
                    break;
                default: // macro call
                    parseMacro();
                    break;
            }
        }

        private void skip()
        {
            int depth = 0;
            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    switch (wordPointer.Text)
                    {
                        case "`ifdef":
                            depth++;
                            wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                            wordPointer.MoveNext();
                            break;
                        case "`ifndef":
                            depth++;
                            wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                            wordPointer.MoveNext();
                            break;
                        case "`else":
                            if (depth == 0)
                            {
                                wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                                wordPointer.MoveNext();
                                return;
                            }
                            else
                            {
                                wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                                wordPointer.MoveNext();
                            }
                            break;
                        case "`endif":
                            if (depth == 0)
                            {
                                wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                                wordPointer.MoveNext();
                                return;
                            }
                            else
                            {
                                depth--;
                                wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                                wordPointer.MoveNext();
                            }
                            break;
                        default:
                            wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                            wordPointer.MoveNext();
                            break;
                    }
                }
                else
                {
                    wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                    wordPointer.MoveNext();
                }
            }
        }

        private void parseDefine()
        {
            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
            wordPointer.MoveNextUntilEol();

            WordReference wordRef = GetReference();
            string macroText = wordPointer.Text;

            //wordPointer.MoveNext();
            //if (!General.IsIdentifier(wordPointer.Text))
            //{
            //    wordPointer.AddError("iilegal identifier");
            //    return;
            //}
//            bool error = false;

            string identifier = "";

            // get identifier separator
            int separatorIndex;
            {
                int spaceIndex = int.MaxValue;
                if (macroText.Contains(" ")) spaceIndex = macroText.IndexOf(" ");

                int tabIndex = int.MaxValue;
                if (macroText.Contains("\t")) tabIndex = macroText.IndexOf("\t");

                separatorIndex = spaceIndex;
                if (tabIndex < separatorIndex) separatorIndex = tabIndex; 
            }

            if (separatorIndex == int.MaxValue)
            { // identifier only
                identifier = macroText;
                macroText = "";
            }
            else
            {
                identifier = macroText.Substring(0, separatorIndex);
                macroText = macroText.Substring(separatorIndex);
            }

            if (macroText.Contains("//"))
            {
                macroText = macroText.Substring(0, macroText.IndexOf("//"));
            }
            wordPointer.MoveNext();

            while(!Eof && wordPointer.Text == "\\")
            {
                wordPointer.MoveNextUntilEol();
                string text = wordPointer.Text;
                if (text.Contains("//"))
                {
                    text = text.Substring(0, text.IndexOf("//"));
                }
                macroText = macroText + text;
                wordPointer.MoveNext();
            }

            Macro macro = Macro.Create(identifier, macroText);
            if (!General.IsIdentifier(macro.Name))
            {
                wordRef.AddError("illegal macro identifier");
            }
            else if (RootParsedDocument.Macros.ContainsKey(macro.Name))
            {
                wordRef.AddError("duplicate macro name");
            }
            else
            {
                RootParsedDocument.Macros.Add(macro.Name, macro);
            }


//            wordPointer.MoveNext();
            recheckWord();
        }

        private void parseInclude()
        {
            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
            wordPointer.MoveNext();
            if(wordPointer.WordType != WordPointer.WordTypeEnum.String)
            {
                wordPointer.AddError("\" expected");
                return;
            }
            string filePath = wordPointer.Text;
            filePath = filePath.Substring(1, filePath.Length - 2);

            // search in same folder with original verilog file
            Data.IVerilogRelatedFile file = wordPointer.VerilogFile;

            if (file == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            string sameFolderPath = file.RelativePath;
            if (sameFolderPath.Contains('\\'))
            {
                sameFolderPath = sameFolderPath.Substring(0, sameFolderPath.LastIndexOf('\\'));
                sameFolderPath = sameFolderPath + '\\' + filePath;

                List<codeEditor.Data.Item> items = wordPointer.ParsedDocument.Project.FindItems(
                    (item) =>
                    {
                        if(item is codeEditor.Data.TextFile)
                        {
                            if((item as codeEditor.Data.TextFile).RelativePath == sameFolderPath)
                            {
                                return true;
                            }
                        }
                        return false;
                    },
                    (item) =>
                    {
                        return false;
                    }
                );
                if (items.Count > 0)
                {
                    //wordPointer.MoveNext();
                    diveIntoIncludeFile(sameFolderPath);
                    return;
                }
            }

            // search same filename in full project
            {

                codeEditor.Data.File ffile = wordPointer.ParsedDocument.Project.SearchFile(
                    (f)=> {
                    if (f.Name == filePath) return true;
                    return false;
                    });
                  if (ffile != null)
                {
                    //wordPointer.MoveNext();
                    diveIntoIncludeFile(ffile.RelativePath);
                    return;
                }
            }
            wordPointer.AddError("file not found");
            wordPointer.MoveNext();
            recheckWord();
            return;
        }

        private void parseMacro()
        {
            wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
            string macroIdentifier = wordPointer.Text.Substring(1);

            Macro macro;
            if (RootParsedDocument.Macros.ContainsKey(macroIdentifier))
            {
                macro = RootParsedDocument.Macros[macroIdentifier];
            }
            else if(RootParsedDocument.ProjectProperty.Macros.ContainsKey(macroIdentifier))
            {
                macro = RootParsedDocument.ProjectProperty.Macros[macroIdentifier];
            }
            else
            {
                wordPointer.AddError("unsupported macro call");
                wordPointer.MoveNext();
                return;
            }
//            wordPointer.MoveNext();

            string macroText = macro.MacroText;
            if(macro.Aurguments != null)
            {
                wordPointer.MoveNext();

                List<string> wordAssingment = new List<string>();
                if (wordPointer.Text != "(")
                {
                    wordPointer.AddError("missing macro arguments");
                    wordPointer.MoveNext();
                    return;
                }
                wordPointer.MoveNext();

                while (!wordPointer.Eof)
                {
                    StringBuilder sb = new StringBuilder();
                    int bracketCount = 0;
                    while (!wordPointer.Eof)
                    {
                        if(wordPointer.Text == "(")
                        {
                            bracketCount++;
                        } else if(wordPointer.Text == ")")
                        {
                            if(bracketCount == 0)
                            {
                                break;
                            } else
                            {
                                bracketCount--;
                            }
                        }

                        if(wordPointer.Text == "," && bracketCount == 0)
                        {
                            break;
                        }

                        if (sb.Length != 0) sb.Append(" ");
                        sb.Append(wordPointer.Text);
                        wordPointer.MoveNext();
                    }
                    wordAssingment.Add(sb.ToString());
                    if (wordPointer.Text == ")")
                    {
                        wordPointer.MoveNext();
                        break;
                    }
                    if (wordPointer.Text == ",")
                    {
                        wordPointer.MoveNext();
                        continue;
                    }
                    wordPointer.AddError("illegal macro call");
                    break;
                }

                if(macro.Aurguments.Count != wordAssingment.Count)
                {
                    wordPointer.AddError("macro arguments mismatch");
                    return;
                }
                else
                {
                    for(int i = 0; i < macro.Aurguments.Count; i++)
                    {
                        macroText = macroText.Replace(macro.Aurguments[i], "\0" + i.ToString("X4"));
                    }
                    for (int i = 0; i < macro.Aurguments.Count; i++)
                    {
                        macroText = macroText.Replace("\0" + i.ToString("X4"),wordAssingment[i]);
                    }
                }
            }
            if(macroText == "")
            {
                return;
            }

            CodeEditor.CodeDocument codeDocument = new CodeEditor.CodeDocument(wordPointer.Document.VerilogFile);
            codeDocument.Replace(0, 0, 0, macroText);

            WordPointer newPointer = new WordPointer(codeDocument, wordPointer.ParsedDocument);
            stock.Add(wordPointer);
            wordPointer = newPointer;

            while (true)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                    if (wordPointer.Eof) break;
                }
                else if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    parseCompilerDirctive();
                    break;
                }
                else
                {
                    break;
                }
            }
            return;
        }

        private void diveIntoIncludeFile(string relativeFilePath)
        {
            codeEditor.Data.Item item;
            //if (wordPointer.ParsedDocument.Project.IsRegistered(id))
            //{
            //    item = wordPointer.ParsedDocument.Project.GetRegisterdItem(id);
            //    wordPointer.ParsedDocument.Project.RegisterProjectItem(item);
            //}
            //else
            //{
            //            Data.IVerilogRelatedFile file = wordPointer.ParsedDocument.File;

            string id = wordPointer.ParsedDocument.File.ID + ","+RootParsedDocument.IncludeFiles.Count.ToString();

            item = Data.VerilogHeaderFile.CreateInstance(relativeFilePath, wordPointer.ParsedDocument.Project,id);
            item.Parent = RootParsedDocument.File as codeEditor.Data.Item;
            //}
            codeEditor.Data.ITextFile textFile = item as codeEditor.Data.ITextFile;
            if(textFile == null ||! (textFile is Data.VerilogHeaderFile))
            {
                wordPointer.AddError("illegal filetype");
                return;
            }

            Data.VerilogHeaderFile vhFile = textFile as Data.VerilogHeaderFile;
            if(!RootParsedDocument.IncludeFiles.ContainsKey(vhFile.Name))
            {
                RootParsedDocument.IncludeFiles.Add(vhFile.Name,vhFile);
            }
            else
            {
                vhFile = RootParsedDocument.IncludeFiles[vhFile.Name];
            }
            vhFile.ParsedDocument = new Verilog.ParsedDocument(vhFile);// editid =, -1);


            WordPointer newPointer = new WordPointer(vhFile.CodeDocument as CodeEditor.CodeDocument, vhFile.ParsedDocument as Verilog.ParsedDocument);
            stock.Add(wordPointer);

            wordPointer = newPointer;

            if (wordPointer.Eof)
            {
                MoveNext();
                return;
            }

            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                else if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    parseCompilerDirctive();
                    break;
                }
                else
                {
                    break;
                }
            }
        }

    }

}