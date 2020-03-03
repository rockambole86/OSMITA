using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FE2PDF.AFIP;
using FE2PDF.Afip.Wsfe;
using fyiReporting.RdlViewer;
using fyiReporting.RDL;
using SelectPdf;
using HtmlToPdf = SelectPdf.HtmlToPdf;

namespace FE2PDF
{
    public partial class Main : Form
    {
        private FileInfo     _inputFile;
        private List<Header> _data     = new List<Header>();
        private DataAfip     _dataAfip = new DataAfip();

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
                Filter           = @"Text File|*.txt",
                AddExtension     = false,
                CheckFileExists  = true,
                CheckPathExists  = true,
                DefaultExt       = "txt",
                InitialDirectory = ConfigInfo.SourcePath,
                Multiselect      = false,
                ShowHelp         = false,
                Title            = @"Archivo de importación"
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
            using (var fbd = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description         = @"Directorio de salida"
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
            try
            {
                txtInputFile.ReadOnly         = true;
                txtOutputFolder.ReadOnly      = true;
                chkSendEmail.Enabled          = false;
                btnSearchInputFile.Enabled    = false;
                btnSearchOutputFolder.Enabled = false;
                btnProcess.Enabled            = false;
                btnGenerateCAE.Enabled        = false;

                if (!File.Exists(_inputFile.FullName))
                    throw new FileNotFoundException($@"El archivo '{_inputFile.FullName}' no existe.");

                if (!Directory.Exists(txtOutputFolder.Text))
                    throw new DirectoryNotFoundException($@"El directorio '{txtOutputFolder.Text}' no existe.");

                if (!LoadTXT())
                    return;

                if (!Import())
                    return;

                if (chkMethodReport.Checked)
                    GeneratePDF2();
                else
                    GeneratePDF();

                var emailDeliveryErrors = false;

                if (chkSendEmail.Checked)
                {
                    emailDeliveryErrors = SendEmails();
                }

                var newName = _inputFile.Name.Replace(_inputFile.Extension, $@"{DateTime.Now:yyyyMMddHHmmss}{_inputFile.Extension}");

                File.Move(_inputFile.FullName, Path.Combine(txtOutputFolder.Text, newName));

                txtInputFile.Text    = string.Empty;
                chkSendEmail.Checked = true;

                MessageBox.Show(!emailDeliveryErrors
                                    ? @"Proceso finalizado correctamente"
                                    : @"Hubo errores al enviar emails, por favor revise el log");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                lblStatus.Text      = string.Empty;
                barProgress.Style   = ProgressBarStyle.Continuous;
                barProgress.Minimum = 0;
                barProgress.Maximum = 0;
                barProgress.Value   = 0;

                txtInputFile.ReadOnly         = false;
                txtOutputFolder.ReadOnly      = false;
                chkSendEmail.Enabled          = true;
                btnSearchInputFile.Enabled    = true;
                btnSearchOutputFolder.Enabled = true;
                btnProcess.Enabled            = true;
                btnGenerateCAE.Enabled        = true;
            }
        }

