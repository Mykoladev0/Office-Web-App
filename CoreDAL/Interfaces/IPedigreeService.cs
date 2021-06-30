using System.Threading.Tasks;
using ABKCCommon.Models.DTOs.Pedigree;

namespace CoreDAL.Interfaces
{
    public interface IPedigreeService
    {
        Task<PedigreeDTO> GeneratePedigreeData(int dogId, bool useOldSystem = false, int pedigreeDepth = -1);
        Task<PedigreeDTO> GeneratePedigreeDataFromABKCNo(string abkcNumber, bool useOldSystem = false, int pedigreeDepth = -1);
    }
}