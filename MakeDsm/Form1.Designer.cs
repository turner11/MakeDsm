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
            this.dsm = new System.Windows.Forms.DataGridView();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.txbPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsm)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMain.Controls.Add(this.dsm, 0, 3);
            this.tlpMain.Controls.Add(this.btnAnalyze, 0, 1);
            this.tlpMain.Controls.Add(this.txbPath, 0, 0);
            this.tlpMain.Controls.Add(this.btnBrowse, 1, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tlpMain.Size = new System.Drawing.Size(282, 260);
            this.tlpMain.TabIndex = 0;
            // 
            // dsm
            // 
            this.dsm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dsm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsm.Location = new System.Drawing.Point(3, 128);
            this.dsm.Name = "dsm";
            this.dsm.RowTemplate.Height = 24;
            this.dsm.Size = new System.Drawing.Size(240, 129);
            this.dsm.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAnalyze.Location = new System.Drawing.Point(3, 32);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(240, 23);
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
            this.txbPath.Size = new System.Drawing.Size(240, 22);
            this.txbPath.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(249, 3);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 260);
            this.Controls.Add(this.tlpMain);
            this.Name = "Form1";
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.DataGridView dsm;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.TextBox txbPath;
        private System.Windows.Forms.Button btnBrowse;
    }
}