        private void btnGenerateCAE_Click(object sender, EventArgs e)
        {
            try
            {
                txtInputFile.ReadOnly         = true;
                txtOutputFolder.ReadOnly      = true;
                chkSendEmail.Enabled          = false;
                btnSearchInputFile.Enabled    = false;
                btnSearchOutputFolder.Enabled = false;
                btnProcess.Enabled            = false;
                btnGenerateCAE.Enabled        = false;

                if (!File.Exists(_inputFile.FullName))
                    throw new FileNotFoundException($@"El archivo '{_inputFile.FullName}' no existe.");

                if (!Directory.Exists(txtOutputFolder.Text))
                    throw new DirectoryNotFoundException($@"El directorio '{txtOutputFolder.Text}' no existe.");

                if (!LoadTxtAfip())
                    return;

                if (!ImportAfip())
                    return;

                if (!GetCae())
                    return;

                //if (chkMethodReport.Checked)
                //    GeneratePDF2();
                //else
                //    GeneratePDF();

                //var emailDeliveryErrors = false;

                //if (chkSendEmail.Checked)
                //{
                //    emailDeliveryErrors = SendEmails();
                //}

                var newName = _inputFile.Name.Replace(_inputFile.Extension, $@"{DateTime.Now:yyyyMMddHHmmss}{_inputFile.Extension}");

                File.Move(_inputFile.FullName, Path.Combine(txtOutputFolder.Text, newName));

                //txtInputFile.Text    = string.Empty;
                //chkSendEmail.Checked = true;

                //MessageBox.Show(!emailDeliveryErrors
                //                    ? $@"Proceso finalizado correctamente"
                //                    : $@"Hubo errores al enviar emails, por favor revise el log");

                MessageBox.Show(@"Proceso finalizado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                lblStatus.Text      = string.Empty;
                barProgress.Style   = ProgressBarStyle.Continuous;
                barProgress.Minimum = 0;
                barProgress.Maximum = 0;
                barProgress.Value   = 0;

                txtInputFile.ReadOnly         = false;
                txtOutputFolder.ReadOnly      = false;
                chkSendEmail.Enabled          = true;
                btnSearchInputFile.Enabled    = true;
                btnSearchOutputFolder.Enabled = true;
                btnProcess.Enabled            = true;
                btnGenerateCAE.Enabled        = true;
            }
        }

        private static void ReadConfig()
        {
            ConfigInfo.SourcePath    = ConfigurationManager.AppSettings["SourcePath"];
            ConfigInfo.ProcessedPath = ConfigurationManager.AppSettings["ProcessedPath"];
            ConfigInfo.ErrorPath     = ConfigurationManager.AppSettings["ErrorPath"];
            ConfigInfo.BatchSize     = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSize"]);
            ConfigInfo.SaveHtml      = Convert.ToBoolean(ConfigurationManager.AppSettings["SaveHtml"]);

            ConfigInfo.SMTPServer   = ConfigurationManager.AppSettings["mail_smtp_server"];
            ConfigInfo.SMTPUser     = ConfigurationManager.AppSettings["mail_smtp_server_user"];
            ConfigInfo.SMTPPassword = ConfigurationManager.AppSettings["mail_smtp_server_pass"];
            ConfigInfo.SMTPPort     = Convert.ToInt32(ConfigurationManager.AppSettings["mail_smtp_server_port"]);
            ConfigInfo.SMTPUseSSL   = Convert.ToBoolean(ConfigurationManager.AppSettings["mail_smtp_server_ssl"]);

            ConfigInfo.EmailFromAddress     = ConfigurationManager.AppSettings["mail_from_address"];
            ConfigInfo.EmailSubject         = ConfigurationManager.AppSettings["mail_subject"];
            ConfigInfo.EmailHtmlBody        = ConfigurationManager.AppSettings["mail_html_body"];
            ConfigInfo.EmailDeliveryOptions = ConfigurationManager.AppSettings["mail_delivery_options"];

            ConfigInfo.WsaaUrl = ConfigurationManager.AppSettings["WsaaUrl"];
            ConfigInfo.WsfeUrl = ConfigurationManager.AppSettings["WsfeUrl"];
            ConfigInfo.PfxPath = ConfigurationManager.AppSettings["PfxPath"];
            ConfigInfo.PfxPass = ConfigurationManager.AppSettings["PfxPass"];
            ConfigInfo.PtoVta = ConfigurationManager.AppSettings["PtoVta"];
            ConfigInfo.CUIT = ConfigurationManager.AppSettings["CUIT"].ToLong();
        }

        private bool LoadTXT()
        {
            _data = new List<Header>();

            try
            {
                lblStatus.Text    = @"Cargando archivo";
                barProgress.Style = ProgressBarStyle.Marquee;

                var    sr         = new StreamReader(_inputFile.FullName, Encoding.GetEncoding(850));
                Header header     = null;
                var    lineNumber = 0;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line != null && line.StartsWith("0"))
                    {
                        if (header != null)
                        {
                            _data.Add(header);
                        }

                        header = new Header
                        {
                            /*02*/NombreComprobante = line.Substring(1, 30).Trim(),
                            /*03*/TipoComprobante   = line.Substring(31, 2).Trim(),
                            /*04*/CondicionIVA      = line.Substring(33, 1).Trim(),
                            /*05*/CentroEmisor      = line.Substring(34, 4).Trim(),
                            /*06*/NumeroComprobante = line.Substring(38, 8).Trim(),
                            /*07*/FechaEmision      = line.Substring(46, 10).Trim(),
                            /*08*/Detalle1          = line.Substring(56, 75).Trim(),
                            /*09*/Detalle2          = line.Substring(131, 75).Trim(),
                            /*10*/Domicilio         = line.Substring(206, 52).Trim(),
                            /*11*/Localidad         = line.Substring(258, 20).Trim(),
                            /*12*/Barrio            = line.Substring(278, 30).Trim(),
                            /*13*/CodigoPostal      = line.Substring(308, 8).Trim(),
                            /*14*/CUILCUIT          = line.Substring(316, 13).Trim().Replace("/", "-"),
                            /*15*/FechaVencimiento  = line.Substring(329, 10).Trim(),
                            /*16*/NumeroCAE         = line.Substring(339, 14).Trim(),
                            /*17*/CondicionPago     = line.Substring(353, 70).Trim(),
                            /*18*/Importe           = line.Substring(423, 12).Trim(),
                            /*19*/MontoIVA          = line.Substring(435, 12).Trim(),
                            /*20*/MontoGravado      = line.Substring(447, 12).Trim(),
                            /*21*/MontoNoGravado    = line.Substring(459, 12).Trim(),
                            /*22*/Subtotal          = line.Substring(471, 12).Trim(),
                            /*23*/CodigoBarra       = line.Substring(483, 50).Trim(),
                            /*24*/RefPagoMisCuentas = line.Substring(533, 19).Trim(),
                            /*25*/RefRedLink        = line.Substring(552, 19).Trim(),
                            /*26*/Email             = line.Substring(571, 50).Trim(),
                            /*27*/TipoIVA           = line.Substring(621, 25).Trim(),
                            /*28*/FechaVencimiento2 = line.Substring(646, 35).Trim(),
                            /*29*/FechaVencimiento3 = line.Substring(681, 35).Trim()
                        };
                    }
                    else if (line != null && line.StartsWith("1"))
                    {
                        if (header == null)
                            continue;

                        var item = new Detail
                        {
                            Detalle        = line.Substring(1, 50).Trim(),
                            MontoFacturado = line.Substring(51, 12).Trim()
                        };

                        header.Items.Add(item);
                    }

                    lineNumber++;
                    lblStatus.Text = $@"Cargando línea {lineNumber}";

                    Application.DoEvents();
                }

                _data.Add(header);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
            finally
            {
                lblStatus.Text    = string.Empty;
                barProgress.Style = ProgressBarStyle.Continuous;
            }
        }

