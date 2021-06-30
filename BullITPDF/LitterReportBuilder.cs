using System;
using PdfSharpCore.Drawing;
using ABKCCommon.Models.DTOs.Pedigree;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Pdf.IO;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using System.Linq;

namespace BullITPDF
{
    public class LitterReportBuilder : BasePDFBuilder
    {
        const double WIDTH = 21.59;
        const double HEIGHT = 27.94;
        private bool _buildWithBackground;
        private LitterReportDTO _litterReport;
        public LitterReportBuilder(FontResolver fontResolver, bool swapPages = true) :
            base(fontResolver, WIDTH, HEIGHT, swapPages)
        {

        }
        private void DrawHeaderTemplate(XGraphics gfx, double top = 3.5)
        {
            this.AddStringToPDF("SireName: ", gfx, 0.9, top, 11);
            this.AddStringToPDF("SireABKC#: ", gfx, 0.7, top + 0.4, 11);
            this.AddStringToPDF("Birthdate: ", gfx, 1, top + 0.8, 11);
            this.AddStringToPDF("DamName: ", gfx, 12.4, top, 11);
            this.AddStringToPDF("DamABKC#: ", gfx, 12.4, top + 0.4, 11);
            this.AddStringToPDF("Breed: ", gfx, 13, top + 0.8, 11);
            this.AddStringToPDF(_litterReport.SireName, gfx, 2.7, top, 11);
            this.AddStringToPDF(_litterReport.SireABKCNumber.ToString(), gfx, 2.7, top + 0.4, 11);
            this.AddStringToPDF(_litterReport.Birthdate.ToString("d"), gfx, 2.7, top + 0.8, 11);
            this.AddStringToPDF(_litterReport.DamName, gfx, 14.7, top, 11);
            this.AddStringToPDF(_litterReport.DamABKCNumber.ToString(), gfx, 14.7, top + 0.4, 11);
            this.AddStringToPDF(_litterReport.Breed, gfx, 14.7, top + 0.8, 11);
        }
        private void AddPuppyInformation(LitterReportPuppyInformationDTO _puppyInfo, XGraphics gfx, double left, double top, int fontSize)
        {
            this.AddStringToPDF("Puppy Name: ", gfx, left - 1, top, fontSize);
            this.AddStringToPDF("ABKC #: ", gfx, left + 11.7, top, fontSize);
            this.AddStringToPDF("Gender: ", gfx, left + 16.2, top, fontSize);
            this.AddStringToPDF("Sold to: ", gfx, left, top + 0.7, fontSize);
            this.AddStringToPDF("Address: ", gfx, left - 0.2, top + 1.2, fontSize);
            this.AddStringToPDF("City: ", gfx, left + 8.8, top + 1.2, fontSize);
            this.AddStringToPDF("ST/Zip: ", gfx, left + 14.8, top + 1.2, fontSize);
            this.AddStringToPDF(_puppyInfo.PuppyName, gfx, left + 1.5, top, fontSize);
            this.AddStringToPDF(_puppyInfo.ABKCNumber.ToString(), gfx, left + 13, top, fontSize);
            this.AddStringToPDF(_puppyInfo.Sex, gfx, left + 17.5, top, fontSize);
            this.AddStringToPDF(_puppyInfo.SoldTo, gfx, left + 1.2, top + 0.7, fontSize);
            this.AddStringToPDF(_puppyInfo.Address, gfx, left + 1.2, top + 1.2, fontSize);
            this.AddStringToPDF(_puppyInfo.City, gfx, left + 9.8, top + 1.2, fontSize);
            this.AddStringToPDF(_puppyInfo.STZip, gfx, left + 16, top + 1.2, fontSize);
        }
        private void DrawPage()
        {
            double startTop;
            var left = 1.4;
            var fontSize = 11;
            var numberOfPages = _litterReport.PuppiesInformation.Count / 8;
            for (var i = 0; i < numberOfPages + 1; i++)
            {
                var gfx = this.CreateNextPage(_buildWithBackground);
                if (i == 0)
                {
                    ImageSource.ImageSourceImpl = new PdfSharpCore.Utils.ImageSharpImageSource<SixLabors.ImageSharp.PixelFormats.Rgba32>();
                    var logo = XImage.FromStream(() => EmbeddedResource.GetResource("logo.png"));
                    gfx.DrawImage(logo, XUnit.FromCentimeter(1), XUnit.FromCentimeter(0.5), XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(2.5));
                    startTop = 5.8;
                    this.DrawHeaderTemplate(gfx);
                    this.AddStringToPDF("Litter Record for Litter #  " + _litterReport.LitterNumber, gfx, 0, 1.5, WIDTH, 5, 15);
                    this.AddStringToPDF(_litterReport.ReportGenerationDate.ToString("d"), gfx, 18, 1.5, 11);
                    var line = new XPen(XColors.Black, XUnit.FromMillimeter(0.2));
                    gfx.DrawLine(line, 1, XUnit.FromCentimeter(5), XUnit.FromCentimeter(_pageSize.Width - 1), XUnit.FromCentimeter(5));
                }
                else
                {
                    this.DrawHeaderTemplate(gfx, 1.5);
                    startTop = 4;
                    var line = new XPen(XColors.Black, XUnit.FromMillimeter(0.2));
                    gfx.DrawLine(line, 1, XUnit.FromCentimeter(2.7), XUnit.FromCentimeter(_pageSize.Width - 1), XUnit.FromCentimeter(2.7));
                }
                var puppiesAsArray = _litterReport.PuppiesInformation.ToArray();
                for (var j = i * 8; j < (i + 1) * 8 && j < _litterReport.PuppiesInformation.Count; j++)
                {
                    this.AddPuppyInformation(puppiesAsArray[i], gfx, left, startTop + (j - i * 8) * 2.5, fontSize);
                }
            }
        }
        public async Task BuildPDF(Stream stream, LitterReportDTO litterReport, bool buildWithBackground)
        {
            _litterReport = litterReport;
            _buildWithBackground = buildWithBackground;
            if (buildWithBackground)
            {
                Stream backgroundStream = EmbeddedResource.GetResource("LitterReport.pdf");
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