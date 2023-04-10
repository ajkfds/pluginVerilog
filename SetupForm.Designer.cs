namespace pluginVerilog
{
    partial class SetupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.IcarusVerilogTsmi = new System.Windows.Forms.ToolStripMenuItem();
            this.iVerilogRunTsmi = new System.Windows.Forms.ToolStripMenuItem();
            this.gtkWaveTsmi = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateVerilogFileTsmi = new System.Windows.Forms.ToolStripMenuItem();
            this.VerilogDebugTsmi = new System.Windows.Forms.ToolStripMenuItem();
            this.checkParseDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IcarusVerilogTsmi,
            this.addToolStripMenuItem,
            this.VerilogDebugTsmi});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(185, 100);
            // 
            // IcarusVerilogTsmi
            // 
            this.IcarusVerilogTsmi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iVerilogRunTsmi,
            this.gtkWaveTsmi});
            this.IcarusVerilogTsmi.Name = "IcarusVerilogTsmi";
            this.IcarusVerilogTsmi.Size = new System.Drawing.Size(184, 32);
            this.IcarusVerilogTsmi.Text = "icarusVerilog";
            // 
            // iVerilogRunTsmi
            // 
            this.iVerilogRunTsmi.Name = "iVerilogRunTsmi";
            this.iVerilogRunTsmi.Size = new System.Drawing.Size(188, 34);
            this.iVerilogRunTsmi.Text = "Run";
            this.iVerilogRunTsmi.Click += new System.EventHandler(this.IVerilogRunTsmi_Click);
            // 
            // gtkWaveTsmi
            // 
            this.gtkWaveTsmi.Name = "gtkWaveTsmi";
            this.gtkWaveTsmi.Size = new System.Drawing.Size(188, 34);
            this.gtkWaveTsmi.Text = "GTKWave";
            this.gtkWaveTsmi.Click += new System.EventHandler(this.GtkWaveTsmi_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateVerilogFileTsmi});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(184, 32);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // CreateVerilogFileTsmi
            // 
            this.CreateVerilogFileTsmi.Name = "CreateVerilogFileTsmi";
            this.CreateVerilogFileTsmi.Size = new System.Drawing.Size(195, 34);
            this.CreateVerilogFileTsmi.Text = "VerilogFile";
            this.CreateVerilogFileTsmi.Click += new System.EventHandler(this.CreateVerilogFileTsmi_Click);
            // 
            // VerilogDebugTsmi
            // 
            this.VerilogDebugTsmi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkParseDataToolStripMenuItem});
            this.VerilogDebugTsmi.Name = "VerilogDebugTsmi";
            this.VerilogDebugTsmi.Size = new System.Drawing.Size(184, 32);
            this.VerilogDebugTsmi.Text = "Debug";
            // 
            // checkParseDataToolStripMenuItem
            // 
            this.checkParseDataToolStripMenuItem.Name = "checkParseDataToolStripMenuItem";
            this.checkParseDataToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.checkParseDataToolStripMenuItem.Text = "Check Parse Data";
            this.checkParseDataToolStripMenuItem.Click += new System.EventHandler(this.checkParseDataToolStripMenuItem_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "box.png");
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 326);
            this.Name = "SetupForm";
            this.Text = "SetupForm";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ImageList imageList;
        public System.Windows.Forms.ToolStripMenuItem IcarusVerilogTsmi;
        private System.Windows.Forms.ToolStripMenuItem iVerilogRunTsmi;
        private System.Windows.Forms.ToolStripMenuItem gtkWaveTsmi;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem CreateVerilogFileTsmi;
        public System.Windows.Forms.ToolStripMenuItem VerilogDebugTsmi;
        private System.Windows.Forms.ToolStripMenuItem checkParseDataToolStripMenuItem;
    }
}