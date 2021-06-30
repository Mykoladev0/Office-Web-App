using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AmericanBullyClub.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using ABKCCommon.Models.DTOs.Pedigree;
using BullITPDF;

namespace AmericanBullyClub.Web
{
    public class LitterReportModel : PageModel
    {
        [BindProperty]
        public LitterReportDTO LitterReport { get; set; }
        [BindProperty]
        public bool PrintWithBackground { get; set; }
        private IHostingEnvironment _env;
        private FontResolver _fontResolver;
        public LitterReportModel(IHostingEnvironment env)
        {
            _env = env;
        }
        public IActionResult OnPost()
        {
            _fontResolver = new FontResolver();
            LitterReport.PuppiesInformation = new List<LitterReportPuppyInformationDTO>();
            for (var i = 0; i < 19; i++)
            {
                LitterReport.PuppiesInformation.Add(new LitterReportPuppyInformationDTO
                {
                    PuppyName = "John Snow" + i,
                    ABKCNumber = $"{(i + 35692):n0}",
                    Sex = "Male",
                    SoldTo = "Daenerys Stormborn of the House Targaryen, First of Her Name, the Unburnt, Queen of the Andals and the First Men, Khaleesi of the Great Grass Sea, Breaker of Chains, and Mother of Dragons",
                    Address = "House of Stark",
                    City = "Winterfell",
                    STZip = "0076"
                });
            }
            LitterReportBuilder builder = new LitterReportBuilder(_fontResolver);
            const string filename = "LitterReport.pdf";
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot", filename);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                builder.BuildPDF(stream, LitterReport, PrintWithBackground);
                stream.Flush();
                stream.Close();
            }
            return Redirect(filename);
        }
        public void OnGet()
        {
            LitterReport = new LitterReportDTO();
            LitterReport.LitterNumber = 234567;
            LitterReport.SireName = "Rhaegar Targaryen";
            LitterReport.DamName = "Lina Stark";
            LitterReport.Breed = "Stark";
        }
    }
}