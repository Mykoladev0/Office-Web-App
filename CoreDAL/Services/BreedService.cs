using CoreDAL.Interfaces;
using CoreDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Services
{
    public class BreedService : IBreedService
    {
        private readonly ABKCOnlineContext _context;

        public BreedService(ABKCOnlineContext context) => _context = context;

        public async Task<ICollection<Breeds>> GetBreedsAsync() =>
            await _context.Breeds.OrderBy(b => b.Breed).ToListAsync();
        public async Task<Breeds> GetBreedByNameAsync(string name) =>
            await _context.Breeds.FirstOrDefaultAsync(b => b.Breed.ToLower() == name.ToLower());
    }
}
