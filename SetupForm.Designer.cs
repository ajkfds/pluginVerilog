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
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IcarusVerilogTsmi});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(241, 67);
            // 
            // IcarusVerilogTsmi
            // 
            this.IcarusVerilogTsmi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iVerilogRunTsmi,
            this.gtkWaveTsmi});
            this.IcarusVerilogTsmi.Name = "IcarusVerilogTsmi";
            this.IcarusVerilogTsmi.Size = new System.Drawing.Size(240, 30);
            this.IcarusVerilogTsmi.Text = "icarusVerilog";
            // 
            // iVerilogRunTsmi
            // 
            this.iVerilogRunTsmi.Name = "iVerilogRunTsmi";
            this.iVerilogRunTsmi.Size = new System.Drawing.Size(252, 30);
            this.iVerilogRunTsmi.Text = "Run";
            this.iVerilogRunTsmi.Click += new System.EventHandler(this.IVerilogRunTsmi_Click);
            // 
            // gtkWaveTsmi
            // 
            this.gtkWaveTsmi.Name = "gtkWaveTsmi";
            this.gtkWaveTsmi.Size = new System.Drawing.Size(252, 30);
            this.gtkWaveTsmi.Text = "GTKWave";
            this.gtkWaveTsmi.Click += new System.EventHandler(this.GtkWaveTsmi_Click);
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
    }
}