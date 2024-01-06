using pluginVerilog.Verilog.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Task : NameSpace,IPortNameSpace
    {
        /*
        task_declaration ::=    (From Annex A - A.2.7)  
                                task    [ automatic ] task_identifier ;                     { task_item_declaration }   statement   endtask 
                                | task  [ automatic ] task_identifier ( task_port_list ) ;  { block_item_declaration }  statement   endtask
        task_item_declaration ::=   block_item_declaration 
                                    | { attribute_instance } tf_ input_declaration ; 
                                    | { attribute_instance } tf_output_declaration ; 
                                    | { attribute_instance } tf_inout_declaration ; 
        task_port_list ::= task_port_item { , task_port_item }  
        task_port_item ::=  { attribute_instance } tf_input_declaration 
                            | { attribute_instance } tf_output_declaration 
                            | { attribute_instance } tf_inout_declaration
        tf_input_declaration    ::= input [ reg ] [ signed ] [ range ] list_of_port_identifiers 
                                    | input [ task_port_type ] list_of_port_identifiers
        tf_output_declaration   ::= output [ reg ] [ signed ] [ range ] list_of_port_identifiers 
                                    | output [ task_port_type ] list_of_port_identifiers 
        tf_inout_declaration    ::= inout [ reg ] [ signed ] [ range ] list_of_port_identifiers 
                                    | inout [ task_port_type ] list_of_port_identifiers
        task_port_type          ::= time 
                                    | real 
                                    | realtime 
                                    | integer
        block_item_declaration  ::= (From Annex A - A.2.8)
                                    { attribute_instance } block_reg_declaration 
                                    | { attribute_instance } event_declaration 
                                    | { attribute_instance } integer_declaration 
                                    | { attribute_instance } local_parameter_declaration 
                                    | { attribute_instance } parameter_declaration 
                                    | { attribute_instance } real_declaration 
                                    | { attribute_instance } realtime_declaration 
                                    | { attribute_instance } time_declaration
        block_reg_declaration               ::= reg [ signed ] [ range ]   list_of_block_variable_identifiers ;  
        list_of_block_variable_identifiers  ::= block_variable_type { , block_variable_type } 
        block_variable_type                 ::= variable_identifier | variable_identifier dimension { dimension }
         */
        protected Task(NameSpace parent) : base(parent.BuildingBlock , parent)
        {
        }

        private Dictionary<string, DataObjects.Port> ports = new Dictionary<string, DataObjects.Port>();
        public Dictionary<string, DataObjects.Port> Ports { get { return ports; } }
        private List<DataObjects.Port> portsList = new List<DataObjects.Port>();
        public List<DataObjects.Port> PortsList { get { return portsList; } }

        public Statements.IStatement Statement;

        public static void Parse(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "task")
            {
                System.Diagnostics.Debugger.Break();
            }
            Task task = new Task(nameSpace);
            task.BuildingBlock = nameSpace.BuildingBlock;
            word.Color(CodeDrawStyle.ColorType.Keyword);
            task.BeginIndexReference = word.CreateIndexReference();
            word.MoveNext();

            if (word.Text == "automatic")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            if (!General.IsIdentifier(word.Text))
            {
                word.AddError("illegal identifier name");
                return;
            }

            task.Name = word.Text;
            word.Color(CodeDrawStyle.ColorType.Identifier);
//            if (word.Active && module.Tasks.ContainsKey(task.Name) || module.NameSpaces.ContainsKey(task.Name))
//            {
//                word.AddError("duplicated name");
//            }
            if (!word.Active)
            {
                // skip
            }
            else if (word.Prototype)
            {
                if (!nameSpace.BuildingBlock.Tasks.ContainsKey(task.Name) && !nameSpace.BuildingBlock.NameSpaces.ContainsKey(task.Name))
                {
                    nameSpace.BuildingBlock.Tasks.Add(task.Name, task);
                    nameSpace.BuildingBlock.NameSpaces.Add(task.Name, task);
                }
                else
                {
                    word.AddError("duplicated name");
                }
            }
            else
            {
                if (nameSpace.BuildingBlock.Tasks.ContainsKey(task.Name))
                {
                    task = nameSpace.BuildingBlock.Tasks[task.Name];
                }
            }


            word.MoveNext();


            /*            A.2.8 Block item declarations
                        block_item_declaration ::=            { attribute_instance }
                        block_reg_declaration | { attribute_instance }
                        event_declaration | { attribute_instance }
                        integer_declaration | { attribute_instance }
                        local_parameter_declaration | { attribute_instance }
                        parameter_declaration | { attribute_instance }
                        real_declaration | { attribute_instance }
                        realtime_declaration | { attribute_instance }
                        time_declaration block_reg_declaration ::= reg[signed][range]                    list_of_block_variable_identifiers;
            */
            if (word.Text == ";")
            {
                word.MoveNext();
                while (!word.Eof)
                {
                    switch (word.Text)
                    {
                        case "input":
                        case "output":
                        case "inout":
                            Verilog.DataObjects.Port.ParseTfPortDeclaration(word, task);
                            if (word.Text != ";")
                            {
                                word.AddError("; expected");
                            }
                            else
                            {
                                word.MoveNext();
                            }
                            continue;
                        case "reg":
                        case "integer":
                        case "real":
                            DataObjects.Variables.Variable.ParseDeclaration(word, task);
                            continue;
                        default:
                            break;
                    }
                    break;
                }

            }
            else
            {

            }

//            if (module.Tasks.ContainsKey(task.Name) && module.Tasks[task.Name].BeginIndex == task.BeginIndex)
//            {
//                task = module.Tasks[task.Name];
//            }

            if (word.Text == "endtask")
            {
                word.AddError("statement required");
            }
            else
            {
                Statements.IStatement statement = Statements.Statements.ParseCreateFunctionStatement(word, task);
            }

            if (word.Text != "endtask")
            {
                word.AddError("endtask expected");
                return;
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            task.LastIndexReference = word.CreateIndexReference();
            word.AppendBlock(task.BeginIndexReference, task.LastIndexReference);
            word.MoveNext();

            if (word.Prototype)
            {
                if (nameSpace.BuildingBlock.Tasks.ContainsKey(task.Name)) return;
                if (nameSpace.BuildingBlock.NameSpaces.ContainsKey(task.Name)) return;
                nameSpace.BuildingBlock.Tasks.Add(task.Name, task);
                nameSpace.BuildingBlock.NameSpaces.Add(task.Name, task);
            }

            return;

        }

    }
}
