using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pluginVerilog.IcarusVerilog
{


    public partial class SimulationPanel : UserControl
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll",CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern uint GetShortPathName( [System.Runtime.InteropServices.MarshalAs(
            System.Runtime.InteropServices.UnmanagedType.LPTStr)]
            string lpszLongPath,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)] System.Text.StringBuilder lpszShortPath,
            uint cchBuffer);

        public string getShortPath(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
            uint ret = GetShortPathName(path, sb, (uint)sb.Capacity);
            if (ret == 0) return path;
            return sb.ToString();
        }

        public SimulationPanel(Data.VerilogFile topFile)
        {
            InitializeComponent();
            Disposed += disposed;
            thread = new System.Threading.Thread(run);
            thread.Start();
        }

        private void receiveLineString(string lineString)
        {
            logView.AppendLogLine(lineString);
        }

        private void disposed(object sender,EventArgs e)
        {
            shell.Dispose();
        }

        ajkControls.CommandShell shell = null;

        System.Threading.Thread thread = null;

        private void run()
        {
            string projectName;
            string topId;
            codeEditor.Global.Controller.NavigatePanel.GetSelectedNode(out projectName, out topId);
            if (projectName == "" || topId == "") return;
            if (!codeEditor.Global.Projects.ContainsKey(projectName)) return;
            codeEditor.Data.Project project = codeEditor.Global.Projects[projectName];

            Data.VerilogFile topFile = project.GetRegisterdItem(topId) as Data.VerilogFile;
            if (topFile == null) return;
            Verilog.ParsedDocument topParsedDocument = topFile.ParsedDocument as Verilog.ParsedDocument;
            if (topParsedDocument == null) return;
            if (topParsedDocument.Modules.Count == 0) return;

            string simName = topFile.Name.Substring(0, topFile.Name.LastIndexOf('.'));


            string simulationPath = Setup.SimulationPath + "\\" + simName;
            if (!System.IO.Directory.Exists(simulationPath))
            {
                System.IO.Directory.CreateDirectory(simulationPath);
            }

            List<string> filePathList = new List<string>();
            {
                string absolutePath = project.GetAbsolutePath(topFile.RelativePath);
                filePathList.Add(absolutePath);
            }

            List<string> includeFileList = new List<string>();

            foreach (Verilog.Module module in topParsedDocument.Modules.Values)
            {
                appendFiles(filePathList,includeFileList, module, project);
            }



            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(simulationPath + "\\command"))
            {
                foreach (string includePath in includeFileList)
                { 
                    sw.WriteLine("+incdir+" + getShortPath(includePath)); // path with space is not accepted
                }
                foreach (string absolutePath in filePathList)
                {
                    sw.WriteLine(absolutePath);
                }
            }

            shell = new ajkControls.CommandShell(new List<string> {
                "prompt icarusVerilogShell$G$_",
                "cd "+simulationPath
            });
            shell.LineReceived += receiveLineString;
            shell.Start();

            while(shell.GetLastLine() != "icarusVerilogShell>") { System.Threading.Thread.Sleep(10); }
            shell.ClearLogs();
            shell.StartLogging();
            shell.Execute(Setup.BinPath + "iverilog -f command -o " + simName + ".o");
            while (shell.GetLastLine() != "icarusVerilogShell>") { System.Threading.Thread.Sleep(10); }
            List<string> logs = shell.GetLogs();
            if(logs.Count != 3 || logs[1] !="")
            {
                return;
            }
            shell.EndLogging();
            shell.Execute(Setup.BinPath + "vvp " + simName + ".o");


        }

        private void appendFiles(List<string> filePathList, List<string> includePathList, Verilog.Module module, codeEditor.Data.Project project)
        {
            string fileId = module.FileId;
            Data.VerilogFile file = project.GetRegisterdItem(fileId) as Data.VerilogFile;
            if (file == null) return;
            string absolutePath = project.GetAbsolutePath(file.RelativePath);
            if (!filePathList.Contains(absolutePath)) filePathList.Add(absolutePath);

            if (file.VerilogParsedDocument == null) return;

            // includes
            foreach(var include in file.VerilogParsedDocument.IncludeFiles.Values)
            {
                string includePath = project.GetAbsolutePath(include.RelativePath);
                includePath = includePath.Substring(0, includePath.LastIndexOf('\\'));
                if (!includePathList.Contains(includePath)) includePathList.Add(includePath);
            }

            foreach(Verilog.ModuleItems.ModuleInstantiation instance in module.ModuleInstantiations.Values)
            {
                Verilog.Module subModule = project.GetPluginProperty().GetModule(instance.ModuleName);
                if (subModule != null) appendFiles(filePathList, includePathList, subModule, project);
            }

        }

    }
}
