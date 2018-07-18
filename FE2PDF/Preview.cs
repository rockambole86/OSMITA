using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fyiReporting.RDL;

namespace FE2PDF
{
    public partial class Preview : Form
    {
        public Preview()
        {
            InitializeComponent();
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            var data = new List<Header>();

            data.Add(new Header
            {
                Barrio = "Barrio",
                CUILCUIT = "20-32676067-7",
                CentroEmisor = "Test",
                CodigoBarra = "0123456789",
                CodigoPostal = "1417",
                CondicionIVA = "A",
                CondicionPago = "Efectivo",
                Detalle1 = "Det 1",
                Detalle2 = "Det 2",
                Domicilio = "Allende 2879",
                Email = "enw1986@hotmail.com",
                FechaEmision = "01/01/2018",
                FechaVencimiento = "01/08/2018",
                Importe = "1000",
                Localidad = "CABA",
                MontoGravado = "1000",
                MontoIVA = "1000",
                MontoNoGravado = "1000",
                NombreComprobante = "FC",
                NumeroCAE = "ABC123456",
                NumeroComprobante = "123456",
                RefPagoMisCuentas = "123456",
                RefRedLink = "123456",
                Subtotal = "1000",
                TipoComprobante = "A",
                Items = new List<Detail>
                {
                    new Detail
                    {
                        Detalle = "Detalle 1",
                        MontoFacturado = "1000"
                    },
                    new Detail
                    {
                        Detalle = "Detalle 2",
                        MontoFacturado = "1000"
                    }
                }
            });

            rdlViewer1.SourceFile = new Uri(Path.Combine(Application.StartupPath, "FE2PDF.rdl"));

            foreach (var h in data)
            {
                var header  = new List<Header>{ h };
                var details = h.Items;

                rdlViewer1.Report.DataSets["Header"].SetData(header);
                rdlViewer1.Report.DataSets["Details"].SetData(details);

                var barcode = !string.IsNullOrEmpty(h.CodigoBarra) 
                    ? Int2of5.GenerateBarCode(h.CodigoBarra, 1000, 100, 10).ToBase64() 
                    : string.Empty;

                rdlViewer1.Parameters += $@"&bg_image={Path.Combine(Application.StartupPath, $"fc_{h.CondicionIVA.ToLower()}.jpg")}"; 
                rdlViewer1.Parameters += $@"&barcode={barcode}"; 

                rdlViewer1.Rebuild();

                //var pdfName    = $@"{h.TipoComprobante}{h.CondicionIVA}{h.CentroEmisor}{h.NumeroComprobante}.pdf";
                //var outputFile = Path.Combine($@"C:\Projects\Repositories\RDM\FE2PDF\Paths\Processed", pdfName);

                //rdlViewer1.SaveAs(outputFile, OutputPresentationType.PDF);
            }
        }
    }
}
