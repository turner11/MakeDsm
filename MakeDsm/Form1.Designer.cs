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
            this.btnBrows = new System.Windows.Forms.TableLayoutPanel();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tcDisplays = new System.Windows.Forms.TabControl();
            this.tpDSM = new System.Windows.Forms.TabPage();
            this.dsm = new System.Windows.Forms.DataGridView();
            this.tpModularity = new System.Windows.Forms.TabPage();
            this.tlpModularity = new System.Windows.Forms.TableLayoutPanel();
            this.gvModularity = new System.Windows.Forms.DataGridView();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.lblIdx = new System.Windows.Forms.Label();
            this.txbPath = new System.Windows.Forms.TextBox();
            this.btnBrows.SuspendLayout();
            this.tcDisplays.SuspendLayout();
            this.tpDSM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsm)).BeginInit();
            this.tpModularity.SuspendLayout();
            this.tlpModularity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvModularity)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrows
            // 
            this.btnBrows.ColumnCount = 2;
            this.btnBrows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.btnBrows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.btnBrows.Controls.Add(this.btnAnalyze, 0, 1);
            this.btnBrows.Controls.Add(this.txbPath, 0, 0);
            this.btnBrows.Controls.Add(this.btnBrowse, 1, 0);
            this.btnBrows.Controls.Add(this.tcDisplays, 0, 3);
            this.btnBrows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrows.Location = new System.Drawing.Point(0, 0);
            this.btnBrows.Name = "btnBrows";
            this.btnBrows.RowCount = 4;
            this.btnBrows.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.btnBrows.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.btnBrows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.btnBrows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.btnBrows.Size = new System.Drawing.Size(671, 472);
            this.btnBrows.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAnalyze.Location = new System.Drawing.Point(3, 32);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(627, 23);
            this.btnAnalyze.TabIndex = 1;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(636, 3);
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
            this.tcDisplays.Size = new System.Drawing.Size(627, 398);
            this.tcDisplays.TabIndex = 4;
            this.tcDisplays.SelectedIndexChanged += new System.EventHandler(this.tcDisplays_SelectedIndexChanged);
            // 
            // tpDSM
            // 
            this.tpDSM.Controls.Add(this.dsm);
            this.tpDSM.Location = new System.Drawing.Point(4, 25);
            this.tpDSM.Name = "tpDSM";
            this.tpDSM.Padding = new System.Windows.Forms.Padding(3);
            this.tpDSM.Size = new System.Drawing.Size(619, 369);
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
            this.dsm.Size = new System.Drawing.Size(613, 363);
            this.dsm.TabIndex = 0;
            this.dsm.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dsm_CellClick);
            this.dsm.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dsm_DataBindingComplete);
            // 
            // tpModularity
            // 
            this.tpModularity.Controls.Add(this.tlpModularity);
            this.tpModularity.Location = new System.Drawing.Point(4, 25);
            this.tpModularity.Name = "tpModularity";
            this.tpModularity.Padding = new System.Windows.Forms.Padding(3);
            this.tpModularity.Size = new System.Drawing.Size(619, 369);
            this.tpModularity.TabIndex = 1;
            this.tpModularity.Text = "Modularity Matrix";
            this.tpModularity.UseVisualStyleBackColor = true;
            // 
            // tlpModularity
            // 
            this.tlpModularity.ColumnCount = 2;
            this.tlpModularity.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpModularity.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpModularity.Controls.Add(this.gvModularity, 0, 0);
            this.tlpModularity.Controls.Add(this.pnlButtons, 1, 0);
            this.tlpModularity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpModularity.Location = new System.Drawing.Point(3, 3);
            this.tlpModularity.Name = "tlpModularity";
            this.tlpModularity.RowCount = 1;
            this.tlpModularity.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpModularity.Size = new System.Drawing.Size(613, 363);
            this.tlpModularity.TabIndex = 1;
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
            this.gvModularity.Size = new System.Drawing.Size(462, 357);
            this.gvModularity.TabIndex = 0;
            this.gvModularity.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gvModularity_DataBindingComplete);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.lblIdx);
            this.pnlButtons.Controls.Add(this.btnLeft);
            this.pnlButtons.Controls.Add(this.btnRight);
            this.pnlButtons.Controls.Add(this.btnDown);
            this.pnlButtons.Controls.Add(this.btnUp);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.Location = new System.Drawing.Point(471, 3);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(139, 357);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnLeft
            // 
            this.btnLeft.BackgroundImage = global::MakeDsm.Properties.Resources.Arrow_Left_1;
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLeft.Location = new System.Drawing.Point(3, 69);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(64, 45);
            this.btnLeft.TabIndex = 6;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.BackgroundImage = global::MakeDsm.Properties.Resources.Arrow_Right_1;
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRight.Location = new System.Drawing.Point(72, 69);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(64, 45);
            this.btnRight.TabIndex = 5;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackgroundImage = global::MakeDsm.Properties.Resources.Arrow_Down_1;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDown.Location = new System.Drawing.Point(46, 120);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 59);
            this.btnDown.TabIndex = 2;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackgroundImage = global::MakeDsm.Properties.Resources.Arrow_Up_1;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUp.Location = new System.Drawing.Point(46, 5);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 59);
            this.btnUp.TabIndex = 2;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // lblIdx
            // 
            this.lblIdx.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblIdx.AutoSize = true;
            this.lblIdx.Location = new System.Drawing.Point(49, 200);
            this.lblIdx.Name = "lblIdx";
            this.lblIdx.Size = new System.Drawing.Size(41, 17);
            this.lblIdx.TabIndex = 7;
            this.lblIdx.Text = "Index";
            // 
            // txbPath
            // 
            this.txbPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbPath.Location = new System.Drawing.Point(3, 3);
            this.txbPath.Name = "txbPath";
            this.txbPath.Size = new System.Drawing.Size(627, 22);
            this.txbPath.TabIndex = 2;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnAnalyze;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 472);
            this.Controls.Add(this.btnBrows);
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.btnBrows.ResumeLayout(false);
            this.btnBrows.PerformLayout();
            this.tcDisplays.ResumeLayout(false);
            this.tpDSM.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsm)).EndInit();
            this.tpModularity.ResumeLayout(false);
            this.tlpModularity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvModularity)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel btnBrows;
        private System.Windows.Forms.DataGridView dsm;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TabControl tcDisplays;
        private System.Windows.Forms.TabPage tpDSM;
        private System.Windows.Forms.TabPage tpModularity;
        private System.Windows.Forms.DataGridView gvModularity;
        private System.Windows.Forms.TableLayoutPanel tlpModularity;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Label lblIdx;
        private System.Windows.Forms.TextBox txbPath;
    }
}

