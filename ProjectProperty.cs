using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    public class ProjectProperty : codeEditor.Data.ProjectProperty
    {
        public ProjectProperty(codeEditor.Data.Project project)
        {
            this.project = project;
        }
        private codeEditor.Data.Project project;

        public Verilog.Snippets.Setup SnippetSetup = new Verilog.Snippets.Setup();

        public override void SaveSetup(ajkControls.JsonWriter writer)
        {
            using (var macroWriter = writer.GetObjectWriter("Macros"))
            {
                foreach (var kvp in Macros)
                {
                    macroWriter.writeKeyValue(kvp.Key, kvp.Value.MacroText);
                }
            }
        }

        // VerilogModuleParsedData

        private Dictionary<string, codeEditor.CodeEditor.ParsedDocument> pdocs = new Dictionary<string, codeEditor.CodeEditor.ParsedDocument>();
        public IReadOnlyDictionary<string, codeEditor.CodeEditor.ParsedDocument> VerilogModuleInstanceParsedDocuments
        {
            get
            {
                return pdocs;
            }
        }
        public void RegisterParsedDocument(string ID,codeEditor.CodeEditor.ParsedDocument parsedDocument)
        {
            if (pdocs.ContainsKey(ID))
            {
                pdocs.Remove(ID);
            }
            pdocs.Add(ID, parsedDocument);
        }

        public void RemoveRegisteredParsedDocument(string ID,codeEditor.CodeEditor.ParsedDocument parsedDocument)
        {
            if (pdocs.ContainsKey(ID))
            {
                pdocs.Remove(ID);
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public bool IsRegisteredParsedDocument(string ID)
        {
            if (pdocs.ContainsKey(ID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public codeEditor.CodeEditor.ParsedDocument GetParsedDocument(string id)
        {
            if (id == null) System.Diagnostics.Debugger.Break();

            if (pdocs.ContainsKey(id))
            {
                return pdocs[id];
            }
            else
            {
                return null;
            }
        }


        // module reference table
        private Dictionary<string, string> relativeFilePathWithModuleName = new Dictionary<string, string>();

        public bool RegisterModule(string relativeFilePath, string moduleName)
        {
            if (relativeFilePathWithModuleName.ContainsKey(moduleName))
            {
                if (relativeFilePathWithModuleName[moduleName] == relativeFilePath)
                {
                    return true;    // same item registered
                }
                else
                {
                    return false; // duplicated module name
                }
            }
            else
            {
                relativeFilePathWithModuleName.Add(moduleName, relativeFilePath);
                return true;
            }
        }

        public bool RemoveModule(string relativeFilePath, string moduleName)
        {
            if (relativeFilePathWithModuleName.ContainsKey(moduleName))
            {
                if (relativeFilePathWithModuleName[moduleName] == relativeFilePath)
                {
                    relativeFilePathWithModuleName.Remove(moduleName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsRegisterableModule(string relativeFilePath, string moduleName)
        {
            if (relativeFilePathWithModuleName.ContainsKey(moduleName))
            {
                if (relativeFilePathWithModuleName[moduleName] == relativeFilePath)
                {
                    return true;    // same item registered
                }
                else
                {
                    return false; // duplicated module name
                }
            }
            else
            {
                return true;
            }
        }

        public string GetRelativeFilePathOfModule(string moduleName)
        {
            if (!relativeFilePathWithModuleName.ContainsKey(moduleName))
            {
                return null;
            }
            else
            {
                return relativeFilePathWithModuleName[moduleName];
            }
        }

        public List<string> GetModuleNameList()
        {
            List<string> list = new List<string>();
            foreach (string name in relativeFilePathWithModuleName.Keys)
            {
                list.Add(name);
            }
            return list;
        }

        public Verilog.Module GetModule(string moduleName)
        {
            string fileRelativePath = GetRelativeFilePathOfModule(moduleName);
            if (fileRelativePath == null) return null;
            Data.VerilogFile file = project.GetRegisterdItem(codeEditor.Data.File.GetID(fileRelativePath, project)) as Data.VerilogFile;
            if (file == null || file.VerilogParsedDocument == null) return null;
            if (!file.VerilogParsedDocument.Modules.ContainsKey(moduleName)) return null;
            return file.VerilogParsedDocument.Modules[moduleName];
        }

        // macros
        public Dictionary<string, Verilog.Macro> Macros = new Dictionary<string, Verilog.Macro>();

        // system tasks
        public Dictionary<string, Action<Verilog.WordScanner>> SystemTaskParsers = new Dictionary<string, Action<Verilog.WordScanner>>
        {
            // Display task
            {"$display",null },
            {"$strobe",null },
            {"$displayb",null },
            {"$strobeb",null },
            {"$displayh",null },
            {"$strobeh",null },
            {"$displayo",null },
            {"$strobeo", null },
            {"$monitor", null },
            {"$write", null },
            {"$monitorb", null },
            {"$writeb", null },
            {"$monitorh", null },
            {"$writeh", null },
            {"$monitoro", null },
            {"$writeo", null },
            {"$monitoroff", null },
            {"$monitoron", null },
            // File I/O tasks
            {"$fclose", null },
            {"$fdisplay", null },
            {"$fstrobe", null } ,
            {"$fdisplayb", null },
            {"$fstrobeb", null },
            {"$fdisplayh", null },
            {"$fstrobeh", null },
            {"$fdisplayo", null },
            {"$fstrobeo", null },
            {"$ungetc", null },
            {"$fflush", null },
            {"$fmonitor", null },
            {"$fwrite", null },
            {"$fmonitorb", null },
            {"$fwriteb", null },
            {"$fmonitorh", null },
            {"$fwriteh", null },
            {"$fmonitoro", null },
            {"$fwriteo", null },
            {"$readmemb", null },
            {"$readmemh", null },
            {"$swrite", null },
            {"$swriteb", null },
            {"$swriteo", null },
            {"$swriteh", null } ,
            {"$sdf_annotate", null } ,
            // Timescale tasks
            {"$printtimescale", null },
            {"$timeformat", null },
            // Simulation control tasks
            {"$finish", null },
            {"$stop", null },
            // PLA modeling tasks
            {"$async$and$array", null },
            {"$async$and$plane", null },
            {"$async$nand$array", null },
            {"$async$nand$plane", null },
            {"$async$or$array", null },
            {"$async$or$plane", null },
            {"$async$nor$array", null },
            {"$async$nor$plane", null },
            {"$sync$and$array", null },
            {"$sync$and$plane", null },
            {"$sync$nand$array", null },
            {"$sync$nand$plane", null },
            {"$sync$or$array", null },
            {"$sync$or$plane", null },
            {"$sync$nor$array", null },
            {"$sync$nor$plane", null },
            // Stochastic analysis tasks
            {"$q_initialize", null },
            {"$q_add", null },
            {"$q_remove", null },
            {"$q_full", null },
            {"$q_exam", null },

            // Dump
            {"$dumpfile",null },
            {"$dumpall",null },
            {"$dumpoff",null },
            {"$dumpon",null },
            {"$dumpvars",null },
            {"$dumpflush",null },
            {"$dumplimit",null },
        };

        public Dictionary<string, Func<Verilog.Variables.Variable, Verilog.WordScanner>> SystemFunctions = new Dictionary<string, Func<Verilog.Variables.Variable, Verilog.WordScanner>>
        {
            {"$sformat", null },
            {"$ferror", null },
            {"$rewind", null },
            {"$fseek", null },
            {"$fread", null },
            {"$ftell", null } ,
            {"$sscanf", null } ,
            {"$fscanf", null },
            {"$fgetc", null },
            {"$fgets", null },
            {"$fopen", null },

            // Simulation time functions
            {"$realtime",null },
            {"$stime",null },
            {"$time",null },

            // Conversion functions
            {"$bitstoreal",null },
            {"$realtobits",null },
            {"$itor",null },
            {"$rtoi",null },
            {"$signed",null },
            {"$unsigned",null },

            // Probabilistic distribution functions
            {"$dist_chi_square",null },
            {"$dist_erlang",null },
            {"$dist_exponential",null },
            {"$dist_normal",null },
            {"$dist_poisson",null },
            {"$dist_t",null },
            {"$dist_uniform",null },
            {"$random",null },

            // Command line input
            {"$test$plusargs",null },
            {"$value$plusargs",null },

    };
    }
}
