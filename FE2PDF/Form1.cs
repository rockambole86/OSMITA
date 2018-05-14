using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FE2PDF
{
    public partial class Form1 : Form
    {
        private FileInfo _inputFile;
        private DB _db = DB.Instance();

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

            var query = string.Empty;

            query = $@"SELECT COUNT(*) FROM ImportedFile WHERE Name = '{_inputFile.Name}'";

            _db.Connect();
            var alreadyImported = Convert.ToInt32(_db.ExecuteScalar(query)) > 0;
            _db.Disconnect();

            if (alreadyImported)
            {
                MessageBox.Show(@"Archivo ya importado");
                return;
            }

            var data = gridDataToImport.DataSource as DataTable;

            try
            {
                _db.Connect();
                _db.BeginTran();

                //TODO: Import records in table

                query = $@"INSERT INTO ImportedFile(Name, TimeStamp, Records, Type) VALUES('{_inputFile.Name}',{DateTime.Now.Ticks},{data.Rows.Count},'{_inputFile.Extension.Replace(".", "")}');";

                _db.ExecuteNonQuery(query);

                _db.CommitTran();
            }
            catch(Exception ex)
            {
                _db.RollbackTran();
            }
            finally
            {
                _db.Disconnect();
            }
        }

        private void btnGeneratePDF_Click(object sender, EventArgs e)
        {
        }

        private void btnSendEmails_Click(object sender, EventArgs e)
        {
        }

        private static void ReadConfig()
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

            Console.WriteLine(fi.FullName);

            return null;
        }

        private DataTable LoadCSV(FileInfo fi)
        {
            var dt = new DataTable("toImport");

            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return null;
            }

            return dt;
        }
    }
}