        private bool LoadTxtAfip()
        {
            _dataAfip = new DataAfip();

            try
            {
                lblStatus.Text    = @"Cargando archivo";
                barProgress.Style = ProgressBarStyle.Marquee;

                var lineNumber = 0;

                using (var sr = new StreamReader(_inputFile.FullName, Encoding.GetEncoding(850)))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        if (line != null && line.StartsWith("1"))
                        {
                            _dataAfip.Facturas.Add(new Factura(line));
                        }
                        else if (line != null && line.StartsWith("2"))
                        {
                            _dataAfip.Resumen = new ResumenFacturas(line);
                        }

                        lineNumber++;
                        lblStatus.Text = $@"Cargando línea {lineNumber}";

                        Application.DoEvents();
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
                lblStatus.Text    = string.Empty;
                barProgress.Style = ProgressBarStyle.Continuous;
            }
        }

        private bool Import()
        {
            try
            {
                lblStatus.Text      = @"Importando registros";
                barProgress.Style   = ProgressBarStyle.Continuous;
                barProgress.Maximum = _data.Count;

                _db.Connect();
                _db.BeginTran();

                var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var query =
                    $@"INSERT INTO ImportedFile(Name, TimeStamp, Records) VALUES('{_inputFile.Name}','{timeStamp}',{_data.Count});";

                _db.ExecuteNonQuery(query);

                query = $@"SELECT Id FROM ImportedFile WHERE TimeStamp = '{timeStamp}';";

                var importedFileId = _db.ExecuteScalar(query);

                foreach (var header in _data)
                {
                    query =
                        @"INSERT INTO Header(ImportedFileId, NombreComprobante, TipoComprobante, CondicionIVA, CentroEmisor, NumeroComprobante, FechaEmision, Detalle1, Detalle2, Domicilio, Localidad, Barrio, CodigoPostal, CUILCUIT, FechaVencimiento, NumeroCAE, CondicionPago, Importe, MontoIVA, MontoGravado, MontoNoGravado, Subtotal, CodigoBarra, RefPagoMisCuentas, RefRedLink, Email, TipoIVA, FechaVencimiento2, FechaVencimiento3) ";
                    query +=
                        @"VALUES(@ImportedFileId, @NombreComprobante, @TipoComprobante, @CondicionIVA, @CentroEmisor, @NumeroComprobante, @FechaEmision, @Detalle1, @Detalle2, @Domicilio, @Localidad, @Barrio, @CodigoPostal, @CUILCUIT, @FechaVencimiento, @NumeroCAE, @CondicionPago, @Importe, @MontoIVA, @MontoGravado, @MontoNoGravado, @Subtotal, @CodigoBarra, @RefPagoMisCuentas, @RefRedLink, @Email, @TipoIVA, @FechaVencimiento2, @FechaVencimiento3);";

                    var parameters = new Hashtable
                    {
                        {"@ImportedFileId", importedFileId},
                        {"@NombreComprobante", header.NombreComprobante},
                        {"@TipoComprobante", header.TipoComprobante},
                        {"@CondicionIVA", header.CondicionIVA},
                        {"@CentroEmisor", header.CentroEmisor},
                        {"@NumeroComprobante", header.NumeroComprobante},
                        {"@FechaEmision", header.FechaEmision},
                        {"@Detalle1", header.Detalle1},
                        {"@Detalle2", header.Detalle2},
                        {"@Domicilio", header.Domicilio},
                        {"@Localidad", header.Localidad},
                        {"@Barrio", header.Barrio},
                        {"@CodigoPostal", header.CodigoPostal},
                        {"@CUILCUIT", header.CUILCUIT},
                        {"@FechaVencimiento", header.FechaVencimiento},
                        {"@NumeroCAE", header.NumeroCAE},
                        {"@CondicionPago", header.CondicionPago},
                        {"@Importe", header.Importe},
                        {"@MontoIVA", header.MontoIVA},
                        {"@MontoGravado", header.MontoGravado},
                        {"@MontoNoGravado", header.MontoNoGravado},
                        {"@Subtotal", header.Subtotal},
                        {"@CodigoBarra", header.CodigoBarra},
                        {"@RefPagoMisCuentas", header.RefPagoMisCuentas},
                        {"@RefRedLink", header.RefRedLink},
                        {"@Email", header.Email},
                        {"@TipoIVA", header.TipoIVA},
                        {"@FechaVencimiento2", header.FechaVencimiento2},
                        {"@FechaVencimiento3", header.FechaVencimiento3}
                    };

                    _db.ExecuteNonQuery(query, parameters);

                    query = @"SELECT MAX(Id) FROM Header;";

                    var headerId = _db.ExecuteScalar(query);

                    foreach (var detail in header.Items)
                    {
                        query =
                            $@"INSERT INTO Detail(HeaderId, Detalle, MontoFacturado) VALUES({headerId},'{detail.Detalle}',{detail.MontoFacturado});";

                        _db.ExecuteNonQuery(query);
                    }

                    lblStatus.Text = $@"Registro {barProgress.Value}/{barProgress.Maximum}";
                    barProgress.PerformStep();

                    Application.DoEvents();
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

                lblStatus.Text    = string.Empty;
                barProgress.Value = 0;
            }
        }

        private bool ImportAfip()
        {
            try
            {
                lblStatus.Text      = @"Importando registros";
                barProgress.Style   = ProgressBarStyle.Continuous;
                barProgress.Maximum = _dataAfip.Facturas.Count;

                _db.Connect();
                _db.BeginTran();

                var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var query = "";

                query += @"INSERT INTO ResumenFacturas (NombreArchivo, FechaImportacion, TipoRegistro, Periodo, Relleno1, CantidadRegistrosTipo1, Relleno2, CUIT, Relleno3, ImporteTotal, ImporteTotalSinNetoGravado, ImporteNetoGravado, ImpuestoLiquidado, ImpuestoLiquidadoNoCategorizado, ImporteOperacionesExentas, ImportePagosCuentaImpuestos, ImporteIIBB, ImporteIngresosMunicipales, ImporteImpuestosInternos, Relleno4) ";
                query += @"VALUES (@NombreArchivo, @FechaImportacion, @TipoRegistro, @Periodo, @Relleno1, @CantidadRegistrosTipo1, @Relleno2, @CUIT, @Relleno3, @ImporteTotal, @ImporteTotalSinNetoGravado, @ImporteNetoGravado, @ImpuestoLiquidado, @ImpuestoLiquidadoNoCategorizado, @ImporteOperacionesExentas, @ImportePagosCuentaImpuestos, @ImporteIIBB, @ImporteIngresosMunicipales, @ImporteImpuestosInternos, @Relleno4);";

                var parameters = new Hashtable
                {
                    {"@NombreArchivo", _inputFile.Name},
                    {"@FechaImportacion", timeStamp},
                    {"@TipoRegistro", _dataAfip.Resumen.TipoRegistro},
                    {"@Periodo", _dataAfip.Resumen.Periodo},
                    {"@Relleno1", _dataAfip.Resumen.Relleno1},
                    {"@CantidadRegistrosTipo1", _dataAfip.Resumen.CantidadRegistrosTipo1},
                    {"@Relleno2", _dataAfip.Resumen.Relleno2},
                    {"@CUIT", _dataAfip.Resumen.CUIT},
                    {"@Relleno3", _dataAfip.Resumen.Relleno3},
                    {"@ImporteTotal", _dataAfip.Resumen.ImporteTotal},
                    {"@ImporteTotalSinNetoGravado", _dataAfip.Resumen.ImporteTotalSinNetoGravado},
                    {"@ImporteNetoGravado", _dataAfip.Resumen.ImporteNetoGravado},
                    {"@ImpuestoLiquidado", _dataAfip.Resumen.ImpuestoLiquidado},
                    {"@ImpuestoLiquidadoNoCategorizado", _dataAfip.Resumen.ImpuestoLiquidadoNoCategorizado},
                    {"@ImporteOperacionesExentas", _dataAfip.Resumen.ImporteOperacionesExentas},
                    {"@ImportePagosCuentaImpuestos", _dataAfip.Resumen.ImportePagosCuentaImpuestos},
                    {"@ImporteIIBB", _dataAfip.Resumen.ImporteIIBB},
                    {"@ImporteIngresosMunicipales", _dataAfip.Resumen.ImporteIngresosMunicipales},
                    {"@ImporteImpuestosInternos", _dataAfip.Resumen.ImporteImpuestosInternos},
                    {"@Relleno4", _dataAfip.Resumen.Relleno4}
                };

                _db.ExecuteNonQuery(query, parameters);

                query = $@"SELECT IdArchivo FROM ResumenFacturas WHERE FechaImportacion = '{timeStamp}';";

                var idArchivo = _db.ExecuteScalar(query).ToInt();

                foreach (var f in _dataAfip.Facturas)
                {
                    query  = @"INSERT INTO Facturas(IdArchivo, TipoRegistro, FechaComprobante, TipoComprobante, ControladorFiscal, PuntoVenta, NumeroComprobante, NumeroComprobanteRegistrado, CantidadHojas, CodigoDocumento, IdComprador, DenominacionComprador, ImporteTotal, ImporteTotalSinNetoGravado, ImporteNetoGravado, ImpuestoLiquidado, ImpuestoLiquidadoNoCategorizado, ImporteOperacionesExentas, ImportePagosCuentaImpuestos, ImporteIIBB, ImporteIngresosMunicipales, ImporteImpuestosInternos, FechaDesde, FechaHasta, FechaVencimientoPago, Relleno, CantidadAlicuotasIVA, CodigoOperacion, CodigoAutorizacion, FechaAutorizacion, FechaAnulacion) ";
                    query += @"VALUES(@IdArchivo, @TipoRegistro, @FechaComprobante, @TipoComprobante, @ControladorFiscal, @PuntoVenta, @NumeroComprobante, @NumeroComprobanteRegistrado, @CantidadHojas, @CodigoDocumento, @IdComprador, @DenominacionComprador, @ImporteTotal, @ImporteTotalSinNetoGravado, @ImporteNetoGravado, @ImpuestoLiquidado, @ImpuestoLiquidadoNoCategorizado, @ImporteOperacionesExentas, @ImportePagosCuentaImpuestos, @ImporteIIBB, @ImporteIngresosMunicipales, @ImporteImpuestosInternos, @FechaDesde, @FechaHasta, @FechaVencimientoPago, @Relleno, @CantidadAlicuotasIVA, @CodigoOperacion, @CodigoAutorizacion, @FechaAutorizacion, @FechaAnulacion)";

                    parameters = new Hashtable
                    {
                        {"@IdArchivo", idArchivo},
                        {"@TipoRegistro", f.TipoRegistro},
                        {"@FechaComprobante", f.FechaComprobante},
                        {"@TipoComprobante", f.TipoComprobante},
                        {"@ControladorFiscal", f.ControladorFiscal},
                        {"@PuntoVenta", f.PuntoVenta},
                        {"@NumeroComprobante", f.NumeroComprobante},
                        {"@NumeroComprobanteRegistrado", f.NumeroComprobanteRegistrado},
                        {"@CantidadHojas", f.CantidadHojas},
                        {"@CodigoDocumento", f.CodigoDocumento},
                        {"@IdComprador", f.IdComprador},
                        {"@DenominacionComprador", f.DenominacionComprador},
                        {"@ImporteTotal", f.ImporteTotal},
                        {"@ImporteTotalSinNetoGravado", f.ImporteTotalSinNetoGravado},
                        {"@ImporteNetoGravado", f.ImporteNetoGravado},
                        {"@ImpuestoLiquidado", f.ImpuestoLiquidado},
                        {"@ImpuestoLiquidadoNoCategorizado", f.ImpuestoLiquidadoNoCategorizado},
                        {"@ImporteOperacionesExentas", f.ImporteOperacionesExentas},
                        {"@ImportePagosCuentaImpuestos", f.ImportePagosCuentaImpuestos},
                        {"@ImporteIIBB", f.ImporteIIBB},
                        {"@ImporteIngresosMunicipales", f.ImporteIngresosMunicipales},
                        {"@ImporteImpuestosInternos", f.ImporteImpuestosInternos},
                        {"@FechaDesde", f.FechaDesde},
                        {"@FechaHasta", f.FechaHasta},
                        {"@FechaVencimientoPago", f.FechaVencimientoPago},
                        {"@Relleno", f.Relleno},
                        {"@CantidadAlicuotasIVA", f.CantidadAlicuotasIVA},
                        {"@CodigoOperacion", f.CodigoOperacion},
                        {"@CodigoAutorizacion", f.CodigoAutorizacion},
                        {"@FechaAutorizacion", f.FechaAutorizacion},
                        {"@FechaAnulacion", f.FechaAnulacion}
                    };

                    _db.ExecuteNonQuery(query, parameters);

                    query = $"SELECT max(Id) FROM Facturas WHERE IdArchivo = {idArchivo}";

                    f.Id = _db.ExecuteScalar(query).ToInt();
                    f.IdArchivo = idArchivo;

                    lblStatus.Text = $@"Registro {barProgress.Value}/{barProgress.Maximum}";
                    barProgress.PerformStep();

                    Application.DoEvents();
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

                lblStatus.Text    = string.Empty;
                barProgress.Value = 0;
            }
        }

        private bool GetCae()
        {
            try
            {
                lblStatus.Text      = @"Importando registros";
                barProgress.Style   = ProgressBarStyle.Continuous;
                barProgress.Maximum = _dataAfip.Facturas.Count;

                _db.Connect();

                var wsaa = new WsaaClient();

                var pass = new SecureString();

                foreach (var c in ConfigInfo.PfxPass)
                {
                    pass.AppendChar(c);
                }

                pass.MakeReadOnly();

                wsaa.ObtenerLoginTicketResponse("wsfe", ConfigInfo.WsaaUrl, ConfigInfo.PfxPath, pass, null, null, null, true);

                var authRequest = new FEAuthRequest
                {
                    Token = wsaa.Token,
                    Sign  = wsaa.Sign,
                    Cuit  = ConfigInfo.CUIT
                };

                var svcWsfe = new Afip.Wsfe.Service
                {
                    Url = ConfigInfo.WsfeUrl
                };

                var sb = new StringBuilder();

                foreach (var f in _dataAfip.Facturas)
                {
                    var cab = new FECAECabRequest
                    {
                        CantReg = 1,
                        PtoVta = f.PuntoVenta,
                        CbteTipo = f.TipoComprobante
                    };

                    var det = new FECAEDetRequest
                    {
                        Concepto     = 1,
                        DocTipo      = f.CodigoDocumento,
                        DocNro       = f.IdComprador,
                        CbteDesde    = f.NumeroComprobante,
                        CbteHasta    = f.NumeroComprobanteRegistrado,
                        CbteFch      = f.FechaComprobante.ToString(),
                        ImpTotal     = f.ImporteTotal,
                        ImpTotConc   = f.ImporteTotalSinNetoGravado,
                        ImpNeto      = f.ImporteNetoGravado,
                        ImpOpEx      = f.ImporteOperacionesExentas,
                        ImpTrib      = f.ImportePagosCuentaImpuestos,
                        ImpIVA       = f.ImpuestoLiquidado,
                        FchServDesde = f.FechaDesde.ToString(),
                        FchServHasta = f.FechaHasta.ToString(),
                        FchVtoPago   = f.FechaVencimientoPago.ToString(),
                        MonId        = "PES",
                        MonCotiz     = 1
                    };

                    var req = new FECAERequest
                    {
                        FeCabReq = cab,
                        FeDetReq = new[]
                        {
                            det
                        }
                    };

                    var response = svcWsfe.FECAESolicitar(authRequest, req);

                    var saveData = true;

                    if (response.FeDetResp[0].Observaciones != null)
                    {
                        foreach (var o in response.FeDetResp[0].Observaciones)
                        {
                            sb.AppendLine($"Obs: {o.Code} - {o.Msg}");
                        }

                        saveData = false;
                    }

                    if (response.Errors != null)
                    {
                        foreach (var o in response.Errors)
                        {
                            sb.AppendLine($"Err: {o.Code} - {o.Msg}");
                        }

                        saveData = false;
                    }

                    if (response.Events != null)
                    {
                        foreach (var o in response.Events)
                        {
                            sb.AppendLine($"Evt: {o.Code} - {o.Msg}");
                        }

                        saveData = false;
                    }

                    if (saveData)
                    {
                        var query = "UPDATE Facturas SET Cae = @Cae, CaeVto = @CaeVto WHERE IdArchivo = @IdArchivo AND Id = @Id";

                        var parameters = new Hashtable
                        {
                            {"@Cae", response.FeDetResp[0].CAE},
                            {"@CaeVto", response.FeDetResp[0].CAEFchVto},
                            {"@IdArchivo", f.IdArchivo},
                            {"@Id", f.Id}
                        };

                        _db.ExecuteNonQuery(query, parameters);
                    }

                    lblStatus.Text = $@"Registro {barProgress.Value}/{barProgress.Maximum}";
                    barProgress.PerformStep();

                    Application.DoEvents();
                }

                if (sb.Length > 0)
                    throw new ApplicationException(sb.ToString());

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
            finally
            {
                lblStatus.Text    = string.Empty;
                barProgress.Value = 0;

                _db.Disconnect();
            }
        }

        private void GeneratePDF()
        {
            try
            {
                var threads = new List<Thread>();

                var template = File.ReadAllText(Path.Combine(Application.StartupPath, @"template.html"));

                var start = template.IndexOf(@"<table class=""details"">", 0, StringComparison.OrdinalIgnoreCase) +
                            @"<table class=""details"">".Length;
                var end            = template.IndexOf(@"</table>", start, StringComparison.OrdinalIgnoreCase);
                var templateDetail = template.Substring(start, end - start);

                lblStatus.Text      = @"Generando templates";
                barProgress.Style   = ProgressBarStyle.Continuous;
                barProgress.Maximum = _data.Count;

                foreach (var header in _data)
                {
                    var html = template;
                    var bgFile =
                        $"file:///{Path.Combine(Application.StartupPath, $"fc_{header.CondicionIVA.ToLower()}.jpg")}"
                            .Replace("\\", "/");

                    html = html.Replace("{{BackgroundFilePath}}", bgFile);
                    html = html.Replace("{{ShowSubtotales}}",
                                        header.CondicionIVA.Equals("b", StringComparison.OrdinalIgnoreCase)
                                            ? "hidden"
                                            : "visible");

                    header.CodigoBarra = "977123456700";

                    if (!string.IsNullOrEmpty(header.CodigoBarra))
                    {
                        var barcode = Int2of5.GenerateBarCode(header.CodigoBarra, 1000, 100, 10).ToBase64();
                        html = html.Replace("{{Barcode}}", $"data:image/png;base64, {barcode}");
                        html = html.Replace("{{show-barcode}}", "");
                    }
                    else
                    {
                        html = html.Replace("{{show-barcode}}", "display: none !important; visibility: hidden;");
                    }

                    var properties = typeof(Header).GetProperties();

                    html = properties.Aggregate(
                        html,
                        (current, property) =>
                            current.Replace($"{{{{{property.Name}}}}}", property.GetValue(header, null).ToString()));

                    var htmlDetail = string.Empty;

                    foreach (var detail in header.Items)
                    {
                        htmlDetail += templateDetail;

                        properties = typeof(Detail).GetProperties();

                        htmlDetail =
                            properties.Aggregate(
                                htmlDetail,
                                (current, property) =>
                                    current.Replace($"{{{{{property.Name}}}}}",
                                                    property.GetValue(detail, null).ToString()));
                    }

                    html = html.Replace(templateDetail, htmlDetail);

                    var pdfName =
                        $@"{header.TipoComprobante}{header.CondicionIVA}{header.CentroEmisor}{header.NumeroComprobante}.pdf";
                    var outputFile = Path.Combine(txtOutputFolder.Text, pdfName);

                    var p = new object[] {html, outputFile};

                    threads.Add(new Thread(delegate() { ConvertToPDF(p); }));

                    Application.DoEvents();

                    barProgress.PerformStep();
                    lblStatus.Text = $@"Template {barProgress.Value}/{barProgress.Maximum}";
                }

                var batchSize    = ConfigInfo.BatchSize;
                var currentBatch = 0;

                barProgress.Value = barProgress.Minimum;

                while (true)
                {
                    threads.Where(t => t.ThreadState == ThreadState.Unstarted).Take(batchSize).ToList()
                           .ForEach(t => t.Start());

                    currentBatch++;

                    while (threads.Any(t => t.IsAlive))
                    {
                        Application.DoEvents();
                    }

                    if (currentBatch * batchSize > threads.Count + batchSize)
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ConvertToPDF(object state)
        {
            var p = state as object[];

            var html       = p[0].ToString();
            var outputFile = p[1].ToString();

            var converter = new HtmlToPdf
            {
                Options =
                {
                    PdfPageSize        = PdfPageSize.A4,
                    PdfPageOrientation = PdfPageOrientation.Portrait,
                    WebPageWidth       = 210,
                    WebPageHeight      = 297,
                    CssMediaType       = HtmlToPdfCssMediaType.Screen,
                    DrawBackground     = true
                }
            };

            //Generate PDF file
            var doc = converter.ConvertHtmlString(html);

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            doc.Save(outputFile);
            doc.Close();

            if (ConfigInfo.SaveHtml)
                File.WriteAllText(outputFile.Replace(".pdf", ".html"), html, Encoding.UTF8);

            BeginInvoke((Action) (() =>
            {
                barProgress.PerformStep();
                lblStatus.Text = $@"Archivo PDF {barProgress.Value}/{barProgress.Maximum}";
            }));
        }

        private void GeneratePDF2()
        {
            var viewer = new RdlViewer
            {
                SourceFile = new Uri(Path.Combine(Application.StartupPath, "FE2PDF.rdl"))
            };

            foreach (var h in _data)
            {
                var header  = new List<Header> {h};
                var details = h.Items;

                viewer.Report.DataSets["Header"].SetData(header);
                viewer.Report.DataSets["Details"].SetData(details);

                var barcode = !string.IsNullOrEmpty(h.CodigoBarra)
                    ? Int2of5.GenerateBarCode(h.CodigoBarra, 1000, 100, 2).ToBase64()
                    : string.Empty;

                viewer.Parameters = string.Empty;
                viewer.Parameters +=
                    $@"&bg_image={Path.Combine(Application.StartupPath, $"fc_{h.CondicionIVA.ToLower()}.jpg")}";

                if (!string.IsNullOrEmpty(barcode))
                    viewer.Parameters += $@"&barcode={barcode}";

                viewer.Rebuild();

                var pdfName    = $@"{h.TipoComprobante}{h.CondicionIVA}{h.CentroEmisor}{h.NumeroComprobante}.pdf";
                var outputFile = Path.Combine(txtOutputFolder.Text, pdfName);

                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                viewer.SaveAs(outputFile, OutputPresentationType.PDF);

                barProgress.PerformStep();
                lblStatus.Text = $@"Archivo PDF {barProgress.Value}/{barProgress.Maximum}";

                Application.DoEvents();
            }
        }

        private bool SendEmails()
        {
            lblStatus.Text = @"Enviando emails";

            var mailAuthentication = new System.Net.NetworkCredential(ConfigInfo.SMTPUser, ConfigInfo.SMTPPassword);

            var mailClient = new SmtpClient(ConfigInfo.SMTPServer, ConfigInfo.SMTPPort)
            {
                EnableSsl             = ConfigInfo.SMTPUseSSL,
                UseDefaultCredentials = false,
                Credentials           = mailAuthentication
            };

            var mailMessage = new MailMessage
            {
                From       = new MailAddress(ConfigInfo.EmailFromAddress),
                Subject    = ConfigInfo.EmailSubject,
                IsBodyHtml = true,
                Body = !string.IsNullOrEmpty(ConfigInfo.EmailHtmlBody)
                    ? System.Web.HttpUtility.HtmlDecode(ConfigInfo.EmailHtmlBody)
                    : string.Empty
            };

            if (!string.IsNullOrEmpty(ConfigInfo.EmailDeliveryOptions))
            {
                var options = ConfigInfo.EmailDeliveryOptions.Split(',');

                foreach (var opt in options)
                {
                    var option = (DeliveryNotificationOptions) Enum.Parse(typeof(DeliveryNotificationOptions), opt);

                    mailMessage.DeliveryNotificationOptions = mailMessage.DeliveryNotificationOptions | option;
                }

                mailMessage.Headers.Add("Disposition-Notification-To", ConfigInfo.EmailFromAddress);
            }

            var headers = _data.Where(x => !string.IsNullOrEmpty(x.Email)).ToList();

            barProgress.Style   = ProgressBarStyle.Continuous;
            barProgress.Maximum = headers.Count;

            var emailDeliveryLog = new StringBuilder();

            foreach (var header in headers)
            {
                try
                {
                    var pdfFile = Path.Combine(txtOutputFolder.Text,
                                               $@"{header.TipoComprobante}{header.CondicionIVA}{header.CentroEmisor}{header.NumeroComprobante}.pdf");

                    if (!File.Exists(pdfFile))
                        continue;

                    mailMessage.To.Clear();
                    mailMessage.To.Add(header.Email);
                    //mailMessage.To.Add("enw1986@hotmail.com");
                    //mailMessage.To.Add("mrubio@rdmsolutions.com.ar");

                    mailMessage.Attachments.Clear();
                    mailMessage.Attachments.Add(new Attachment(pdfFile));

                    mailClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    emailDeliveryLog.AppendLine($@"Envío de email a la dirección {header.Email} falló. Detalle: {ex}");
                }
                finally
                {
                    barProgress.PerformStep();

                    lblStatus.Text = $@"Email {barProgress.Value}/{barProgress.Maximum}";

                    Application.DoEvents();
                }
            }

            mailClient.Dispose();
            mailMessage.Dispose();

            if (emailDeliveryLog.Length > 0)
            {
                using (var sw =
                    new StreamWriter(Path.Combine(txtOutputFolder.Text, $@"log_{DateTime.Now:yyyyMMddHHmmss}.txt")))
                {
                    sw.WriteLine(emailDeliveryLog.ToString());
                    sw.Flush();
                    sw.Close();
                }

                return true;
            }

            return false;
        }
    }
}