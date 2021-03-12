namespace FE2PDF
{
    public static class ConfigInfo
    {
        public static string SourcePath    { get; set; }
        public static string ProcessedPath { get; set; }
        public static string ErrorPath     { get; set; }
        public static int    BatchSize     { get; set; }
        public static bool   SaveHtml      { get; set; }

        public static string SMTPServer   { get; set; }
        public static string SMTPUser     { get; set; }
        public static string SMTPPassword { get; set; }
        public static int    SMTPPort     { get; set; }
        public static bool   SMTPUseSSL   { get; set; }

        public static string EmailFromAddress     { get; set; }
        public static string EmailCCAddress       { get; set; }
        public static string EmailCCOAddress      { get; set; }
        public static string EmailSubject         { get; set; } = "";
        public static string EmailHtmlBody        { get; set; } = "";
        public static string EmailDeliveryOptions { get; set; } = "";

        public static string WsaaUrl { get; set; } = "";
        public static string WsfeUrl { get; set; } = "";
        public static string PfxPath { get; set; } = "";
        public static string PfxPass { get; set; } = "";
        public static string PtoVta { get; set; } = "";
        public static long CUIT { get; set; } = 0;

        public static InvoiceCodeType InvoiceCodeType { get; set; } = InvoiceCodeType.None;
    }

    public enum InvoiceCodeType
    {
        None,
        Barcode,
        QR
    }
}