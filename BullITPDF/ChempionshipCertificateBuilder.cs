using System;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs.Pedigree;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf.IO;

namespace BullITPDF
{
    public class ChempionshipCertificateBuilder:BasePDFBuilder
    {
        const double HEIGHT = 24.13;
        const double WIDTH = 35.56;
        private bool _buildWithBackground;
        private PedigreeDTO _pedigreeDTO;
        public ChempionshipCertificateBuilder(FontResolver fontResolver, bool swapPages = true):
            base(fontResolver,WIDTH,HEIGHT,swapPages)
        {
        }
        private void DrawFristPage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_pedigreeDTO.Name,gfx,0,12,WIDTH,5,20);
        }
        private void DrawSecondPage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_pedigreeDTO.RegisteredOwnerName,gfx,7.5,16.5,12);
            this.AddStringToPDF(_pedigreeDTO.Address1,gfx,7.5,17,12);
            this.AddStringToPDF(_pedigreeDTO.Address2,gfx,7.5,17.5,12);
            this.AddStringToPDF(_pedigreeDTO.Address3,gfx,7.5,18,12);
        }
        public async Task BuildPDF(Stream stream, PedigreeDTO pedigreeDTO, bool buildWithBackground = true)
        {
            _pedigreeDTO = pedigreeDTO;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                //_pdfPath = pdfPath;
                Stream backgroundStream = EmbeddedResource.GetResource("ChampionBlank.pdf");
                Document = PdfReader.Open(backgroundStream);
            }
            else
            {
                Document = new PdfDocument();
            }
            this.DrawFristPage();
            this.DrawSecondPage();
            Document.Save(stream, false);
        }
    }
}

        