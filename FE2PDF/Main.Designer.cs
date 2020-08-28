namespace FE2PDF
{
    partial class Main
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
            this.lblInputFile = new System.Windows.Forms.Label();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.chkSendEmail = new System.Windows.Forms.CheckBox();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnSearchInputFile = new System.Windows.Forms.Button();
            this.btnSearchOutputFolder = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.barProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.chkMethodReport = new System.Windows.Forms.RadioButton();
            this.chkMethodHTML = new System.Windows.Forms.RadioButton();
            this.btnGenerateCAE = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInputFile
            // 
            this.lblInputFile.AutoSize = true;
            this.lblInputFile.Location = new System.Drawing.Point(53, 21);
            this.lblInputFile.Name = "lblInputFile";
            this.lblInputFile.Size = new System.Drawing.Size(93, 13);
            this.lblInputFile.TabIndex = 5;
            this.lblInputFile.Text = "Archivo de origen:";
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(62, 47);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(84, 13);
            this.lblOutputFolder.TabIndex = 6;
            this.lblOutputFolder.Text = "Carpeta destino:";
            // 
            // chkSendEmail
            // 
            this.chkSendEmail.AutoSize = true;
            this.chkSendEmail.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSendEmail.Checked = true;
            this.chkSendEmail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSendEmail.Location = new System.Drawing.Point(16, 70);
            this.chkSendEmail.Name = "chkSendEmail";
            this.chkSendEmail.Size = new System.Drawing.Size(150, 17);
            this.chkSendEmail.TabIndex = 7;
            this.chkSendEmail.Text = "¿Enviar archivos por mail?";
            this.chkSendEmail.UseVisualStyleBackColor = true;
            // 
            // txtInputFile
            // 
            this.txtInputFile.Location = new System.Drawing.Point(152, 18);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(343, 20);
            this.txtInputFile.TabIndex = 8;
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(152, 44);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(343, 20);
            this.txtOutputFolder.TabIndex = 9;
            // 
            // btnSearchInputFile
            // 
            this.btnSearchInputFile.Location = new System.Drawing.Point(501, 18);
            this.btnSearchInputFile.Name = "btnSearchInputFile";
            this.btnSearchInputFile.Size = new System.Drawing.Size(28, 20);
            this.btnSearchInputFile.TabIndex = 10;
            this.btnSearchInputFile.Text = "...";
            this.btnSearchInputFile.UseVisualStyleBackColor = true;
            this.btnSearchInputFile.Click += new System.EventHandler(this.btnSearchInputFile_Click);
            // 
            // btnSearchOutputFolder
            // 
            this.btnSearchOutputFolder.Location = new System.Drawing.Point(501, 44);
            this.btnSearchOutputFolder.Name = "btnSearchOutputFolder";
            this.btnSearchOutputFolder.Size = new System.Drawing.Size(28, 20);
            this.btnSearchOutputFolder.TabIndex = 11;
            this.btnSearchOutputFolder.Text = "...";
            this.btnSearchOutputFolder.UseVisualStyleBackColor = true;
            this.btnSearchOutputFolder.Click += new System.EventHandler(this.btnSearchOutputFolder_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcess.Location = new System.Drawing.Point(28, 120);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(501, 40);
            this.btnProcess.TabIndex = 12;
            this.btnProcess.Text = "Procesar";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.barProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 228);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(545, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(150, 17);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // barProgress
            // 
            this.barProgress.Name = "barProgress";
            this.barProgress.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.barProgress.Size = new System.Drawing.Size(392, 16);
            this.barProgress.Step = 1;
            // 
            // chkMethodReport
            // 
            this.chkMethodReport.AutoSize = true;
            this.chkMethodReport.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkMethodReport.Checked = true;
            this.chkMethodReport.Location = new System.Drawing.Point(401, 69);
            this.chkMethodReport.Name = "chkMethodReport";
            this.chkMethodReport.Size = new System.Drawing.Size(94, 17);
            this.chkMethodReport.TabIndex = 14;
            this.chkMethodReport.TabStop = true;
            this.chkMethodReport.Text = "Método nuevo";
            this.chkMethodReport.UseVisualStyleBackColor = true;
            this.chkMethodReport.Visible = false;
            // 
            // chkMethodHTML
            // 
            this.chkMethodHTML.AutoSize = true;
            this.chkMethodHTML.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkMethodHTML.Location = new System.Drawing.Point(409, 92);
            this.chkMethodHTML.Name = "chkMethodHTML";
            this.chkMethodHTML.Size = new System.Drawing.Size(86, 17);
            this.chkMethodHTML.TabIndex = 15;
            this.chkMethodHTML.Text = "Método viejo";
            this.chkMethodHTML.UseVisualStyleBackColor = true;
            this.chkMethodHTML.Visible = false;
            // 
            // btnGenerateCAE
            // 
            this.btnGenerateCAE.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerateCAE.Location = new System.Drawing.Point(28, 166);
            this.btnGenerateCAE.Name = "btnGenerateCAE";
            this.btnGenerateCAE.Size = new System.Drawing.Size(501, 40);
            this.btnGenerateCAE.TabIndex = 16;
            this.btnGenerateCAE.Text = "Generar CAE";
            this.btnGenerateCAE.UseVisualStyleBackColor = true;
            this.btnGenerateCAE.Click += new System.EventHandler(this.btnGenerateCAE_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 250);
            this.Controls.Add(this.btnGenerateCAE);
            this.Controls.Add(this.chkMethodHTML);
            this.Controls.Add(this.chkMethodReport);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnSearchOutputFolder);
            this.Controls.Add(this.btnSearchInputFile);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.txtInputFile);
            this.Controls.Add(this.chkSendEmail);
            this.Controls.Add(this.lblOutputFolder);
            this.Controls.Add(this.lblInputFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FE2PDF";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblInputFile;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.CheckBox chkSendEmail;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnSearchInputFile;
        private System.Windows.Forms.Button btnSearchOutputFolder;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar barProgress;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.RadioButton chkMethodReport;
        private System.Windows.Forms.RadioButton chkMethodHTML;
        private System.Windows.Forms.Button btnGenerateCAE;
    }
}

