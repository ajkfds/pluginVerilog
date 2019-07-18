using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.ModuleItems
{
    public class ModuleInstantiation : Item
    {
        protected ModuleInstantiation() { }

        public string ModuleName{ get; protected set; }

        private List<Verilog.Variables.Port> ports = new List<Variables.Port>();
        public IReadOnlyList<Verilog.Variables.Port> Ports { get { return ports; } }
        public int BeginIndex;
        public int LastIndex;
        public static void Parse(WordScanner word, IModuleOrGeneratedBlock module)
        {

            WordScanner moduleIdentifier = word.Clone();
            string moduleName = word.Text;
            int beginIndex = word.RootIndex;
            word.MoveNext();

            string next = word.NextText;
            if(word.Text != "#" && next != "(" && next != ";" && General.IsIdentifier(word.Text))
            {
                moduleIdentifier.AddError("illegal module item");
                return;
            }
            moduleIdentifier.Color(CodeDrawStyle.ColorType.Keyword);

            if (word.Text == "#") // parameter
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();

                if (word.Text != "(")
                {
                    word.AddError("( expected");
                    return;
                }
                word.MoveNext();

                if (word.Text == ".")
                { // named parameter assignment
                    while (!word.Eof && word.Text == ".")
                    {
                        word.MoveNext();
                        word.Color(CodeDrawStyle.ColorType.Paramater);
                        word.MoveNext();
                        if (word.Text != "(")
                        {
                            word.AddError("( expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module as NameSpace);
                        if (word.Text != ")")
                        {
                            word.AddError(") expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        if (word.Text != ",")
                        {
                            break;
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                }
                else
                { // ordered paramater assignment
                    while (!word.Eof && word.Text != ")")
                    {
                        Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module as NameSpace);
                        if (word.Text != ",")
                        {
                            break;
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                }

                if (word.Text != ")")
                {
                    word.AddError("( expected");
                    return;
                }
                word.MoveNext();
            }


            while (!word.Eof)
            {
                ModuleInstantiation moduleInstantiation = new ModuleInstantiation();
                moduleInstantiation.ModuleName = moduleName;
                moduleInstantiation.BeginIndex = beginIndex;

                Module instancedModule = word.RootParsedDocument.ProjectProperty.GetModule(moduleName);

                word.Color(CodeDrawStyle.ColorType.Identifier);
                if (General.IsIdentifier(word.Text))
                {
                    moduleInstantiation.Name = word.Text;
                }
                else
                {
                    if (word.Prototype) word.AddError("illegal instance name");
                }

                if (word.Prototype)
                {
                    if (moduleInstantiation.Name == null)
                    {
                        // 
                    }
                    else if (module.Module.ModuleInstantiations.ContainsKey(moduleInstantiation.Name))
                    {   // duplicated
                        word.AddError("instance name duplicated");
                    }
                    else
                    {
                        module.Module.ModuleInstantiations.Add(moduleInstantiation.Name, moduleInstantiation);
                    }
                }
                else
                {
                    if (moduleInstantiation.Name == null)
                    {
                        // 
                    }
                    else if (module.Module.ModuleInstantiations.ContainsKey(moduleInstantiation.Name))
                    {   // duplicated
                        moduleInstantiation = module.Module.ModuleInstantiations[moduleInstantiation.Name];
                    }
                    else
                    {
                        //module.ModuleInstantiations.Add(moduleInstantiation.Name, moduleInstantiation);
                    }
                }

                word.MoveNext();

                if (word.Text != "(")
                {
                    word.AddError("( expected");
                    return;
                }
                word.MoveNext();

                if (word.GetCharAt(0) == '.')
                { // named parameter assignment
                    while (!word.Eof && word.Text == ".")
                    {
                        word.MoveNext();
                        string pinName = word.Text;
                        bool outPort = false;
                        word.Color(CodeDrawStyle.ColorType.Identifier);
                        if (instancedModule != null && !word.Prototype) {
                            if (instancedModule.Ports.ContainsKey(pinName))
                            {
                                if(instancedModule.Ports[pinName].Direction == Variables.Port.DirectionEnum.Output
                                    || instancedModule.Ports[pinName].Direction == Variables.Port.DirectionEnum.Inout )
                                {
                                    outPort = true;
                                }
                            }
                            else
                            {
                                word.AddError("illegal port name");
                            }
                        }
                        word.MoveNext();
                        if (word.Text != "(")
                        {
                            word.AddError("( expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        if (outPort)
                        {
                            Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module as NameSpace,true);
                        }
                        else
                        {
                            Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module as NameSpace);
                        }
                        if (word.Text != ")")
                        {
                            word.AddError(") expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        if (word.Text != ",")
                        {
                            break;
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                }
                else
                { // ordered paramater assignment
                    while (!word.Eof && word.Text != ")")
                    {
                        Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module as NameSpace);
                        if (word.Text != ",")
                        {
                            break;
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                }
                if (word.Text != ")")
                {
                    word.AddError(") expected");
                    return;
                }
                word.MoveNext();
                moduleInstantiation.LastIndex = word.RootIndex;

                if (!word.Prototype && word.Active) word.AppendBlock(moduleInstantiation.BeginIndex, moduleInstantiation.LastIndex);
                if (word.Text != ",") break;
                word.MoveNext();
            }

            if (word.Text != ";")
            {
                word.AddError("; expected");
                return;
            }
            word.MoveNext();
        }

        /*
        module_instantiation            ::= module_identifier [ parameter_value_assignment ] module_instance { , module_instance } ;
        parameter_value_assignment      ::= # ( list_of_parameter_assignments )  
        list_of_parameter_assignments   ::= ordered_parameter_assignment { , ordered_parameter_assignment } | named_parameter_assignment { , named_parameter_assignment }  
        ordered_parameter_assignment    ::= expression  named_parameter_assignment ::= .parameter_identifier ( [ expression ] ) 
        module_instance                 ::= name_of_instance ( [ list_of_port_connections ] )
        name_of_instance                ::= module_instance_identifier [ range ]  
        list_of_port_connections        ::= ordered_port_connection { , ordered_port_connection }          | named_port_connection { , named_port_connection }  
        ordered_port_connection         ::= { attribute_instance } [ expression ]
        named_port_connection           ::= { attribute_instance } .port_identifier ( [ expression ] )  
         */
    }
}
