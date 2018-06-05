using System.Collections.Generic;

namespace FE2PDF
{
    public class Header
    {
        public string NombreComprobante { get; set; }
        public string TipoComprobante   { get; set; }
        public string CondicionIVA      { get; set; }
        public string CentroEmisor      { get; set; }
        public string NumeroComprobante { get; set; }
        public string FechaEmision      { get; set; }
        public string Detalle1          { get; set; }
        public string Detalle2          { get; set; }
        public string Domicilio         { get; set; }
        public string Localidad         { get; set; }
        public string Barrio            { get; set; }
        public string CodigoPostal      { get; set; }
        public string CUILCUIT          { get; set; }
        public string FechaVencimiento  { get; set; }
        public string NumeroCAE         { get; set; }
        public string CondicionPago     { get; set; }
        public string Importe           { get; set; }
        public string MontoIVA          { get; set; }
        public string MontoGravado      { get; set; }
        public string MontoNoGravado    { get; set; }
        public string Subtotal          { get; set; }
        public string CodigoBarra       { get; set; }
        public string RefPagoMisCuentas { get; set; }
        public string RefRedLink        { get; set; }
        public string Email             { get; set; }
        public List<Detail> Items       { get; set; } = new List<Detail>();
    }

    public class Detail
    {
        public string Detalle { get; set; }
        public string Monto   { get; set; }
    }
}