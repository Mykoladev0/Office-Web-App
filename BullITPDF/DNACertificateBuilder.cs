using System;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs.Pedigree;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf.IO;

namespace BullITPDF
{
    public class DNACertificateBuilder : BasePDFBuilder
    {
        const double WIDTH = 22.2504;
        const double HEIGHT = 14.6304;

        private bool _buildWithBackground;
        private DNACertificateDTO _dnaCertificate;
        public DNACertificateBuilder(FontResolver fontResolver, bool swapPages = true) : 
            base(fontResolver, WIDTH, HEIGHT, swapPages)
        {
        }
        public void DrawPage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_dnaCertificate.Name,gfx,3.5,3.5);
            this.AddStringToPDF(_dnaCertificate.Breed,gfx,3,4.8);
            this.AddStringToPDF(_dnaCertificate.Gender,gfx,3,5.4);
            this.AddStringToPDF(_dnaCertificate.DateOfBirth.ToString("d"),gfx,3.6,6);
            this.AddStringToPDF(_dnaCertificate.ABKCNumber,gfx,3.5,6.5);
            this.AddStringToPDF(_dnaCertificate.DateOfAnalysis.ToString("d"),gfx,5.2,7.2);
            this.AddStringToPDF(_dnaCertificate.RegisteredOwnerName,gfx,4,7.8);
            this.AddStringToPDF(_dnaCertificate.Address1,gfx,4,8.3);
            this.AddStringToPDF(_dnaCertificate.Address2,gfx,4,8.8);
            this.AddStringToPDF(_dnaCertificate.Address3,gfx,4,9.3);
            this.AddStringToPDF(AddOrdinalsToNumber(_dnaCertificate.CertificateGenerationDate.Day), gfx, 13.5, 11);
            this.AddStringToPDF(_dnaCertificate.CertificateGenerationDate.ToString("MMMM") + " , " + _dnaCertificate.CertificateGenerationDate.ToString("yyyy"), gfx, 15.3, 11);
        }
        public async Task BuildPDF(Stream stream, DNACertificateDTO dNACertificate, bool buildWithBackground = true)
        {
            _dnaCertificate = dNACertificate;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                Stream backgroundStream = EmbeddedResource.GetResource("DNACertificateBlank.pdf");
                Document = PdfReader.Open(backgroundStream);
            }
            else
            {
                Document = new PdfDocument();
            }
            this.DrawPage();
            Document.Save(stream, false);
        }
    }
}