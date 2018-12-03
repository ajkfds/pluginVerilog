using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    public class ProjectProperty : codeEditor.Data.ProjectProperty
    {
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
        // macros
        public Dictionary<string, string> Macros = new Dictionary<string, string>();

    }
}
