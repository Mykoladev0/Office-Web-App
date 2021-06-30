using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDAL.Models;

namespace CoreDAL.Interfaces
{
    public interface IBreedService
    {
        Task<Breeds> GetBreedByNameAsync(string name);
        Task<ICollection<Breeds>> GetBreedsAsync();
    }
}