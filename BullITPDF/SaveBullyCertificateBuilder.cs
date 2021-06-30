using System;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs.Pedigree;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf.IO;

namespace BullITPDF
{
    public class SaveBullyCertificateBuilder : BasePDFBuilder
    {
        private bool _buildWithBackground;
        const double WIDTH =  21.59;
        const double HEIGHT = 13.97;
        private SaveABullyDTO _saveABully;
        public SaveBullyCertificateBuilder(FontResolver fontResolver, bool swapPages = true) : 
            base(fontResolver, WIDTH, HEIGHT, swapPages)
        {
        }
        private void DrawPage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_saveABully.RescueDog,gfx,5.4,3.3);
            this.AddStringToPDF(_saveABully.Breed,gfx, 3,4.5);
            this.AddStringToPDF(_saveABully.Gender,gfx, 3,5.1);
            this.AddStringToPDF(_saveABully.Birthday.ToString("d"),gfx,4.5,5.7);
            this.AddStringToPDF(_saveABully.RescueDate.ToString("d"),gfx,4.5,6.3);
            this.AddStringToPDF(_saveABully.Color,gfx, 3,6.85);
            this.AddStringToPDF(_saveABully.OwnedBy,gfx,4,7.4);
            this.AddStringToPDF(_saveABully.RegisteredOwnerName,gfx,5.4,9.6);
            this.AddStringToPDF(_saveABully.Address1,gfx,5.4,10.1);
            this.AddStringToPDF(_saveABully.Address2,gfx,5.4,10.6);
            this.AddStringToPDF(_saveABully.Address3, gfx,5.4,11.1);
            this.AddStringToPDF(AddOrdinalsToNumber(_saveABully.CertificateGenerationDate.Day), gfx, 12.5, 10.6);
            this.AddStringToPDF(_saveABully.CertificateGenerationDate.ToString("MMMM") + " , " + _saveABully.CertificateGenerationDate.ToString("yyyy"), gfx, 15, 10.6);
        }
        public async Task BuildPDF(Stream stream, SaveABullyDTO saveABullyDTO, bool buildWithBackground = true)
        {
            _saveABully = saveABullyDTO;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                Stream backgroundStream = EmbeddedResource.GetResource("saveBullyCertificate.pdf");
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