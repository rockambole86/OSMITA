namespace ControlSUR
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
            this.btnImport = new System.Windows.Forms.Button();
            this.btnGeneratePDF = new System.Windows.Forms.Button();
            this.btnSendEmails = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.gridDataToImport = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataToImport)).BeginInit();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(118, 12);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(100, 23);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Importar";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnGeneratePDF
            // 
            this.btnGeneratePDF.Location = new System.Drawing.Point(224, 12);
            this.btnGeneratePDF.Name = "btnGeneratePDF";
            this.btnGeneratePDF.Size = new System.Drawing.Size(100, 23);
            this.btnGeneratePDF.TabIndex = 2;
            this.btnGeneratePDF.Text = "Generar PDF";
            this.btnGeneratePDF.UseVisualStyleBackColor = true;
            this.btnGeneratePDF.Click += new System.EventHandler(this.btnGeneratePDF_Click);
            // 
            // btnSendEmails
            // 
            this.btnSendEmails.Location = new System.Drawing.Point(330, 12);
            this.btnSendEmails.Name = "btnSendEmails";
            this.btnSendEmails.Size = new System.Drawing.Size(100, 23);
            this.btnSendEmails.TabIndex = 3;
            this.btnSendEmails.Text = "Enviar mails";
            this.btnSendEmails.UseVisualStyleBackColor = true;
            this.btnSendEmails.Click += new System.EventHandler(this.btnSendEmails_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(12, 12);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(100, 23);
            this.btnLoadFile.TabIndex = 0;
            this.btnLoadFile.Text = "Cargar archivo";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // gridDataToImport
            // 
            this.gridDataToImport.AllowUserToAddRows = false;
            this.gridDataToImport.AllowUserToDeleteRows = false;
            this.gridDataToImport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridDataToImport.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridDataToImport.Location = new System.Drawing.Point(12, 162);
            this.gridDataToImport.Name = "gridDataToImport";
            this.gridDataToImport.RowHeadersVisible = false;
            this.gridDataToImport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDataToImport.Size = new System.Drawing.Size(606, 276);
            this.gridDataToImport.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 450);
            this.Controls.Add(this.gridDataToImport);
            this.Controls.Add(this.btnLoadFile);
            this.Controls.Add(this.btnSendEmails);
            this.Controls.Add(this.btnGeneratePDF);
            this.Controls.Add(this.btnImport);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gridDataToImport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnGeneratePDF;
        private System.Windows.Forms.Button btnSendEmails;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.DataGridView gridDataToImport;
    }
}

