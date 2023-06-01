
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAlwaysListGeneralBA2 = new System.Windows.Forms.CheckBox();
            this.chkListZeroMipmaps = new System.Windows.Forms.CheckBox();
            this.chkProcessBA2 = new System.Windows.Forms.CheckBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtScanPath = new System.Windows.Forms.TextBox();
            this.lsvResults = new System.Windows.Forms.ListView();
            this.colFilePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDimensions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMipmapCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colIsCubemap = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colArchive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblPleaseWait = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lnkAbout = new System.Windows.Forms.LinkLabel();
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
            this.statusStrip1.Size = new System.Drawing.Size(1047, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslStatus
            // 
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Size = new System.Drawing.Size(1032, 17);
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
            this.splitContainer1.Panel2.Controls.Add(this.lblPleaseWait);
            this.splitContainer1.Size = new System.Drawing.Size(1047, 396);
            this.splitContainer1.SplitterDistance = 71;
            this.splitContainer1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lnkAbout);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkAlwaysListGeneralBA2);
            this.groupBox1.Controls.Add(this.chkListZeroMipmaps);
            this.groupBox1.Controls.Add(this.chkProcessBA2);
            this.groupBox1.Controls.Add(this.btnGo);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtScanPath);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1047, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scanner Control";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Scan Path";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkAlwaysListGeneralBA2
            // 
            this.chkAlwaysListGeneralBA2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAlwaysListGeneralBA2.AutoSize = true;
            this.chkAlwaysListGeneralBA2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAlwaysListGeneralBA2.Location = new System.Drawing.Point(720, 46);
            this.chkAlwaysListGeneralBA2.Name = "chkAlwaysListGeneralBA2";
            this.chkAlwaysListGeneralBA2.Size = new System.Drawing.Size(169, 17);
            this.chkAlwaysListGeneralBA2.TabIndex = 5;
            this.chkAlwaysListGeneralBA2.Text = "Always list files in General BA2";
            this.toolTip1.SetToolTip(this.chkAlwaysListGeneralBA2, "Always list textures contained in General type BA2 archives.");
            this.chkAlwaysListGeneralBA2.UseVisualStyleBackColor = true;
            // 
            // chkListZeroMipmaps
            // 
            this.chkListZeroMipmaps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkListZeroMipmaps.AutoSize = true;
            this.chkListZeroMipmaps.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkListZeroMipmaps.Location = new System.Drawing.Point(545, 46);
            this.chkListZeroMipmaps.Name = "chkListZeroMipmaps";
            this.chkListZeroMipmaps.Size = new System.Drawing.Size(159, 17);
            this.chkListZeroMipmaps.TabIndex = 4;
            this.chkListZeroMipmaps.Text = "Always list zero MipMap files";
            this.toolTip1.SetToolTip(this.chkListZeroMipmaps, "Always list textures that don\'t have any MipMaps.");
            this.chkListZeroMipmaps.UseVisualStyleBackColor = true;
            // 
            // chkProcessBA2
            // 
            this.chkProcessBA2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProcessBA2.AutoSize = true;
            this.chkProcessBA2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkProcessBA2.Location = new System.Drawing.Point(904, 46);
            this.chkProcessBA2.Name = "chkProcessBA2";
            this.chkProcessBA2.Size = new System.Drawing.Size(131, 17);
            this.chkProcessBA2.TabIndex = 3;
            this.chkProcessBA2.Text = "Process BA2 Archives";
            this.toolTip1.SetToolTip(this.chkProcessBA2, "Process DDS textures inside any BA2 archives discovered in scanning path.");
            this.chkProcessBA2.UseVisualStyleBackColor = true;
            this.chkProcessBA2.CheckedChanged += new System.EventHandler(this.chkProcessBA2_CheckedChanged);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.Location = new System.Drawing.Point(955, 20);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(80, 20);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "GO";
            this.toolTip1.SetToolTip(this.btnGo, "Start/Stop scanning.");
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(921, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 20);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.toolTip1.SetToolTip(this.btnBrowse, "Open \"Browse For Folder\" dialog to pick the scanning path root folder.");
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtScanPath
            // 
            this.txtScanPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScanPath.Location = new System.Drawing.Point(76, 20);
            this.txtScanPath.Name = "txtScanPath";
            this.txtScanPath.Size = new System.Drawing.Size(839, 20);
            this.txtScanPath.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtScanPath, "The scanning path. Includes all nested subfolders.");
            // 
            // lsvResults
            // 
            this.lsvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFilePath,
            this.colDimensions,
            this.colMipmapCount,
            this.colIsCubemap,
            this.colArchive});
            this.lsvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvResults.FullRowSelect = true;
            this.lsvResults.HideSelection = false;
            this.lsvResults.Location = new System.Drawing.Point(0, 0);
            this.lsvResults.Name = "lsvResults";
            this.lsvResults.ShowItemToolTips = true;
            this.lsvResults.Size = new System.Drawing.Size(1047, 321);
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
            // colArchive
            // 
            this.colArchive.Text = "Archive Type";
            this.colArchive.Width = 100;
            // 
            // lblPleaseWait
            // 
            this.lblPleaseWait.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPleaseWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPleaseWait.Location = new System.Drawing.Point(0, 0);
            this.lblPleaseWait.Name = "lblPleaseWait";
            this.lblPleaseWait.Size = new System.Drawing.Size(1047, 321);
            this.lblPleaseWait.TabIndex = 1;
            this.lblPleaseWait.Text = "Working; Please Stand By...";
            this.lblPleaseWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkAbout
            // 
            this.lnkAbout.AutoSize = true;
            this.lnkAbout.Location = new System.Drawing.Point(73, 47);
            this.lnkAbout.Name = "lnkAbout";
            this.lnkAbout.Size = new System.Drawing.Size(147, 13);
            this.lnkAbout.TabIndex = 7;
            this.lnkAbout.TabStop = true;
            this.lnkAbout.Text = "would you like to know more?";
            this.lnkAbout.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lnkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAbout_LinkClicked);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 418);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "DDS Texture Scanner v1.3 by niston";
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
        private System.Windows.Forms.CheckBox chkProcessBA2;
        private System.Windows.Forms.ColumnHeader colArchive;
        private System.Windows.Forms.CheckBox chkListZeroMipmaps;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblPleaseWait;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkAlwaysListGeneralBA2;
        private System.Windows.Forms.LinkLabel lnkAbout;
    }
}

