using System.IO;
using PdfSharpCore;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs.Pedigree;
using System.Threading.Tasks;
using PdfSharpCore.Fonts;

namespace BullITPDF
{
    public class PedigreeBuilder : BasePDFBuilder
    {
        private PedigreeDTO _pedigreeDTO;
        private bool _buildWithBackground;
        private string _pdfPath;
        public PedigreeBuilder(FontResolver fontResolver, bool swapPages = true) : 
            base(fontResolver, 35.56, 24.13, swapPages)
        {

        }
        private void DrawFirstPage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_pedigreeDTO?.Name, gfx, 23, 11.4);
            this.AddStringToPDF(_pedigreeDTO?.Breed, gfx, 23, 12.4);
            this.AddStringToPDF(_pedigreeDTO?.ABKCNumber, gfx, 23, 13.4);
            this.AddStringToPDF(_pedigreeDTO?.Gender, gfx, 23, 14.4);
            this.AddStringToPDF(_pedigreeDTO?.DateOfBirth.ToString("d"), gfx, 23, 15.3);
            this.AddStringToPDF(_pedigreeDTO?.Color, gfx, 23, 16.2);
            this.AddStringToPDF(_pedigreeDTO?.MicrochipNumber, gfx, 23, 17.2);
            this.AddStringToPDF(_pedigreeDTO?.RegisteredOwnerName, gfx, 24, 18.1);
            this.AddStringToPDF(_pedigreeDTO?.Address1, gfx, 24, 18.5);
            this.AddStringToPDF(_pedigreeDTO?.Address2, gfx, 24, 19);
            this.AddStringToPDF(_pedigreeDTO?.Address3, gfx, 24, 19.5);
            this.AddStringToPDF(AddOrdinalsToNumber(_pedigreeDTO?.PedigreeGeneratedDate.Day), gfx, 30.5, 20.8);
            this.AddStringToPDF(_pedigreeDTO?.PedigreeGeneratedDate.ToString("MMMM") + " , " + _pedigreeDTO?.PedigreeGeneratedDate.ToString("yyyy"), gfx, 32.6, 20.8);
        }
        private void AddPedigreeTreeToPDF(PedigreeAncestorDTO pedigree, XGraphics gfx, double left, double top)
        {
            if (pedigree == null)
                return;
            this.AddStringToPDF(pedigree.Name, gfx, left - 1.4, top);
            this.AddStringToPDF(pedigree.ABKCNumber, gfx, left, top + 0.35);
            if (pedigree.NumberOfPups != 0)
                this.AddStringToPDF(pedigree.NumberOfPups.ToString(), gfx, left - 0.4, top + 0.6);
            this.AddStringToPDF(pedigree.Color, gfx, left + 3.3, top + 0.3);

            if (pedigree.Certifications != null)
                this.AddStringToPDF(string.Join(",", pedigree.Certifications), gfx, left + 4.6, top + 0.6);
        }
        private void DrawPedigreeTreePage()
        {
            var gfx = this.CreateNextPage(_buildWithBackground);
            this.AddStringToPDF(_pedigreeDTO?.Name, gfx, 2, 11.6);
            this.AddStringToPDF(_pedigreeDTO?.ABKCNumber, gfx, 4.2, 12);
            this.AddStringToPDF(_pedigreeDTO?.Gender, gfx, 2.4, 12.3);
            this.AddStringToPDF(_pedigreeDTO?.Color, gfx, 2.4, 12.6);
            this.AddStringToPDF(_pedigreeDTO?.DateOfBirth.ToString("d"), gfx, 3.2, 13);
            this.AddStringToPDF(_pedigreeDTO?.SireOwnerName, gfx, 1.6, 13.6);
            this.AddStringToPDF(_pedigreeDTO?.DamOwnerName, gfx, 1.6, 14.4);
            this.AddStringToPDF(_pedigreeDTO?.NumberOfPups.ToString(), gfx, 3, 14.65);
            if (_pedigreeDTO?.Certifications != null)
                this.AddStringToPDF(string.Join(",", _pedigreeDTO?.Certifications), gfx, 3.4, 14.9);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire, gfx, 5.6, 8.6);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam, gfx, 5.6, 17.1);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire, gfx, 12.4, 6.4);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam, gfx, 12.4, 10.7);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire, gfx, 12.4, 14.9);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam, gfx, 12.4, 19.2);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire?.Sire, gfx, 17.8, 5.4);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire?.Dam, gfx, 17.8, 7.5);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam?.Sire, gfx, 17.8, 9.6);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam?.Dam, gfx, 17.8, 11.8);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire?.Sire, gfx, 17.8, 13.85);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire?.Dam, gfx, 17.8, 16);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam?.Sire, gfx, 17.8, 18.1);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam?.Dam, gfx, 17.8, 20.25);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire?.Sire?.Sire, gfx, 27.6, 4.8);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire?.Sire?.Dam, gfx, 27.6, 5.95);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire?.Dam?.Sire, gfx, 27.6, 6.9);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Sire?.Dam?.Dam, gfx, 27.6, 8);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam?.Sire?.Sire, gfx, 27.6, 9.1);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam?.Sire?.Dam, gfx, 27.6, 10.15);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam?.Dam?.Sire, gfx, 27.6, 11.2);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Sire?.Dam?.Dam?.Dam, gfx, 27.6, 12.25);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire?.Sire?.Sire, gfx, 27.6, 13.35);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire?.Sire?.Dam, gfx, 27.6, 14.4);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire?.Dam?.Sire, gfx, 27.6, 15.5);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Sire?.Dam?.Dam, gfx, 27.6, 16.5);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam?.Sire?.Sire, gfx, 27.6, 17.6);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam?.Sire?.Dam, gfx, 27.6, 18.6);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam?.Dam?.Sire, gfx, 27.6, 19.7);
            this.AddPedigreeTreeToPDF(_pedigreeDTO?.Dam?.Dam?.Dam?.Dam, gfx, 27.6, 20.8);
            this.AddStringToPDF(AddOrdinalsToNumber(_pedigreeDTO?.PedigreeGeneratedDate.Day), gfx, 4, 21.2);
            this.AddStringToPDF(_pedigreeDTO?.PedigreeGeneratedDate.ToString("MMMM") + " , " + _pedigreeDTO?.PedigreeGeneratedDate.ToString("yyyy"), gfx, 5.8, 21.2);
        }
        public async Task BuildPDF(Stream stream, PedigreeDTO pedigreeDTO, bool buildWithBackground = true)
        {
            _pedigreeDTO = pedigreeDTO;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                Stream backgroundStream = EmbeddedResource.GetResource("PedigreeBlank.pdf");
                Document = PdfReader.Open(backgroundStream);
            }
            else
            {
                Document = new PdfDocument();
            }
            this.DrawFirstPage();
            this.DrawPedigreeTreePage();
            Document.Save(stream, false);
        }

    }
}