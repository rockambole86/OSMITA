namespace FE2PDF
{
    public static class ConfigInfo
    {
        public static string SourcePath    { get; set; }
        public static string ProcessedPath { get; set; }
        public static string ErrorPath     { get; set; }

        public static string SMTPServer   { get; set; }
        public static string SMTPUser     { get; set; }
        public static string SMTPPassword { get; set; }
        public static int    SMTPPort     { get; set; }
        public static bool   SMTPUseSSL   { get; set; }

        public static string EmailFromAddress { get; set; }
        public static string EmailSubject     { get; set; } = "";
        public static string EmailHtmlBody    { get; set; } = "";
    }
}