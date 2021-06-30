using System;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf.IO;

namespace BullITPDF
{
    public class JuniorHandlerCertificateBuilder:BasePDFBuilder
    {
        const double WIDTH = 19.2786;
        const double HEIGHT = 12.446;
        private bool _buildWithBackground;
        private JuniorHandlerDTO _juniorHandler;
        public JuniorHandlerCertificateBuilder(FontResolver fontResolver, bool swapPages = true):
        base(fontResolver,WIDTH,HEIGHT,swapPages)
            {
                
            }
        private void DrawJuniorHandlerPage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_juniorHandler.Name, gfx,2.5,2.9);
            this.AddStringToPDF(_juniorHandler.Birthdate.ToString("d"), gfx,3.5,4);
            this.AddStringToPDF(_juniorHandler.Id.ToString(), gfx,5.5,4.5);
            this.AddStringToPDF(_juniorHandler.RegisteredOwnerName, gfx,3,9);
            this.AddStringToPDF(_juniorHandler.Address1, gfx,3,9.5);
            this.AddStringToPDF(_juniorHandler.Address2, gfx,3,10);
            this.AddStringToPDF(_juniorHandler.Address3, gfx,3,10.5);
            this.AddStringToPDF(AddOrdinalsToNumber(_juniorHandler.CertificateGenerationDate.Day), gfx, 11.5, 9.3,9);
            this.AddStringToPDF(_juniorHandler.CertificateGenerationDate.ToString("MMMM") + " , " + _juniorHandler.CertificateGenerationDate.ToString("yyyy"), gfx, 13.3, 9.3,9);
        }
        public async Task BuildPDF(Stream stream, JuniorHandlerDTO juniorHandlerDTO, bool buildWithBackground = true)
        {
            _juniorHandler = juniorHandlerDTO;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                Stream backgroundStream = EmbeddedResource.GetResource("JuniorhandlerBlank.pdf");
                Document = PdfReader.Open(backgroundStream);
            }
            else
            {
                Document = new PdfDocument();
            }
            this.DrawJuniorHandlerPage();
            Document.Save(stream, false);
        }
    }
}