using System;
using System.IO;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using ABKCCommon.Models.DTOs.Pedigree;

namespace BullITPDF
{
    public class ABKCBuilder
    {
        private readonly FontResolver _fontResolver;

        public ABKCBuilder()
        {
            _fontResolver = new FontResolver();
        }
        /// <summary>
        /// Given a pedigree tree, builds a PDF
        /// </summary>
        /// <param name="pedigree">dog pedigree</param>
        /// <param name="buildWithBackground">include background graphics</param>
        /// <param name="swapPages">if true, pedigree tree is page 1</param>
        /// <returns></returns>
        public async Task<Stream> GeneratePedigree(PedigreeDTO pedigree, bool buildWithBackground = true, bool swapPages = true)
        {
            try
            {
                Stream stream = new MemoryStream();
                var pedigreeBuilder = new PedigreeBuilder(_fontResolver, swapPages);
                await pedigreeBuilder.BuildPDF(stream, pedigree, buildWithBackground);
                return stream;
            }
            catch (System.Exception x)
            {

                throw;
            }
        }

        public async Task<Stream> GenerateLitterReport(LitterReportDTO litter, bool includeBackground)
        {
            try
            {
                Stream stream = new MemoryStream();
                LitterReportBuilder builder = new LitterReportBuilder(_fontResolver, true);
                await builder.BuildPDF(stream, litter, includeBackground);
                return stream;
            }
            catch (System.Exception x)
            {

                throw;
            }
        }
        public async Task<Stream> GeneratePermanentRegistration(DogInfoDTO dog, bool includeBackground)
        {
            try
            {
                Stream stream = new MemoryStream();
                PermanentRegistrationBuilder builder = new PermanentRegistrationBuilder(_fontResolver, true);
                await builder.BuildPDF(stream, dog, includeBackground);
                return stream;
            }
            catch (System.Exception x)
            {

                throw;
            }
        }
        public async Task<Stream> GenerateJrHandlerCertificate(JuniorHandlerDTO jrHandler, bool includeBackground)
        {
            try
            {
                Stream stream = new MemoryStream();
                JuniorHandlerCertificateBuilder builder = new JuniorHandlerCertificateBuilder(_fontResolver, true);
                await builder.BuildPDF(stream, jrHandler, includeBackground);
                return stream;
            }
            catch (System.Exception x)
            {

                throw;
            }
        }
    }
}
