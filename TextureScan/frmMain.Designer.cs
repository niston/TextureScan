
namespace TextureScan
{
    partial class frmMain
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtScanPath = new System.Windows.Forms.TextBox();
            this.lsvResults = new System.Windows.Forms.ListView();
            this.colFilePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDimensions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMipmapCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colIsCubemap = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 396);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(952, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslStatus
            // 
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Size = new System.Drawing.Size(937, 17);
            this.tslStatus.Spring = true;
            this.tslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lsvResults);
            this.splitContainer1.Size = new System.Drawing.Size(952, 396);
            this.splitContainer1.SplitterDistance = 51;
            this.splitContainer1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGo);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtScanPath);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(952, 51);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scan Folder";
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.Location = new System.Drawing.Point(893, 20);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(46, 20);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(859, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 20);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtScanPath
            // 
            this.txtScanPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScanPath.Location = new System.Drawing.Point(13, 20);
            this.txtScanPath.Name = "txtScanPath";
            this.txtScanPath.Size = new System.Drawing.Size(840, 20);
            this.txtScanPath.TabIndex = 0;
            // 
            // lsvResults
            // 
            this.lsvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFilePath,
            this.colDimensions,
            this.colMipmapCount,
            this.colIsCubemap});
            this.lsvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvResults.FullRowSelect = true;
            this.lsvResults.HideSelection = false;
            this.lsvResults.Location = new System.Drawing.Point(0, 0);
            this.lsvResults.Name = "lsvResults";
            this.lsvResults.Size = new System.Drawing.Size(952, 341);
            this.lsvResults.TabIndex = 0;
            this.lsvResults.UseCompatibleStateImageBehavior = false;
            this.lsvResults.View = System.Windows.Forms.View.Details;
            this.lsvResults.DoubleClick += new System.EventHandler(this.lsvResults_Click);
            // 
            // colFilePath
            // 
            this.colFilePath.Text = "Texture Path";
            this.colFilePath.Width = 700;
            // 
            // colDimensions
            // 
            this.colDimensions.Text = "Dimensions";
            this.colDimensions.Width = 100;
            // 
            // colMipmapCount
            // 
            this.colMipmapCount.Text = "MipMaps";
            // 
            // colIsCubemap
            // 
            this.colIsCubemap.Text = "Cubemap";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 418);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "frmMain";
            this.Text = "DDS Texture Scanner v1.0 by niston";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tslStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtScanPath;
        private System.Windows.Forms.ListView lsvResults;
        private System.Windows.Forms.ColumnHeader colFilePath;
        private System.Windows.Forms.ColumnHeader colDimensions;
        private System.Windows.Forms.ColumnHeader colMipmapCount;
        private System.Windows.Forms.ColumnHeader colIsCubemap;
    }
}

