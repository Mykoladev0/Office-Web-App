using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class ChmaionshipCertificateModel : PageModel
    {
        [BindProperty]
        public PedigreeDTO PedigreeInfo {get; set;}
        [BindProperty]
        public bool PrintWithBackground {get;set;}
        private IHostingEnvironment _env;
        private FontResolver _fontResolver;
        public ChmaionshipCertificateModel(IHostingEnvironment env)
        {
            _env = env;
        }
        public IActionResult OnPost()
        {
            _fontResolver = new FontResolver();
            ChempionshipCertificateBuilder builder = new ChempionshipCertificateBuilder(_fontResolver);
            const string filename = "ChampionshipCertificate.pdf";
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot", filename);
            if(System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            using (var stream = System.IO.File.OpenWrite(filePath)){
                 builder.BuildPDF(stream,PedigreeInfo,PrintWithBackground);
                 stream.Flush();
                 stream.Close();
            }
            return Redirect(filename);
        }
        public void OnGet()
        {
            PedigreeDTO pedigree = JsonConvert.DeserializeObject<PedigreeDTO>(System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "wwwroot", "response_1550849757856.json")));
            PedigreeInfo = pedigree;
        }
    }
}