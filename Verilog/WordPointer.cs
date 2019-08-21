using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordPointer
    {
        public WordPointer(codeEditor.CodeEditor.CodeDocument document, codeEditor.CodeEditor.ParsedDocument parsedDocument)
        {
            this.Document = document;
            this.ParsedDocument = parsedDocument;
            FetchNext(this.Document,ref index, out length, out nextIndex, out wordType);
        }

        public void Dispose()
        {
            this.Document = null;
            this.ParsedDocument = null;
        }

        public codeEditor.CodeEditor.CodeDocument Document { get; protected set; }
        public codeEditor.CodeEditor.ParsedDocument ParsedDocument { get; protected set; }

        protected int index = 0;
        protected int length = 0;
        protected int nextIndex;
        protected bool commentSkipped = false;
        protected int commentIndex = -1;

        protected WordTypeEnum wordType = WordTypeEnum.Eof;

        public enum WordTypeEnum
        {
            Number,
            Symbol,
            Text,
            Comment,
            String,
            CompilerDirective,
            Eof
        }

        public WordPointer Clone()
        {
            WordPointer ret = new WordPointer(Document, ParsedDocument);
            ret.index = index;
            ret.length = length;
            ret.nextIndex = nextIndex;
            ret.wordType = wordType;
            return ret;
        }

        public void Color(CodeDrawStyle.ColorType colorType)
        {
            for (int i = index; i < index + length; i++)
            {
                Document.SetColorAt(i, CodeDrawStyle.ColorIndex(colorType));
            }
        }

        public void AppendBlock(int startIndex,int lastIndex)
        {
            Document.AppendBlock(startIndex, lastIndex);
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public void AddError(string message)
        {
            addError(index, length, message);
            if (ParsedDocument == null) return;
        }

        public void AddError(WordReference fromReference, string message)
        {
            addError(fromReference.Index, fromReference.Length, message);
        }

        private void addError(int index,int length, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).ErrorCount < 100)
            {
                int lineNo = Document.GetLineAt(index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Error, index, lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).ErrorCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(">100 errors", Verilog.ParsedDocument.Message.MessageType.Error, 0, 0, 0, ParsedDocument.ItemID, ParsedDocument.Project));
            }

            {
                for (int i = index; i < index + length; i++)
                {
                    Document.SetMarkAt(i, 0);
                }
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).ErrorCount++;
        }

        public void AddWarning(string message)
        {
            addWarning(index, length, message);
        }

        public void AddWarning(WordReference fromReference, string message)
        {
            addWarning(fromReference.Index, fromReference.Length, message);
        }
        private void addWarning(int index, int length, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount < 100)
            {
                int lineNo = Document.GetLineAt(index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Warning, index, lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(">100 warnings", Verilog.ParsedDocument.Message.MessageType.Warning, 0, 0, 0, ParsedDocument.ItemID, ParsedDocument.Project));
            }

            for (int i = index; i < index + length; i++)
            {
                Document.SetMarkAt(i, 1);
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).WarningCount++;
        }

        public void AddNotice(string message)
        {
            addNotice(index, length, message);
        }

        public void AddNotice(WordReference fromReference, string message)
        {
            addNotice(fromReference.Index, fromReference.Length, message);
        }

        private void addNotice(int index, int length, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount < 100)
            {
                int lineNo = Document.GetLineAt(index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Notice, index, lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(">100 notices", Verilog.ParsedDocument.Message.MessageType.Notice, 0, 0, 0, ParsedDocument.ItemID, ParsedDocument.Project));
            }

            for (int i = index; i < index + length; i++)
            {
                Document.SetMarkAt(i, 2);
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).WarningCount++;
        }

        public void AddHint(string message)
        {
            addHint(index, length, message);
        }

        public void AddHint(WordReference fromReference, string message)
        {
            addHint(fromReference.Index, fromReference.Length, message);
        }

        private void addHint(int index, int length, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount < 100)
            {
                int lineNo = Document.GetLineAt(index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Hint, index, lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(">100 notices", Verilog.ParsedDocument.Message.MessageType.Hint, 0, 0, 0, ParsedDocument.ItemID, ParsedDocument.Project));
            }

            for (int i = index; i < index + length; i++)
            {
                Document.SetMarkAt(i, 3);
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).WarningCount++;
        }



        public void MoveNext()
        {
            index = nextIndex;
            if (Eof) return;

            FetchNext(Document, ref index, out length, out nextIndex, out wordType);
            commentSkipped = false;
            commentIndex = index;

            while (!Eof && wordType == WordTypeEnum.Comment)
            {
                commentSkipped = true;
                index = nextIndex;
                FetchNext(Document, ref index, out length, out nextIndex, out wordType);
            }
            if (!commentSkipped) commentIndex = -1;
        }

        public string GetFollowedComment()
        {
            if (!commentSkipped) return "";
            return Document.CreateString(commentIndex, index-commentIndex);
        }

        public void MoveNextUntilEol()
        {
            index = nextIndex;
            FetchNextUntilEol(Document, ref index, out length, out nextIndex, out wordType);
            commentSkipped = false;

            while (!Eof && wordType == WordTypeEnum.Comment)
            {
                commentSkipped = true;
                index = nextIndex;
                FetchNext(Document, ref index, out length, out nextIndex, out wordType);
            }
        }

        public bool Eof
        {
            get
            {
                if (nextIndex == Document.Length)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string Text
        {
            get
            {
                return Document.CreateString(index, length);
            }
        }

        public WordTypeEnum WordType
        {
            get
            {
                return wordType;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public char GetCharAt(int wordIndex)
        {
            return Document.GetCharAt(index + wordIndex);
        }

        public static void FetchNext(ajkControls.Document document,ref int index, out int length, out int nextIndex, out WordTypeEnum wordType)
        {
            // skip blanks before word

            int docLength = document.Length;
            char ch;

            while(docLength > index)
            {
                ch = document.GetCharAt(index);
                if ((ch & 0xff80) == 0 && charClass[ch] == 0) { index++; continue; }
                break;
            }

            length = 0;
            nextIndex = index;
            ch = document.GetCharAt(nextIndex);
            if (ch < 128)
            {
                switch (charClass[ch])
                {
                    case 1: // number
                    case 7: // quote
                        wordType = WordTypeEnum.Number;
                        fetchNextAtNumber(document, ref nextIndex, ref wordType);
                        break;
                    case 2: // alphabet
                        wordType = WordTypeEnum.Text;
                        nextIndex++;
                        while (docLength > nextIndex)
                        {
                            ch = document.GetCharAt(nextIndex);
                            if ((ch & 0xff80) == 0 && charClass1246[ch]) { nextIndex++; continue; }
                            break;
                        }
                        break;
                    case 6: // backquote (compiler directive)
                        wordType = WordTypeEnum.CompilerDirective;
                        nextIndex++;
                        while (docLength > nextIndex)
                        {
                            ch = document.GetCharAt(nextIndex);
                            if ((ch & 0xff80) == 0 && charClass1246[ch]) { nextIndex++; continue; }
                            break;
                        }
                        break;
                    case 3: // operators
                        wordType = WordTypeEnum.Symbol;
                        fetchNextAtOperator(document, ref nextIndex, ref wordType);
                        break;
                    case 4: // double quote
                        wordType = WordTypeEnum.String;
                        nextIndex++;
                        while (
                            docLength > nextIndex
                        )
                        {
                            if(document.GetCharAt(nextIndex) == '\"')
                            {
                                if(document.Length > nextIndex) nextIndex++;
                                break;
                            }
                            nextIndex++;
                        }
                        
                        break;
                    case 5: // blackslash
                        wordType = WordTypeEnum.Text;
                        nextIndex++;
                        while (docLength > nextIndex)
                        {
                            ch = document.GetCharAt(nextIndex);
                            if (ch == '\n') break;
                            if (ch == '\r') break;
                            if (ch == ' ') break;
                            if (ch == '\t') break;
                            nextIndex++;
                        }
                        break;
                    default:
                        wordType = WordTypeEnum.Text;
                        break;
                }
            }
            else
            {
                wordType = WordTypeEnum.Text;
                while (document.Length > nextIndex && document.GetCharAt(nextIndex) >= 128)
                {
                    nextIndex++;
                }
            }
            length = nextIndex - index;

        }

        public static void FetchNextUntilEol(ajkControls.Document document, ref int index, out int length, out int nextIndex, out WordTypeEnum wordType)
        {
            int docLength = document.Length;
            char ch;
            // skip blanks before word

            while (docLength > index)
            {
                ch = document.GetCharAt(index);
                if (ch == '\t') { index++; continue; }
                if (ch == ' ') { index++; continue; }
                break;
            }

            if (index == document.Length - 1)
            {
                length = 0;
                nextIndex = index;
                wordType = WordTypeEnum.Eof;
                return;
            }

            length = 0;
            nextIndex = index;

            wordType = WordTypeEnum.Text;
            nextIndex++;

            while (docLength > nextIndex)
            {
                ch = document.GetCharAt(nextIndex);
                if (ch == '\r') break;
                if (ch == '\n') break;
                nextIndex++;
            }

            length = nextIndex - index;
        }

        private static void fetchNextAtNumber(ajkControls.Document document, ref int nextIndex, ref WordTypeEnum wordType)
        {
            int docLength = document.Length;
            char ch;

            while (docLength > nextIndex)
            {
                ch = document.GetCharAt(nextIndex);
                if ((ch & 0xff80) != 0) break;
                if (charClass[ch] == 1) { nextIndex++; continue; }
                if (ch == '_') { nextIndex++; continue; }
                break;
            }

            if (docLength <= nextIndex) return;

            ch = document.GetCharAt(nextIndex);
            if (ch == '.' | ch == 'e' || ch == 'E')
            { // real
                if (ch == '.')
                {
                    nextIndex++;
                    if (document.Length <= nextIndex) return;

                    while (docLength > nextIndex)
                    {
                        ch = document.GetCharAt(nextIndex);
                        if ((ch & 0xff80) != 0) break;
                        if (charClass[ch] == 1) { nextIndex++; continue; }
                        if (ch == '_') { nextIndex++; continue; }
                        break;
                    }
                }

                ch = document.GetCharAt(nextIndex);
                if (ch == 'e' || ch == 'E')
                {
                    nextIndex++;
                    if (document.Length <= nextIndex) return;

                    ch = document.GetCharAt(nextIndex);
                    if (ch == '+' || ch == '-')
                    {
                        nextIndex++;
                        if (document.Length <= nextIndex) return;
                    }
                    while (docLength > nextIndex)
                    {
                        ch = document.GetCharAt(nextIndex);
                        if ((ch & 0xff80) != 0) break;
                        if (charClass[ch] == 1) { nextIndex++; continue; }
                        if (ch == '_') { nextIndex++; continue; }
                        break;
                    }
                }
            }
            else if (ch == '\'')
            {
                nextIndex++;
                if (docLength <= nextIndex) return;

                ch = document.GetCharAt(nextIndex);
                while (docLength > nextIndex)
                {
                    ch = document.GetCharAt(nextIndex);
                    if ((ch & 0xff80) != 0) break;
                    if (charClass[ch] == 1) { nextIndex++; continue; } // 0-9
                    if (charClass[ch] == 2) { nextIndex++; continue; } // alphabet
                    if (ch == '?') { nextIndex++; continue; }
                    if (ch == 'x') { nextIndex++; continue; }
                    if (ch == 'X') { nextIndex++; continue; }
                    if (ch == 'z') { nextIndex++; continue; }
                    if (ch == 'Z') { nextIndex++; continue; }
                    break;
                }
            }
            else
            {
                while (docLength > nextIndex)
                {
                    ch = document.GetCharAt(nextIndex);
                    if ((ch & 0xff80) != 0) break;
                    if (charClass[ch] == 1) { nextIndex++; continue; } // 0-9
                    if (charClass[ch] == 2) { nextIndex++; continue; } // alphabet
                    if (ch == '?') { nextIndex++; continue; }
                    if (ch == 'x') { nextIndex++; continue; }
                    if (ch == 'X') { nextIndex++; continue; }
                    if (ch == 'z') { nextIndex++; continue; }
                    if (ch == 'Z') { nextIndex++; continue; }
                    break;
                }
            }
        }

        private static void fetchNextAtOperator(ajkControls.Document document, ref int nextIndex, ref WordTypeEnum wordType)
        {
            int docLength = document.Length;

            if (docLength > nextIndex + 1 && document.GetCharAt(nextIndex) == '/' && document.GetCharAt(nextIndex + 1) == '*') // "/*"
            { // comment block
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                while (docLength > nextIndex)
                {
                    document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                    if (document.GetCharAt(nextIndex) == '/' && document.GetCharAt(nextIndex - 1) == '*')
                    {
                        nextIndex++;
                        break;
                    }
                    nextIndex++;
                }
                wordType = WordTypeEnum.Comment;
                return;
            }

            if (docLength > nextIndex + 1 && document.GetCharAt(nextIndex) == '/' && document.GetCharAt(nextIndex + 1) == '/') // "//"
            { // line comment
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                while (docLength > nextIndex && document.GetCharAt(nextIndex) != '\n')
                {
                    document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                    nextIndex++;
                }
                if (docLength > nextIndex && document.GetCharAt(nextIndex) == '\n')
                {
                    document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                    nextIndex++;
                }
                wordType = WordTypeEnum.Comment;
                return;
            }

            string op3 = "";
            if (docLength > nextIndex + 2)
            {
                op3 = document.CreateString(nextIndex, 3);
            }

            string op2 = "";
            if (docLength > nextIndex + 1)
            {
                op2 = document.CreateString(nextIndex, 2);
            }


            if (
                    op3 == ">>>" ||
                    op3 == "<<<" ||
                    op3 == "===" ||
                    op3 == "!=="
            )
            {
                nextIndex = nextIndex + 3;
                return;
            }

            if (
                op2 == "==" ||
                op2 == "!=" ||
                op2 == "&&" ||
                op2 == "||" ||
                op2 == ">>" ||
                op2 == "<<" ||
                op2 == "<=" ||
                op2 == ">=" ||
                op2 == "~|" ||
                op2 == "~&" ||
                op2 == "~^" ||
                op2 == "^~" ||
                op2 == "**" ||
                op2 == "(*" ||
                op2 == "*)" ||
                op2 == "+:" ||
                op2 == "-:" ||
                op2 == "=>"
            )
            {
                nextIndex = nextIndex + 2;
                return;
            }

            nextIndex++;
        }

        private static byte[] charClass = new byte[128]
        {
            //      0,1,2,3,4,5,6,7,8,9,a,b,c,e,d,f
            // 0*
                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            // 1*
                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            // 2*     ! " # $ % & ' ( ) * + , - . /
                    0,3,4,3,2,3,3,7,3,3,3,3,3,3,3,3,
            // 3*   0 1 2 3 4 5 6 7 8 9 : ; < = > ?
                    1,1,1,1,1,1,1,1,1,1,3,3,3,3,3,3,
            // 4*   @ A B C D E F G H I J K L M N O
                    3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
            // 5*   P Q R S T U V W X Y Z [ \ ] ^ _
                    2,2,2,2,2,2,2,2,2,2,2,3,5,3,3,2,
            // 6*   ` a b c d e f g h i j k l m n o
                    6,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
            // 7*   p q r s t u v w x y z { | } ~ 
                    2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,0
        };

        private static bool[] charClass1246 = new bool[128]
        {
            //      0,1,2,3,4,5,6,7,8,9,a,b,c,e,d,f
            // 0*
                    false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            // 1*
                    false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            // 2*     ! " # $ % & ' ( ) * + , - . /
                    false,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,
            // 3*   0 1 2 3 4 5 6 7 8 9 : ; < = > ?
                    true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,false,
            // 4*   @ A B C D E F G H I J K L M N O
                    false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            // 5*   P Q R S T U V W X Y Z [ \ ] ^ _
                    true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,
            // 6*   ` a b c d e f g h i j k l m n o
                    true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            // 7*   p q r s t u v w x y z { | } ~ 
                    true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false
        };

    }
}

