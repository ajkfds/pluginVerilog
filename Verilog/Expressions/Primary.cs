using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Primary : Expression
    {
        protected Primary() {
            Constant = false;
        }
        /*
        public virtual bool Constant { get; protected set; }
        public virtual double? Value { get; protected set; }
        public virtual int? BitWidth { get; protected set; }
        //        public bool Signed { get; protected set; }
        public WordReference Reference { get; protected set; }

        public static Primary Create(bool constant, double? value, int? bitWidth)
        {
            Primary primary = new Primary();
            primary.Constant = constant;
            primary.Value = value;
            primary.BitWidth = bitWidth;
            return primary;
        }

        public virtual ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            AppendLabel(label);
            return label;
        }

        public virtual string CreateString()
        {
            return "";
        }
        */
/*        public virtual void AppendLabel(ajkControls.ColorLabel label)
        {

        }

        public virtual void AppendString( StringBuilder stringBuilder )
        {

        }
        */
        /*
         * 
         * 
         A.8.4 Primaries
        constant_primary    ::= constant_concatenation
                                | constant_function_call
                                | ( constant_mintypmax_expression )
                                | constant_multiple_concatenation
                                | genvar_identifier
                                | number
                                | parameter_identifier
                                | specparam_identifier  
        module_path_primary ::= number
                                | identifier
                                | module_path_concatenation
                                | module_path_multiple_concatenation
                                | function_call          
                                | system_function_call          
                                | constant_function_call          
                                | ( module_path_mintypmax_expression )  
        primary             ::= number
                                | concatenation          
                                | multiple_concatenation
                                | function_call 
                                | system_function_call
                                | constant_function_call
                                | ( mintypmax_expression )
                                | hierarchical_identifier
                                | hierarchical_identifier [ expression ] { [ expression ] }
                                | hierarchical_identifier [ expression ] { [ expression ] }  [ range_expression ]
                                | hierarchical_identifier [ range_expression ]
        */
        public static Primary ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            return parseCreate(word, nameSpace, false);
        }
        public static Primary ParseCreateLValue(WordScanner word, NameSpace nameSpace)
        {
            return parseCreate(word, nameSpace, true);
        }
        private static Primary parseCreate(WordScanner word, NameSpace nameSpace,bool lValue)
        {
            if (nameSpace == null) System.Diagnostics.Debugger.Break();
            switch (word.WordType)
            {
                case WordPointer.WordTypeEnum.Number:
                    return Number.ParseCreate(word);
                case WordPointer.WordTypeEnum.Symbol:
                    if (word.GetCharAt(0) == '{')
                    {
                        return Concatenation.ParseCreateConcatenationOrMultipleConcatenation(word, nameSpace, lValue);
                    }else if(word.GetCharAt(0) == '(')
                    {
                        return Bracket.ParseCreateBracketOrMinTypMax(word, nameSpace);
                    }
                    return null;
                case WordPointer.WordTypeEnum.String:
                    return ConstantString.ParseCreate(word);
                case WordPointer.WordTypeEnum.Text:
                    {
                        var variable = VariableReference.ParseCreate(word, nameSpace,lValue);
                        if (variable != null) return variable;

                        var parameter = ParameterReference.ParseCreate(word, nameSpace);
                        if (parameter != null) return parameter;

                        if (word.Text.StartsWith("$") && word.RootParsedDocument.ProjectProperty.SystemFunctions.Keys.Contains(word.Text)) {
                            return FunctionCall.ParseCreate(word, nameSpace);
                        }

                        if (word.NextText == "(")
                        {
                            return FunctionCall.ParseCreate(word, nameSpace);
                        }

                        if(word.NextText == ".")
                        {
                            NameSpace space = null;
                            if (nameSpace.Module.ModuleInstantiations.ContainsKey(word.Text))
                            {
                                ModuleItems.ModuleInstantiation inst = nameSpace.Module.ModuleInstantiations[word.Text];
                                space = word.RootParsedDocument.ProjectProperty.GetModule(inst.ModuleName);
                            }
                            else
                            {
                                space = getSpace(word.Text, nameSpace);
                            }

                            if(space == null)
                            { // root module
                                Module module = word.RootParsedDocument.ProjectProperty.GetModule(word.Text);
                                if(module != null)
                                {
                                    word.Color(CodeDrawStyle.ColorType.Keyword);
                                    word.MoveNext();
                                    word.MoveNext(); // .
                                    Primary primary = subParseCreate(word, module, lValue);
                                    if (primary == null) word.AddError("illegal variable");
                                    return primary;
                                }
                            }
                            else
                            { // 
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                word.MoveNext();
                                word.MoveNext(); // .
                                Primary primary = subParseCreate(word, space,lValue);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            }
                        }
                        else if (nameSpace.NameSpaces.ContainsKey(word.Text))
                        {
                            word.Color(CodeDrawStyle.ColorType.Identifier);
                            NameSpace space = nameSpace.NameSpaces[word.Text];
                            if (space == null) return null;
                            word.MoveNext();
                            return new NameSpaceReference(space);
                        }
                        else if (nameSpace.Module.ModuleInstantiations.ContainsKey(word.Text))
                        {
                            word.Color(CodeDrawStyle.ColorType.Identifier);
                            ModuleItems.ModuleInstantiation minst = nameSpace.Module.ModuleInstantiations[word.Text];
                            ModuleInstanceReference moduleInstanceReference = new ModuleInstanceReference(minst);
                            word.MoveNext();
                            return moduleInstanceReference;
                        }


                        if (word.Eof) return null;
                        if (General.ListOfKeywords.Contains(word.Text)) return null;
                        if(General.IsIdentifier(word.Text) && !nameSpace.Variables.ContainsKey(word.Text) && !word.Prototype)
                        {   // undefined net
                            if(!word.CellDefine) word.AddWarning("undefined");
                            Variables.Net net = new Variables.Net();
                            net.Name = word.Text;
                            net.Signed = false;
                            if (word.Active)
                            {
                                nameSpace.Variables.Add(net.Name, net);
                            }
                            variable = VariableReference.ParseCreate(word, nameSpace,lValue);
                            if (variable != null) return variable;
                        }
                    }
                    break;
            }
            return null;
        }

        private static Primary subParseCreate(WordScanner word, NameSpace nameSpace,bool lValue)
        {
            if (nameSpace == null) System.Diagnostics.Debugger.Break();
            switch (word.WordType)
            {
                case WordPointer.WordTypeEnum.Number:
                    return null;
                case WordPointer.WordTypeEnum.Symbol:
                    return null;
                case WordPointer.WordTypeEnum.String:
                    return null;
                case WordPointer.WordTypeEnum.Text:
                    {
                        var variable = VariableReference.ParseCreate(word, nameSpace,lValue);
                        if (variable != null) return variable;

                        var parameter = ParameterReference.ParseCreate(word, nameSpace);
                        if (parameter != null) return parameter;

                        if (!lValue && word.NextText == "(")
                        {
                            return FunctionCall.ParseCreate(word, nameSpace);
                        }

                        if (word.NextText == ".")
                        {
//                            if(word.RootParsedDocument.Project.)
                           if (nameSpace.Module.ModuleInstantiations.ContainsKey(word.Text))
                            { // module instancce
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                string moduleName = nameSpace.Module.ModuleInstantiations[word.Text].ModuleName;
                                Module module = word.RootParsedDocument.ProjectProperty.GetModule(moduleName);
                                if (module == null) return null;
                                word.MoveNext();
                                word.MoveNext(); // .

                                Primary primary = subParseCreate(word, module,lValue);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            } else if (nameSpace.NameSpaces.ContainsKey(word.Text))
                            { // namespaces
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                NameSpace space = nameSpace.NameSpaces[word.Text];
                                if (space == null) return null;
                                word.MoveNext();
                                word.MoveNext(); // .

                                Primary primary = subParseCreate(word, space, lValue);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            }
                        }
                        else
                        {
                            if (nameSpace.Module.ModuleInstantiations.ContainsKey(word.Text))
                            { // module instance
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                string moduleName = nameSpace.Module.ModuleInstantiations[word.Text].ModuleName;
                                Module module = word.RootParsedDocument.ProjectProperty.GetModule(moduleName);
                                if (module == null) return null;
                                word.MoveNext();

                                Primary primary = subParseCreate(word, module, lValue);
                                if (primary == null) word.AddError("illegal variable");
                                return new NameSpaceReference(module);

                            }else if(nameSpace is Module && nameSpace.Module.Tasks.ContainsKey(word.Text))
                            {
                                Primary primary = new TaskReference(nameSpace.Module.Tasks[word.Text], nameSpace.Module);
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                word.MoveNext();
                                return primary;
                            }
                            else if (nameSpace.NameSpaces.ContainsKey(word.Text))
                            {
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                NameSpace space = nameSpace.NameSpaces[word.Text];
                                if (space == null) return null;
                                word.MoveNext();
                                return new NameSpaceReference(space);

                            }
                        }

                        if (word.Eof || General.ListOfKeywords.Contains(word.Text))
                        {
                            return new NameSpaceReference(nameSpace);
                        }
//                            return null;
                    }
                    break;
            }
            return null;
        }

        private static NameSpace getSpace(string identifier, NameSpace nameSpace)
        {
            if (nameSpace.NameSpaces.ContainsKey(identifier))
            {
                return nameSpace.NameSpaces[identifier];
            }
            if (nameSpace.Parent == null) return null;

            return getSpace(identifier, nameSpace.Parent);
        }
    }






}



