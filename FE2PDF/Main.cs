using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SelectPdf;

namespace FE2PDF
{
    public partial class Main : Form
    {
        private FileInfo     _inputFile;
        private List<Header> _data = new List<Header>();

        private readonly DB _db = DB.Instance();

        public Main()
        {
            InitializeComponent();

            ReadConfig();

            txtOutputFolder.Text = ConfigInfo.ProcessedPath;
        }

        private void btnSearchInputFile_Click(object sender, EventArgs e)
        {
            using (var ofdInput = new OpenFileDialog
            {
                Filter = @"Text File|*.txt",
                AddExtension = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
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

                txtInputFile.Text = ofdInput.FileName;
            }
        }

        private void btnSearchOutputFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = @"Directorio de salida"
            })
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                txtOutputFolder.Text = fbd.SelectedPath;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_inputFile.FullName))
                return;

            if (!Directory.Exists(txtOutputFolder.Text))
                return;

            if (!LoadTXT())
                return;

            if (!Import())
                return;

            GeneratePDF();

            if (chkSendEmail.Checked)
            {
                SendEmails();
            }

            var newName = _inputFile.Name.Replace(_inputFile.Extension, $@"{DateTime.Now:yyyyMMddHHmmss}{_inputFile.Extension}");

            File.Move(_inputFile.FullName, Path.Combine(ConfigInfo.ProcessedPath, newName));
        }

        private static void ReadConfig()
        {
            ConfigInfo.SourcePath = ConfigurationManager.AppSettings["SourcePath"];
            ConfigInfo.ProcessedPath = ConfigurationManager.AppSettings["ProcessedPath"];
            ConfigInfo.ErrorPath = ConfigurationManager.AppSettings["ErrorPath"];
        }

        private bool LoadTXT()
        {
            _data = new List<Header>();

            try
            {
                barProgress.Style = ProgressBarStyle.Marquee;

                var    sr     = new StreamReader(_inputFile.FullName);
                Header header = null;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line.StartsWith("0"))
                    {
                        if (header != null)
                        {
                            _data.Add(header);
                        }

                        header = new Header
                        {
                            NombreComprobante = line.Substring(1, 30).Trim(),
                            TipoComprobante = line.Substring(31, 2).Trim(),
                            CondicionIVA = line.Substring(33, 1).Trim(),
                            CentroEmisor = line.Substring(34, 4).Trim(),
                            NumeroComprobante = line.Substring(38, 8).Trim(),
                            FechaEmision = line.Substring(46, 10).Trim(),
                            Detalle1 = line.Substring(56, 75).Trim(),
                            Detalle2 = line.Substring(131, 75).Trim(),
                            Domicilio = line.Substring(206, 52).Trim(),
                            Localidad = line.Substring(258, 20).Trim(),
                            Barrio = line.Substring(278, 30).Trim(),
                            CodigoPostal = line.Substring(308, 8).Trim(),
                            CUILCUIT = line.Substring(316, 13).Trim().Replace("/", "-"),
                            FechaVencimiento = line.Substring(329, 10).Trim(),
                            NumeroCAE = line.Substring(339, 14).Trim(),
                            CondicionPago = line.Substring(353, 70).Trim(),
                            Importe = line.Substring(423, 12).Trim(),
                            MontoIVA = line.Substring(435, 12).Trim(),
                            MontoGravado = line.Substring(447, 12).Trim(),
                            MontoNoGravado = line.Substring(459, 12).Trim(),
                            Subtotal = line.Substring(471, 12).Trim(),
                            CodigoBarra = line.Substring(483, 50).Trim(),
                            RefPagoMisCuentas = line.Substring(533, 19).Trim(),
                            RefRedLink = line.Substring(552, 19).Trim(),
                            Email = line.Substring(571, 50).Trim()
                        };
                    }
                    else if (line.StartsWith("1"))
                    {
                        if (header == null)
                            continue;

                        var item = new Detail
                        {
                            Detalle = line.Substring(1, 50).Trim(),
                            MontoFacturado = line.Substring(51, 12).Trim()
                        };

                        header.Items.Add(item);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
            finally
            {
                barProgress.Style = ProgressBarStyle.Continuous;
            }
        }

        private bool Import()
        {
            var query = string.Empty;

            try
            {
                barProgress.Style = ProgressBarStyle.Continuous;
                barProgress.Maximum = _data.Count;

                _db.Connect();
                _db.BeginTran();

                var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                query = $@"INSERT INTO ImportedFile(Name, TimeStamp, Records) VALUES('{_inputFile.Name}','{timeStamp}',{_data.Count});";

                _db.ExecuteNonQuery(query);

                query = $@"SELECT Id FROM ImportedFile WHERE TimeStamp = '{timeStamp}';";

                var importedFileId = _db.ExecuteScalar(query);

                foreach (var header in _data)
                {
                    var importe        = Convert.ToDouble(header.Importe.Replace(".", "")) / 100;
                    var montoIVA       = Convert.ToDouble(header.MontoIVA.Replace(".", "")) / 100;
                    var montoGravado   = Convert.ToDouble(header.MontoGravado.Replace(".", "")) / 100;
                    var montoNoGravado = Convert.ToDouble(header.MontoNoGravado.Replace(".", "")) / 100;
                    var subtotal       = Convert.ToDouble(header.Subtotal.Replace(".", "")) / 100;

                    query = $@"INSERT INTO Header(ImportedFileId, NombreComprobante, TipoComprobante, CondicionIVA, CentroEmisor, NumeroComprobante, FechaEmision, Detalle1, Detalle2, Domicilio, Localidad, Barrio, CodigoPostal, CUILCUIT, FechaVencimiento, NumeroCAE, CondicionPago, Importe, MontoIVA, MontoGravado, MontoNoGravado, Subtotal, CodigoBarra, RefPagoMisCuentas, RefRedLink, Email) ";
                    query += $@"VALUES(@ImportedFileId, @NombreComprobante, @TipoComprobante, @CondicionIVA, @CentroEmisor, @NumeroComprobante, @FechaEmision, @Detalle1, @Detalle2, @Domicilio, @Localidad, @Barrio, @CodigoPostal, @CUILCUIT, @FechaVencimiento, @NumeroCAE, @CondicionPago, @Importe, @MontoIVA, @MontoGravado, @MontoNoGravado, @Subtotal, @CodigoBarra, @RefPagoMisCuentas, @RefRedLink, @Email);";

                    var parameters = new Hashtable
                    {
                        { "@ImportedFileId", importedFileId },
                        { "@NombreComprobante", header.NombreComprobante },
                        { "@TipoComprobante", header.TipoComprobante },
                        { "@CondicionIVA", header.CondicionIVA },
                        { "@CentroEmisor", header.CentroEmisor },
                        { "@NumeroComprobante", header.NumeroComprobante },
                        { "@FechaEmision", header.FechaEmision },
                        { "@Detalle1", header.Detalle1 },
                        { "@Detalle2", header.Detalle2 },
                        { "@Domicilio", header.Domicilio },
                        { "@Localidad", header.Localidad },
                        { "@Barrio", header.Barrio },
                        { "@CodigoPostal", header.CodigoPostal },
                        { "@CUILCUIT", header.CUILCUIT },
                        { "@FechaVencimiento", header.FechaVencimiento },
                        { "@NumeroCAE", header.NumeroCAE },
                        { "@CondicionPago", header.CondicionPago },
                        { "@Importe", importe },
                        { "@MontoIVA", montoIVA },
                        { "@MontoGravado", montoGravado },
                        { "@MontoNoGravado", montoNoGravado },
                        { "@Subtotal", subtotal },
                        { "@CodigoBarra", header.CodigoBarra },
                        { "@RefPagoMisCuentas", header.RefPagoMisCuentas },
                        { "@RefRedLink", header.RefRedLink },
                        { "@Email", header.Email }
                    };

                    _db.ExecuteNonQuery(query, parameters);

                    query = $@"SELECT MAX(Id) FROM Header;";

                    var headerId = _db.ExecuteScalar(query);

                    foreach (var detail in header.Items)
                    {
                        var monto = Convert.ToDouble(detail.MontoFacturado.Replace(".", "")) / 100;

                        query = $@"INSERT INTO Detail(HeaderId, Detalle, MontoFacturado) VALUES({headerId},'{detail.Detalle}',{monto});";

                        _db.ExecuteNonQuery(query);
                    }

                    barProgress.PerformStep();
                }

                _db.CommitTran();

                return true;
            }
            catch (Exception ex)
            {
                _db.RollbackTran();

                MessageBox.Show(ex.Message);

                return false;
            }
            finally
            {
                _db.Disconnect();

                barProgress.Value = 0;
            }
        }

        private void GeneratePDF()
        {
            var template = File.ReadAllText(Path.Combine(Application.StartupPath, $@"template.html"));

            var start = template.IndexOf(@"<table class=""details"">", 0, StringComparison.OrdinalIgnoreCase) + @"<table class=""details"">".Length;
            var end   = template.IndexOf(@"</table>", start, StringComparison.OrdinalIgnoreCase);
            var templateDetail = template.Substring(start, end - start);
            
            var converter = new HtmlToPdf
            {
                Options =
                {
                    PdfPageSize = PdfPageSize.A4,
                    PdfPageOrientation = PdfPageOrientation.Portrait,
                    WebPageWidth = 210,
                    WebPageHeight = 297,
                }
            };

            barProgress.Style = ProgressBarStyle.Continuous;
            barProgress.Maximum = _data.Count;

            foreach (var header in _data)
            {
                var html = template;

                var properties = typeof(Header).GetProperties();

                html = properties.Aggregate(html, (current, property) => current.Replace($"{{{{{property.Name}}}}}", property.GetValue(header, null).ToString()));

                var htmlDetail = string.Empty;

                foreach (var detail in header.Items)
                {
                    htmlDetail += templateDetail;

                    properties = typeof(Detail).GetProperties();

                    htmlDetail = properties.Aggregate(htmlDetail, (current, property) => current.Replace($"{{{{{property.Name}}}}}", property.GetValue(detail, null).ToString()));
                }

                html = html.Replace(templateDetail, htmlDetail);

                //Generate PDF file
                var doc = converter.ConvertHtmlString(html);

                var pdfName = $@"{header.TipoComprobante}{header.CondicionIVA}{header.CentroEmisor}{header.NumeroComprobante}.pdf";
                var outputFile = Path.Combine(txtOutputFolder.Text, pdfName); 

                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                doc.Save(outputFile);
                doc.Close();

                barProgress.PerformStep();
            }
        }

        private void SendEmails()
        {
        }
    }
}