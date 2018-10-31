using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordScanner : IDisposable
    {
        public WordScanner( codeEditor.CodeEditor.CodeDocument document, Verilog.ParsedDocument parsedDocument)
        {
            RootParsedDocument = parsedDocument;
            wordPointer = new WordPointer(document, parsedDocument);
        }

        public void Dispose()
        {
            wordPointer.Dispose();
        }

        public Verilog.ParsedDocument RootParsedDocument { get; protected set; }

        protected IWordPointer wordPointer = null;
        protected List<IWordPointer> stock = new List<IWordPointer>();

        public WordScanner Clone()
        {
            WordScanner ret = new WordScanner(wordPointer.Document, wordPointer.ParsedDocument);
            ret.wordPointer = wordPointer.Clone();
            foreach (var wp in stock)
            {
                ret.stock.Add(wp.Clone());
            }
            return ret;
        }

        public void Color(byte colorIndex)
        {
            wordPointer.Color(colorIndex);
        }

        public void AddError(string message)
        {
            wordPointer.AddError(message);
        }

        public void AddWarning(string message)
        {
            wordPointer.AddWarning(message);
        }

        public void MoveNext()
        {
            while(wordPointer.Eof && stock.Count != 0)
            {
                wordPointer.Dispose();
                wordPointer = stock.Last();
                stock.Remove(stock.Last());
            }

            wordPointer.MoveNext();
            recheckWord();
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
                return wordPointer.Text;
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
                IWordPointer temp = wordPointer.Clone();
                wordPointer.MoveNext();
                while (!wordPointer.Eof && wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                string text = wordPointer.Text;
                wordPointer = temp;
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
                case "`default_nettype":
                case "`endcelldefine":
                    break;
                case "`endif":
                    wordPointer.Color((byte)Style.Color.Keyword);
                    wordPointer.MoveNext();
                    if (ifDefs.Count != 0) ifDefs.Remove(ifDefs.Last());
                    break;
                case "`ifdef":
                    wordPointer.Color((byte)Style.Color.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color((byte)Style.Color.Identifier);
                    if (RootParsedDocument.Macros.ContainsKey(wordPointer.Text))
                    {
                        wordPointer.MoveNext();
                        ifDefs.Add(ifdefEnum.ifdefActive);
                    }
                    else
                    {
                        wordPointer.MoveNext();
                        while (!wordPointer.Eof)
                        {
                            if(wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                            {
                                if(wordPointer.Text == "`else")
                                {
                                    wordPointer.Color((byte)Style.Color.Keyword);
                                    wordPointer.MoveNext();
                                    ifDefs.Add(ifdefEnum.ElseActive);
                                    break;
                                }else if(wordPointer.Text == "`endif"){
                                    wordPointer.Color((byte)Style.Color.Keyword);
                                    wordPointer.MoveNext();
                                    break;
                                }else if(wordPointer.Text == "`ifdef")
                                {
                                    break;
                                }
                            }
                            wordPointer.MoveNext();
                        }
                    }
                    break;
                case "`else":
                    wordPointer.Color((byte)Style.Color.Keyword);
                    wordPointer.MoveNext();
                    while (!wordPointer.Eof)
                    {
                        if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                        {
                            if (wordPointer.Text == "`else")
                            {
                                wordPointer.Color((byte)Style.Color.Keyword);
                                wordPointer.MoveNext();
                                ifDefs.Add(ifdefEnum.ElseActive);
                                break;
                            }
                            else if (wordPointer.Text == "`endif")
                            {
                                wordPointer.Color((byte)Style.Color.Keyword);
                                wordPointer.MoveNext();
                                break;
                            }
                        }
                        wordPointer.MoveNext();
                    }
                    break;
                case "`elsif":
                case "`ifndef":
                case "`line":
                case "`nounconnected_drive":
                case "`resetall":
                case "`timescale":
                case "`unconnected_drive":
                case "`undef":
                    wordPointer.Color((byte)Style.Color.Keyword);
                    wordPointer.AddError("unsupported compiler directive");
                    wordPointer.MoveNext();
                    break;
                default: // macro call
                    parseMacro();
                    break;
            }
        }

        private void parseDefine()
        {
            wordPointer.Color((byte)Style.Color.Keyword);
            wordPointer.MoveNext();
            if (!General.IsIdentifier(wordPointer.Text))
            {
                wordPointer.AddError("iilegal identifier");
                return;
            }
            bool error = false;

            wordPointer.Color((byte)Style.Color.Identifier);
            string identifier = wordPointer.Text;
            if (RootParsedDocument.Macros.ContainsKey(identifier))
            {
                wordPointer.AddError("dulplicated identifier");
                error = true;
            }

            wordPointer.MoveNextUntilEol();
            string macroText = wordPointer.Text;

            if (!error)
            {
                RootParsedDocument.Macros.Add(identifier, macroText);
            }

            wordPointer.MoveNext();
            recheckWord();
        }

        private void parseInclude()
        {
            wordPointer.Color((byte)Style.Color.Keyword);
            wordPointer.MoveNext();
            if(wordPointer.WordType != WordPointer.WordTypeEnum.String)
            {
                wordPointer.AddError("\" expected");
                return;
            }
            string filePath = wordPointer.Text;
            filePath = filePath.Substring(1, filePath.Length - 2);

            // search in same folder with original verilog file
            string fileID = wordPointer.ParsedDocument.ItemID;
            codeEditor.Data.File file = wordPointer.ParsedDocument.Project.GetRegisterdItem(fileID) as codeEditor.Data.File;

            if(file == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            string sameFolderPath = file.RelativePath;
            if (sameFolderPath.Contains('\\'))
            {
                sameFolderPath = sameFolderPath.Substring(0, sameFolderPath.LastIndexOf('\\'));
                sameFolderPath = sameFolderPath +'\\'+ filePath;

                string id = codeEditor.Data.File.GetID(sameFolderPath, wordPointer.ParsedDocument.Project);
                if (wordPointer.ParsedDocument.Project.IsRegistered(id)){
                    diveIntoIncludeFile(sameFolderPath);
                    return;
                }
            }

            // search same filename in full project
            {
                List<string> ids = wordPointer.ParsedDocument.Project.GetRegisteredIdList();
                string id = ids.FirstOrDefault((x) =>
                {
                    return x.EndsWith(filePath);
                });

                if (id == null)
                {
                    wordPointer.AddError("file not found");
                    wordPointer.MoveNext();
                    return;
                }

                codeEditor.Data.Item item = wordPointer.ParsedDocument.Project.GetRegisterdItem(id);
                codeEditor.Data.File ffile = item as codeEditor.Data.File;
                if (ffile != null)
                {
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
            wordPointer.Color((byte)Style.Color.Identifier);
            string macroIdentifier = wordPointer.Text.Substring(1);

            if (!RootParsedDocument.Macros.ContainsKey(macroIdentifier))
            {
                wordPointer.AddError("unsupported macro call");
                wordPointer.MoveNext();
                return;
            }

            string macroText = RootParsedDocument.Macros[macroIdentifier];
//            wordPointer.MoveNext();

            codeEditor.CodeEditor.CodeDocument codeDocument = new codeEditor.CodeEditor.CodeDocument();
            codeDocument.Replace(0, 0, 0, macroText);

            IWordPointer newPointer = new WordPointer(codeDocument, wordPointer.ParsedDocument);
            stock.Add(wordPointer);
            wordPointer = newPointer;
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
            return;
        }

        private void diveIntoIncludeFile(string relativeFilePath)
        {
            string id = wordPointer.ParsedDocument.ItemID+":include:"+relativeFilePath;
            codeEditor.Data.Item item;
            if (wordPointer.ParsedDocument.Project.IsRegistered(id))
            {
                item = wordPointer.ParsedDocument.Project.GetRegisterdItem(id);
            }
            else
            {
                item = Data.VerilogHeaderFile.CreateInstance(relativeFilePath, id, wordPointer.ParsedDocument.Project);
            }
            codeEditor.Data.ITextFile textFile = item as codeEditor.Data.ITextFile;
            if(textFile == null ||! (textFile is Data.VerilogHeaderFile))
            {
                wordPointer.AddError("illegal filetype");
                return;
            }
            Data.VerilogHeaderFile vhFile = textFile as Data.VerilogHeaderFile;


            if(!wordPointer.ParsedDocument.IncludeFiles.ContainsKey(vhFile.ID))
            {
                wordPointer.ParsedDocument.IncludeFiles.Add(vhFile.ID,vhFile);
            }

            IWordPointer newPointer = new WordPointer(vhFile.CodeDocument, wordPointer.ParsedDocument);
            stock.Add(wordPointer);
            wordPointer = newPointer;
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