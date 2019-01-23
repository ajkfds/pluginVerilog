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

        public int Index
        {
            get
            {
                return index;
            }
        }

        public void AddError(string message)
        {
            if (ParsedDocument == null) return;
            int lineNo = Document.GetLineAt(index);

            ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Error, index,lineNo, length, ParsedDocument.ItemID,ParsedDocument.Project));
            for (int i = index; i < index + length; i++)
            {
                Document.SetMarkAt(i, 0);
            }
        }

        public void AddError(WordReference fromReference, string message)
        {
            if (ParsedDocument == null) return;
            int lineNo = Document.GetLineAt(index);

            ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Error, index, lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            if (fromReference.Document == Document)
            {
                for (int i = fromReference.Index; i < index + length; i++)
                {
                    Document.SetMarkAt(i, 0);
                }
            }
            else
            {
                for (int i = index; i < index + length; i++)
                {
                    Document.SetMarkAt(i, 0);
                }
            }
        }

        public void AddWarning(string message)
        {
            if (ParsedDocument == null) return;
            int lineNo = Document.GetLineAt(index);

            ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Warning, index,lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            for (int i = index; i < index + length; i++)
            {
                Document.SetMarkAt(i, 1);
            }
        }

        public void AddWarning(WordReference fromReference, string message)
        {
            if (ParsedDocument == null) return;
            int lineNo = Document.GetLineAt(index);

            ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Warning, index, lineNo, length, ParsedDocument.ItemID, ParsedDocument.Project));
            if (fromReference.Document == Document)
            {
                for (int i = fromReference.Index; i < index + length; i++)
                {
                    Document.SetMarkAt(i, 1);
                }
            }
            else
            {
                for (int i = index; i < index + length; i++)
                {
                    Document.SetMarkAt(i, 1);
                }
            }
        }

        public void MoveNext()
        {
            index = nextIndex;
            FetchNext(Document, ref index, out length, out nextIndex, out wordType);

            while (!Eof && wordType == WordTypeEnum.Comment)
            {
                index = nextIndex;
                FetchNext(Document, ref index, out length, out nextIndex, out wordType);
            }
        }

        public void MoveNextUntilEol()
        {
            index = nextIndex;
            FetchNextUntilEol(Document, ref index, out length, out nextIndex, out wordType);

            while (!Eof && wordType == WordTypeEnum.Comment)
            {
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
            while (document.Length > index && document.GetCharAt(index) < 128 && charClass[document.GetCharAt(index)] == 0)
            {
                index++;
            }
            if (index == document.Length)
            {
                length = 0;
                nextIndex = index;
                wordType = WordTypeEnum.Eof;
                return;
            }

            length = 0;
            nextIndex = index;
            if (document.GetCharAt(nextIndex) < 128)
            {
                switch (charClass[document.GetCharAt(nextIndex)])
                {
                    case 1: // number
                    case 7: // quote
                        wordType = WordTypeEnum.Number;
                        fetchNextAtNumber(document, ref nextIndex, ref wordType);
                        break;
                    case 2: // alphabet
                        wordType = WordTypeEnum.Text;
                        nextIndex++;
                        while (
                            document.Length > nextIndex &&
                            document.GetCharAt(nextIndex) < 128 &&
                            (
                                charClass[document.GetCharAt(nextIndex)] == 1 ||
                                charClass[document.GetCharAt(nextIndex)] == 2 ||
                                charClass[document.GetCharAt(nextIndex)] == 4 ||
                                charClass[document.GetCharAt(nextIndex)] == 6
                            )
                        )
                        {
                            nextIndex++;
                        }
                        break;
                    case 6: // backquote (compiler directive)
                        wordType = WordTypeEnum.CompilerDirective;
                        nextIndex++;
                        while (
                            document.Length > nextIndex &&
                            document.GetCharAt(nextIndex) < 128 &&
                            (
                                charClass[document.GetCharAt(nextIndex)] == 1 ||
                                charClass[document.GetCharAt(nextIndex)] == 2 ||
                                charClass[document.GetCharAt(nextIndex)] == 4 ||
                                charClass[document.GetCharAt(nextIndex)] == 6
                            )
                        )
                        {
                            nextIndex++;
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
                            document.Length > nextIndex
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
                        while (
                            document.Length > nextIndex &&
                            (
                                document.GetCharAt(nextIndex) != '\n' &&
                                document.GetCharAt(nextIndex) != '\r' &&
                                document.GetCharAt(nextIndex) != ' ' &&
                                document.GetCharAt(nextIndex) != '\t'
                            )
                        )
                        {
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

            //while (document.Length > nextIndex && document.GetCharAt(nextIndex) < 128 && charClass[document.GetCharAt(nextIndex)] == 0)
            //{
            //    nextIndex++;
            //}
        }

        public static void FetchNextUntilEol(ajkControls.Document document, ref int index, out int length, out int nextIndex, out WordTypeEnum wordType)
        {
            // skip blanks before word
            while (document.Length > index && ( document.GetCharAt(index)=='\t' || document.GetCharAt(index) == ' '))
            {
                index++;
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
            while (
                document.Length > nextIndex &&
                (
                    document.GetCharAt(nextIndex) != '\n' &&
                    document.GetCharAt(nextIndex) != '\r'
                )
            )
            {
                nextIndex++;
            }

            length = nextIndex - index;

            //while (document.Length > nextIndex && document.GetCharAt(nextIndex) < 128 && charClass[document.GetCharAt(index)] == 0)
            //{
            //    nextIndex++;
            //}
        }

        private static void fetchNextAtNumber(ajkControls.Document document, ref int nextIndex, ref WordTypeEnum wordType)
        {
            //nextIndex++;
            while (
                document.Length > nextIndex &&
                document.GetCharAt(nextIndex) < 128 &&
                (charClass[document.GetCharAt(nextIndex)] == 1) // 0-9
                )
            {
                nextIndex++;
            }
            if (document.Length <= nextIndex) return;
            if (document.GetCharAt(nextIndex) == '.' | document.GetCharAt(nextIndex) == 'e' || document.GetCharAt(nextIndex) == 'E')
            { // real
                if (document.GetCharAt(nextIndex) == '.')
                {
                    nextIndex++;
                    if (document.Length <= nextIndex) return;
                    while (
                        document.Length > nextIndex &&
                        document.GetCharAt(nextIndex) < 128 &&
                        (charClass[document.GetCharAt(nextIndex)] == 1) // 0-9
                        )
                    {
                        nextIndex++;
                    }
                }

                if (document.GetCharAt(nextIndex) == 'e' || document.GetCharAt(nextIndex) == 'E')
                {
                    nextIndex++;
                    if (document.Length <= nextIndex) return;
                    if (document.GetCharAt(nextIndex) == '+' || document.GetCharAt(nextIndex) == '-')
                    {
                        nextIndex++;
                        if (document.Length <= nextIndex) return;
                    }
                    while (
                        document.Length > nextIndex &&
                        document.GetCharAt(nextIndex) < 128 &&
                        (charClass[document.GetCharAt(nextIndex)] == 1) // 0-9
                        )
                    {
                        nextIndex++;
                    }
                }
            }
            else if (document.GetCharAt(nextIndex) == '\'')
            {
                nextIndex++;
                if (document.Length <= nextIndex) return;

                while (
                    document.Length > nextIndex &&
                    document.GetCharAt(nextIndex) < 128 &&
                    (
                        charClass[document.GetCharAt(nextIndex)] == 1 || // 0-9
                        charClass[document.GetCharAt(nextIndex)] == 2 || // alphabet
                        document.GetCharAt(nextIndex) == '?' ||
                        document.GetCharAt(nextIndex) == 'x' ||
                        document.GetCharAt(nextIndex) == 'X' ||
                        document.GetCharAt(nextIndex) == 'z' ||
                        document.GetCharAt(nextIndex) == 'Z'
                        )
                    )
                {
                    nextIndex++;
                }
            }
            else
            {
                while (
                    document.Length > nextIndex &&
                    document.GetCharAt(nextIndex) < 128 &&
                    (
                        charClass[document.GetCharAt(nextIndex)] == 1 || // 0-9
                        charClass[document.GetCharAt(nextIndex)] == 2 || // alphabet
                        document.GetCharAt(nextIndex) == '?' ||
                        document.GetCharAt(nextIndex) == 'x' ||
                        document.GetCharAt(nextIndex) == 'X' ||
                        document.GetCharAt(nextIndex) == 'z' ||
                        document.GetCharAt(nextIndex) == 'Z'
                        )
                    )
                {
                    nextIndex++;
                }
            }
        }

        private static void fetchNextAtOperator(ajkControls.Document document, ref int nextIndex, ref WordTypeEnum wordType)
        {
            if (document.Length > nextIndex + 1 && document.GetCharAt(nextIndex) == '/' && document.GetCharAt(nextIndex + 1) == '*') // "/*"
            { // comment block
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                while (document.Length > nextIndex)
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
            }
            else if (document.Length > nextIndex + 1 && document.GetCharAt(nextIndex) == '/' && document.GetCharAt(nextIndex + 1) == '/') // "//"
            { // line comment
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                nextIndex++;
                while (document.Length > nextIndex && document.GetCharAt(nextIndex) != '\n')
                {
                    document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                    nextIndex++;
                }
                if (document.Length > nextIndex && document.GetCharAt(nextIndex) == '\n')
                {
                    document.SetColorAt(nextIndex, CodeDrawStyle.ColorIndex(CodeDrawStyle.ColorType.Comment));
                    nextIndex++;
                }
                wordType = WordTypeEnum.Comment;
            }
            else if ((document.Length > nextIndex + 2) && // ">>>","<<<","===","!=="
                (
                    document.CreateString(nextIndex, 3) == ">>>" ||
                    document.CreateString(nextIndex, 3) == "<<<" ||
                    document.CreateString(nextIndex, 3) == "===" ||
                    document.CreateString(nextIndex, 3) == "!=="
                )
            )
            {
                nextIndex = nextIndex + 3;
            }
            else if ((document.Length > nextIndex + 1) && // "==","!=","&&","||",">>","<<","<=",">=","~|","~&","~^","^~","**","(*","*)","//","/*","*/"
               (
                   document.CreateString(nextIndex, 2) == "==" ||
                   document.CreateString(nextIndex, 2) == "!=" ||
                   document.CreateString(nextIndex, 2) == "&&" ||
                   document.CreateString(nextIndex, 2) == "||" ||
                   document.CreateString(nextIndex, 2) == ">>" ||
                   document.CreateString(nextIndex, 2) == "<<" ||
                   document.CreateString(nextIndex, 2) == "<=" ||
                   document.CreateString(nextIndex, 2) == ">=" ||
                   document.CreateString(nextIndex, 2) == "~|" ||
                   document.CreateString(nextIndex, 2) == "~&" ||
                   document.CreateString(nextIndex, 2) == "~^" ||
                   document.CreateString(nextIndex, 2) == "^~" ||
                   document.CreateString(nextIndex, 2) == "**" ||
                   document.CreateString(nextIndex, 2) == "(*" ||
                   document.CreateString(nextIndex, 2) == "*)" ||
                   document.CreateString(nextIndex, 2) == "+:" ||
                   document.CreateString(nextIndex, 2) == "-:" ||
                   document.CreateString(nextIndex, 2) == "=>"
               )
           )
            {
                nextIndex = nextIndex + 2;
            }
            else
            {
                nextIndex++;
            }
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


    }
}

