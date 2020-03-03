using System.Collections.Generic;

namespace FE2PDF
{
    public class DataAfip
    {
        public List<Factura>   Facturas { get; set; } = new List<Factura>();
        public ResumenFacturas Resumen  { get; set; }
    }

    public class Factura
    {
        public int    IdArchivo                       { get; set; }
        public int    Id                              { get; set; }
        public int    TipoRegistro                    => 1;
        public int    FechaComprobante                { get; set; }
        public int    TipoComprobante                 { get; set; }
        public string ControladorFiscal               { get; set; }
        public int    PuntoVenta                      { get; set; }
        public int    NumeroComprobante               { get; set; }
        public int    NumeroComprobanteRegistrado     { get; set; }
        public int    CantidadHojas                   { get; set; }
        public int    CodigoDocumento                 { get; set; }
        public long   IdComprador                     { get; set; }
        public string DenominacionComprador           { get; set; }
        public double ImporteTotal                    { get; set; }
        public double ImporteTotalSinNetoGravado      { get; set; }
        public double ImporteNetoGravado              { get; set; }
        public double ImpuestoLiquidado               { get; set; }
        public double ImpuestoLiquidadoNoCategorizado { get; set; }
        public double ImporteOperacionesExentas       { get; set; }
        public double ImportePagosCuentaImpuestos     { get; set; }
        public double ImporteIIBB                     { get; set; }
        public double ImporteIngresosMunicipales      { get; set; }
        public double ImporteImpuestosInternos        { get; set; }
        public int    FechaDesde                      { get; set; }
        public int    FechaHasta                      { get; set; }
        public int    FechaVencimientoPago            { get; set; }
        public int    Relleno                         { get; set; }
        public int    CantidadAlicuotasIVA            { get; set; }
        public string CodigoOperacion                 { get; set; }
        public int    CodigoAutorizacion              { get; set; }
        public int    FechaAutorizacion               { get; set; }
        public int    FechaAnulacion                  { get; set; }

        public Factura(string line)
        {
            FechaComprobante                = line.Substring(1, 8).ToInt();
            TipoComprobante                 = line.Substring(9, 2).ToInt();
            ControladorFiscal               = line.Substring(11, 1);
            PuntoVenta                      = line.Substring(12, 4).ToInt();
            NumeroComprobante               = line.Substring(16, 8).ToInt();
            NumeroComprobanteRegistrado     = line.Substring(24, 8).ToInt();
            CantidadHojas                   = line.Substring(32, 3).ToInt();
            CodigoDocumento                 = line.Substring(35, 2).ToInt();
            IdComprador                     = line.Substring(37, 11).ToLong();
            DenominacionComprador           = line.Substring(48, 30);
            ImporteTotal                    = line.Substring(78, 15).ToInt() * 0.01;
            ImporteTotalSinNetoGravado      = line.Substring(93, 15).ToInt() * 0.01;
            ImporteNetoGravado              = line.Substring(108, 15).ToInt() * 0.01;
            ImpuestoLiquidado               = line.Substring(123, 15).ToInt() * 0.01;
            ImpuestoLiquidadoNoCategorizado = line.Substring(138, 15).ToInt() * 0.01;
            ImporteOperacionesExentas       = line.Substring(153, 15).ToInt() * 0.01;
            ImportePagosCuentaImpuestos     = line.Substring(168, 15).ToInt() * 0.01;
            ImporteIIBB                     = line.Substring(183, 15).ToInt() * 0.01;
            ImporteIngresosMunicipales      = line.Substring(198, 15).ToInt() * 0.01;
            ImporteImpuestosInternos        = line.Substring(213, 15).ToInt() * 0.01;
            FechaDesde                      = line.Substring(228, 8).ToInt();
            FechaHasta                      = line.Substring(236, 8).ToInt();
            FechaVencimientoPago            = line.Substring(244, 8).ToInt();
            Relleno                         = line.Substring(252, 6).ToInt();
            CantidadAlicuotasIVA            = line.Substring(258, 1).ToInt();
            CodigoOperacion                 = line.Substring(259, 1);
            CodigoAutorizacion              = line.Substring(260, 14).ToInt();
            FechaAutorizacion               = line.Substring(274, 8).ToInt();
            FechaAnulacion                  = line.Substring(282, 8).ToInt();
        }
    }

    public class ResumenFacturas
    {
        public int    TipoRegistro                    => 2;
        public int    Periodo                         { get; set; }
        public string Relleno1                        { get; set; }
        public int    CantidadRegistrosTipo1          { get; set; }
        public string Relleno2                        { get; set; }
        public long   CUIT                            { get; set; }
        public string Relleno3                        { get; set; }
        public double ImporteTotal                    { get; set; }
        public double ImporteTotalSinNetoGravado      { get; set; }
        public double ImporteNetoGravado              { get; set; }
        public double ImpuestoLiquidado               { get; set; }
        public double ImpuestoLiquidadoNoCategorizado { get; set; }
        public double ImporteOperacionesExentas       { get; set; }
        public double ImportePagosCuentaImpuestos     { get; set; }
        public double ImporteIIBB                     { get; set; }
        public double ImporteIngresosMunicipales      { get; set; }
        public double ImporteImpuestosInternos        { get; set; }
        public string Relleno4                        { get; set; }

        public ResumenFacturas(string line)
        {
            Periodo                         = line.Substring(1, 6).ToInt();
            Relleno1                        = line.Substring(7, 13);
            CantidadRegistrosTipo1          = line.Substring(20, 8).ToInt();
            Relleno2                        = line.Substring(28, 17);
            CUIT                            = line.Substring(45, 11).ToLong();
            Relleno3                        = line.Substring(56, 22);
            ImporteTotal                    = line.Substring(78, 15).ToInt() * 0.01;
            ImporteTotalSinNetoGravado      = line.Substring(93, 15).ToInt() * 0.01;
            ImporteNetoGravado              = line.Substring(108, 15).ToInt() * 0.01;
            ImpuestoLiquidado               = line.Substring(123, 15).ToInt() * 0.01;
            ImpuestoLiquidadoNoCategorizado = line.Substring(138, 15).ToInt() * 0.01;
            ImporteOperacionesExentas       = line.Substring(153, 15).ToInt() * 0.01;
            ImportePagosCuentaImpuestos     = line.Substring(168, 15).ToInt() * 0.01;
            ImporteIIBB                     = line.Substring(183, 15).ToInt() * 0.01;
            ImporteIngresosMunicipales      = line.Substring(198, 15).ToInt() * 0.01;
            ImporteImpuestosInternos        = line.Substring(213, 15).ToInt() * 0.01;
            Relleno4                        = line.Substring(228, 62);
        }
    }
}