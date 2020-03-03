using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FE2PDF.Afip.Wsaa;

namespace FE2PDF.AFIP
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class WsaaClient
    {
        public uint        UniqueId;       // Entero de 32 bits sin signo que identifica el requerimiento
        public DateTime    GenerationTime; // Momento en que fue generado el requerimiento
        public DateTime    ExpirationTime; // Momento en el que expira la solicitud
        public string      Service;        // Identificacion del WSN para el cual se solicita el TA
        public string      Sign;           // Firma de seguridad recibida en la respuesta
        public string      Token;          // Token de seguridad recibido en la respuesta
        public XmlDocument XmlLoginTicketRequest;
        public XmlDocument XmlLoginTicketResponse;
        public string      RutaDelCertificadoFirmante;
        public X509Certificate2 Certificado;

        public string XmlStrLoginTicketRequestTemplate =
            "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";

        private        bool _verboseMode = true;
        private static uint _globalUniqueID; // OJO! NO ES THREAD-SAFE

        /// <summary>
        /// Construye un Login Ticket obtenido del WSAA
        /// </summary>
        /// <param name="argServicio">Servicio al que se desea acceder</param>
        /// <param name="argUrlWsaa">URL del WSAA</param>
        /// <param name="argRutaCertX509Firmante">Ruta del certificado X509 (con clave privada) usado para firmar</param>
        /// <param name="argPassword">Password del certificado X509 (con clave privada) usado para firmar</param>
        /// <param name="argProxy">IP:port del proxy</param>
        /// <param name="argProxyUser">Usuario del proxy</param>'''
        /// <param name="argProxyPassword">Password del proxy</param>
        /// <param name="argVerbose">Nivel detallado de descripcion? true/false</param>
        /// <remarks></remarks>
        public string ObtenerLoginTicketResponse(string       argServicio,
                                                 string       argUrlWsaa,
                                                 string       argRutaCertX509Firmante,
                                                 SecureString argPassword,
                                                 string       argProxy,
                                                 string       argProxyUser,
                                                 string       argProxyPassword,
                                                 bool         argVerbose)
        {
            const string ID_FNC = "[ObtenerLoginTicketResponse]";

            RutaDelCertificadoFirmante = argRutaCertX509Firmante;
            _verboseMode               = argVerbose;
            VerboseMode                = argVerbose;

            string cmsFirmadoBase64;
            string loginTicketResponse;

            var xmlFile = Path.Combine(Application.StartupPath, $"{ConfigInfo.CUIT}.xml");

            if (File.Exists(xmlFile))
            {
                XmlLoginTicketResponse = new XmlDocument();
                XmlLoginTicketResponse.Load(xmlFile);

                ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);

                if (ExpirationTime > DateTime.Now.AddSeconds(10))
                {
                    UniqueId       = uint.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText);
                    GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
                    ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);
                    Sign           = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText;
                    Token          = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText;

                    return string.Empty;
                }
            }

            // PASO 1: Genero el Login Ticket Request
            try
            {
                _globalUniqueID += 1;

                XmlLoginTicketRequest = new XmlDocument();
                XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

                var xmlNodoUniqueId       = XmlLoginTicketRequest.SelectSingleNode("//uniqueId");
                var xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime");
                var xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime");
                var xmlNodoService        = XmlLoginTicketRequest.SelectSingleNode("//service");

                xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
                xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+10).ToString("s");
                xmlNodoUniqueId.InnerText       = Convert.ToString(_globalUniqueID);
                xmlNodoService.InnerText        = argServicio;
                Service                         = argServicio;

                if (_verboseMode) Console.WriteLine(XmlLoginTicketRequest.OuterXml);
            }
            catch (Exception excepcionAlGenerarLoginTicketRequest)
            {
                throw new Exception(
                    $"{ID_FNC}***Error GENERANDO el LoginTicketRequest : {excepcionAlGenerarLoginTicketRequest.Message}{excepcionAlGenerarLoginTicketRequest.StackTrace}");
            }

            // PASO 2: Firmo el Login Ticket Request
            try
            {
                if (_verboseMode)
                    Console.WriteLine(ID_FNC + @"***Leyendo certificado: {0}", RutaDelCertificadoFirmante);

                var certFirmante =
                    ObtieneCertificadoDesdeArchivo(RutaDelCertificadoFirmante, argPassword);

                if (_verboseMode)
                {
                    Console.WriteLine($@"{ID_FNC}***Firmando: ");
                    Console.WriteLine(XmlLoginTicketRequest.OuterXml);
                }

                // Convierto el Login Ticket Request a bytes, firmo el msg y lo convierto a Base64
                var EncodedMsg       = Encoding.UTF8;
                var msgBytes         = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);
                var encodedSignedCms = FirmaBytesMensaje(msgBytes, certFirmante);
                cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);

                Certificado = certFirmante;
            }
            catch (Exception excepcionAlFirmar)
            {
                throw new Exception($"{ID_FNC}***Error FIRMANDO el LoginTicketRequest : {excepcionAlFirmar.Message}");
            }

            // PASO 3: Invoco al WSAA para obtener el Login Ticket Response
            try
            {
                if (_verboseMode)
                {
                    Console.WriteLine($@"{ID_FNC}***Llamando al WSAA en URL: {argUrlWsaa}");
                    Console.WriteLine($@"{ID_FNC}***Argumento en el request:");
                    Console.WriteLine(cmsFirmadoBase64);
                }

                var servicioWsaa = new LoginCMSService
                {
                    Url = argUrlWsaa
                };


                // Veo si hay que salir a traves de un proxy
                if (argProxy != null)
                {
                    servicioWsaa.Proxy = new WebProxy(argProxy, true);

                    if (argProxyUser != null)
                    {
                        var Credentials = new NetworkCredential(argProxyUser, argProxyPassword);
                        servicioWsaa.Proxy.Credentials = Credentials;
                    }
                }

                loginTicketResponse = servicioWsaa.loginCms(cmsFirmadoBase64);

                if (_verboseMode)
                {
                    Console.WriteLine($@"{ID_FNC}***LoguinTicketResponse: ");
                    Console.WriteLine(loginTicketResponse);
                }
            }
            catch (Exception excepcionAlInvocarWsaa)
            {
                throw new Exception($"{ID_FNC}***Error INVOCANDO al servicio WSAA : {excepcionAlInvocarWsaa.Message}");
            }

            // PASO 4: Analizo el Login Ticket Response recibido del WSAA
            try
            {
                XmlLoginTicketResponse = new XmlDocument();
                XmlLoginTicketResponse.LoadXml(loginTicketResponse);
                XmlLoginTicketResponse.Save(xmlFile);

                UniqueId       = uint.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText);
                GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
                ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);
                Sign           = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText;
                Token          = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText;
            }
            catch (Exception excepcionAlAnalizarLoginTicketResponse)
            {
                throw new Exception(
                    $"{ID_FNC}***Error ANALIZANDO el LoginTicketResponse : {excepcionAlAnalizarLoginTicketResponse.Message}");
            }

            return loginTicketResponse;
        }

        public static bool VerboseMode;

        /// <summary>
        /// Firma mensaje
        /// </summary>
        /// <param name="argBytesMsg">Bytes del mensaje</param>
        /// <param name="argCertFirmante">Certificado usado para firmar</param>
        /// <returns>Bytes del mensaje firmado</returns>
        /// <remarks></remarks>
        public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertFirmante)
        {
            const string ID_FNC = "[FirmaBytesMensaje]";

            try
            {
                // Pongo el mensaje en un objeto ContentInfo (requerido para construir el obj SignedCms)
                var infoContenido = new ContentInfo(argBytesMsg);
                var cmsFirmado    = new SignedCms(infoContenido);

                // Creo objeto CmsSigner que tiene las caracteristicas del firmante
                var cmsFirmante = new CmsSigner(argCertFirmante)
                {
                    IncludeOption = X509IncludeOption.EndCertOnly
                };


                if (VerboseMode) Console.WriteLine($@"{ID_FNC}***Firmando bytes del mensaje...");

                // Firmo el mensaje PKCS #7
                cmsFirmado.ComputeSignature(cmsFirmante);

                if (VerboseMode) Console.WriteLine($@"{ID_FNC}***OK mensaje firmado");

                // Encodeo el mensaje PKCS #7.
                return cmsFirmado.Encode();
            }
            catch (Exception excepcionAlFirmar)
            {
                throw new Exception($"{ID_FNC}***Error al firmar: {excepcionAlFirmar.Message}");
            }
        }

        /// <summary>
        /// Lee certificado de disco
        /// </summary>
        /// <param name="argArchivo">Ruta del certificado a leer.</param>
        /// <param name="argPassword"></param>
        /// <returns>Un objeto certificado X509</returns>
        /// <remarks></remarks>
        public static X509Certificate2 ObtieneCertificadoDesdeArchivo(string argArchivo, SecureString argPassword)
        {
            const string ID_FNC  = "[ObtieneCertificadoDesdeArchivo]";
            var          objCert = new X509Certificate2();
            try
            {
                if (argPassword.IsReadOnly())
                {
                    objCert.Import(File.ReadAllBytes(argArchivo), argPassword, X509KeyStorageFlags.PersistKeySet);
                }
                else
                {
                    objCert.Import(File.ReadAllBytes(argArchivo));
                }

                return objCert;
            }
            catch (Exception excepcionAlImportarCertificado)
            {
                throw new Exception($"{ID_FNC}***Error al leer certificado: {excepcionAlImportarCertificado.Message}");
            }
        }
    }
}