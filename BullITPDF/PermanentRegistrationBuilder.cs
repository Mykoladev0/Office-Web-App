using System;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs.Pedigree;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf.IO;
using ABKCCommon.Models.DTOs;

namespace BullITPDF
{
    public class PermanentRegistrationBuilder : BasePDFBuilder
    {
        const double HEIGHT = 13.97;
        const double WIDTH = 21.59;
        private bool _buildWithBackground;
        private DogInfoDTO _dog;
        public PermanentRegistrationBuilder(FontResolver fontResolver, bool swapPages = true) :
            base(fontResolver, WIDTH, HEIGHT, swapPages)
        {
        }
        private void DrawPage()
        {
            DateTime generatedDate = DateTime.Now;
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_dog.Breed, gfx, 5.2, 3);
            this.AddStringToPDF(_dog.Sire?.DogName, gfx, 2.8, 4.6);
            this.AddStringToPDF(_dog.Sire?.ABKCNumber, gfx, 4, 5.8);
            this.AddStringToPDF(_dog.Dam?.DogName, gfx, 2.5, 7.2);
            this.AddStringToPDF(_dog.Dam?.ABKCNumber, gfx, 4, 8.4);
            this.AddStringToPDF(_dog.Owner?.FullName, gfx, 4, 10.2);
            this.AddStringToPDF(_dog.Owner?.Address1, gfx, 4, 10.7);
            this.AddStringToPDF(_dog.Owner?.Address2, gfx, 4, 11.2);
            this.AddStringToPDF(_dog.Owner?.Address3, gfx, 4, 11.7);
            this.AddStringToPDF(_dog.ABKCNumber, gfx, 16.1, 3);
            this.AddStringToPDF(_dog.DateOfBirth?.ToString("d"), gfx, 15.7, 3.6);
            this.AddStringToPDF(_dog.Gender, gfx, 19.5, 3.6);
            this.AddStringToPDF(_dog.Color, gfx, 15.2, 5.9);
            this.AddStringToPDF(_dog.MicrochipNumber, gfx, 15.8, 6.4);
            this.AddStringToPDF(AddOrdinalsToNumber(generatedDate.Day), gfx, 15.2, 11);
            this.AddStringToPDF(generatedDate.ToString("MMMM") + " , " + generatedDate.ToString("yyyy"), gfx, 17.2, 11, 11);
        }
        public async Task BuildPDF(Stream stream, DogInfoDTO dog, bool buildWithBackground = true)
        {
            _dog = dog;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                Stream backgroundStream = EmbeddedResource.GetResource("PuppyRegistrationBlank.pdf");
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