using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ControlSUR
{
    public partial class Form1 : Form
    {
        private FileInfo _inputFile;

        public Form1()
        {
            InitializeComponent();

            ReadConfig();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            using (var ofdInput = new OpenFileDialog
            {
                Filter = @"DBF File|*.dbf|CSV File|*.csv",
                AddExtension = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "dbf",
                InitialDirectory = ConfigInfo.SourcePath,
                Multiselect = false,
                ShowHelp = false,
                Title = @"Archivo de importación"
            })
            {
                if (ofdInput.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                _inputFile = new FileInfo(ofdInput.FileName);
                var data = LoadFileContents(_inputFile);

                if (data == null)
                {
                    return;
                }

                gridDataToImport.DataSource = data;
                gridDataToImport.Refresh();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (_inputFile == null)
            {
                return;
            }
        }

        private void btnGeneratePDF_Click(object sender, EventArgs e)
        {
        }

        private void btnSendEmails_Click(object sender, EventArgs e)
        {
        }

        private void ReadConfig()
        {
            ConfigInfo.SourcePath = ConfigurationManager.AppSettings["SourcePath"];
            ConfigInfo.ProcessedPath = ConfigurationManager.AppSettings["ProcessedPath"];
            ConfigInfo.ErrorPath = ConfigurationManager.AppSettings["ErrorPath"];
        }

        private DataTable LoadFileContents(FileInfo fi)
        {
            var extension = fi.Extension.Replace(".", "").ToLower();

            switch (extension)
            {
                case "dbf":
                    return LoadDBF(fi);

                case "csv":
                    return LoadCSV(fi);

                default:
                    return null;
            }
        }

        private DataTable LoadDBF(FileInfo fi)
        {
            var dt = new DataTable("toImport");

            return null;
        }

        private DataTable LoadCSV(FileInfo fi)
        {
            var dt = new DataTable("toImport");

            var sr      = new StreamReader(fi.FullName);
            var headers = sr.ReadLine().Split(',');
            
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                var rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var dr   = dt.NewRow();

                for (var i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}