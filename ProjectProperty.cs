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
            using(var macroWriter = writer.GetObjectWriter("Macros"))
            {
                foreach(var kvp in Macros)
                {
                    macroWriter.writeKeyValue(kvp.Key, kvp.Value);
                }
            }
        }

        // module reference table
        private Dictionary<string, string> relativeFilePathWithModuleName = new Dictionary<string, string>();

        public bool RegisterModule(string relativeFilePath,string moduleName)
        {
            if (relativeFilePathWithModuleName.ContainsKey(moduleName))
            {
                if(relativeFilePathWithModuleName[moduleName] == relativeFilePath)
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
                if( relativeFilePathWithModuleName[moduleName] == relativeFilePath)
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
            foreach(string name in relativeFilePathWithModuleName.Keys)
            {
                list.Add(name);
            }
            return list;
        }

        public Verilog.Module GetModule(string moduleName)
        {
            string fileRelativePath = GetRelativeFilePathOfModule(moduleName);
            if (fileRelativePath == null) return null;
            Data.VerilogFile file = project.GetRegisterdItem(codeEditor.Data.File.GetID(fileRelativePath,project)) as Data.VerilogFile;
            if (file == null || file.VerilogParsedDocument == null) return null;
            if (!file.VerilogParsedDocument.Modules.ContainsKey(moduleName)) return null;
            return file.VerilogParsedDocument.Modules[moduleName];
        }

        // macros
        public Dictionary<string, string> Macros = new Dictionary<string, string>();

    }
}
