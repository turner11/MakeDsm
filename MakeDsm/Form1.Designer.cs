namespace MakeDsm
{
    partial class Form1
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.txbPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tcDisplays = new System.Windows.Forms.TabControl();
            this.tpDSM = new System.Windows.Forms.TabPage();
            this.dsm = new System.Windows.Forms.DataGridView();
            this.tpModularity = new System.Windows.Forms.TabPage();
            this.gvModularity = new System.Windows.Forms.DataGridView();
            this.tlpMain.SuspendLayout();
            this.tcDisplays.SuspendLayout();
            this.tpDSM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsm)).BeginInit();
            this.tpModularity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvModularity)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMain.Controls.Add(this.btnAnalyze, 0, 1);
            this.tlpMain.Controls.Add(this.txbPath, 0, 0);
            this.tlpMain.Controls.Add(this.btnBrowse, 1, 0);
            this.tlpMain.Controls.Add(this.tcDisplays, 0, 3);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(478, 434);
            this.tlpMain.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAnalyze.Location = new System.Drawing.Point(3, 32);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(434, 23);
            this.btnAnalyze.TabIndex = 1;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // txbPath
            // 
            this.txbPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbPath.Location = new System.Drawing.Point(3, 3);
            this.txbPath.Name = "txbPath";
            this.txbPath.Size = new System.Drawing.Size(434, 22);
            this.txbPath.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(443, 3);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tcDisplays
            // 
            this.tcDisplays.Controls.Add(this.tpDSM);
            this.tcDisplays.Controls.Add(this.tpModularity);
            this.tcDisplays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDisplays.Location = new System.Drawing.Point(3, 71);
            this.tcDisplays.Name = "tcDisplays";
            this.tcDisplays.SelectedIndex = 0;
            this.tcDisplays.Size = new System.Drawing.Size(434, 360);
            this.tcDisplays.TabIndex = 4;
            // 
            // tpDSM
            // 
            this.tpDSM.Controls.Add(this.dsm);
            this.tpDSM.Location = new System.Drawing.Point(4, 25);
            this.tpDSM.Name = "tpDSM";
            this.tpDSM.Padding = new System.Windows.Forms.Padding(3);
            this.tpDSM.Size = new System.Drawing.Size(426, 331);
            this.tpDSM.TabIndex = 0;
            this.tpDSM.Text = "DSM";
            this.tpDSM.UseVisualStyleBackColor = true;
            // 
            // dsm
            // 
            this.dsm.AllowUserToAddRows = false;
            this.dsm.AllowUserToDeleteRows = false;
            this.dsm.AllowUserToResizeColumns = false;
            this.dsm.AllowUserToResizeRows = false;
            this.dsm.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dsm.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dsm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dsm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsm.Location = new System.Drawing.Point(3, 3);
            this.dsm.Name = "dsm";
            this.dsm.RowTemplate.Height = 24;
            this.dsm.Size = new System.Drawing.Size(420, 325);
            this.dsm.TabIndex = 0;
            this.dsm.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dsm_CellClick);
            this.dsm.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dsm_DataBindingComplete);
            // 
            // tpModularity
            // 
            this.tpModularity.Controls.Add(this.gvModularity);
            this.tpModularity.Location = new System.Drawing.Point(4, 25);
            this.tpModularity.Name = "tpModularity";
            this.tpModularity.Padding = new System.Windows.Forms.Padding(3);
            this.tpModularity.Size = new System.Drawing.Size(426, 331);
            this.tpModularity.TabIndex = 1;
            this.tpModularity.Text = "Modularity Matrix";
            this.tpModularity.UseVisualStyleBackColor = true;
            // 
            // gvModularity
            // 
            this.gvModularity.AllowUserToAddRows = false;
            this.gvModularity.AllowUserToDeleteRows = false;
            this.gvModularity.AllowUserToResizeColumns = false;
            this.gvModularity.AllowUserToResizeRows = false;
            this.gvModularity.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvModularity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvModularity.Location = new System.Drawing.Point(3, 3);
            this.gvModularity.Name = "gvModularity";
            this.gvModularity.RowTemplate.Height = 24;
            this.gvModularity.Size = new System.Drawing.Size(420, 325);
            this.gvModularity.TabIndex = 0;
            this.gvModularity.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gvModularity_DataBindingComplete);
            // 
            // Form1
            // 
            this.AcceptButton = this.btnAnalyze;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 434);
            this.Controls.Add(this.tlpMain);
            this.Name = "Form1";
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tcDisplays.ResumeLayout(false);
            this.tpDSM.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsm)).EndInit();
            this.tpModularity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvModularity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.DataGridView dsm;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.TextBox txbPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TabControl tcDisplays;
        private System.Windows.Forms.TabPage tpDSM;
        private System.Windows.Forms.TabPage tpModularity;
        private System.Windows.Forms.DataGridView gvModularity;
    }
}

