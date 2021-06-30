using CoreDAL.Interfaces;
using CoreDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Services
{
    public class JudgeService : IJudgeService
    {
        private readonly ABKCOnlineContext _context;

        public JudgeService(ABKCOnlineContext context)
        {
            _context = context;
        }


        public async Task<Judges> FindByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            //begins with comparision on name
            String[] names = name.Split(' ');
            IQueryable<Judges> q = _context.Judges;
            if (names.Length > 1)
            {
                q = q.Where(j => j.FirstName.ToLower() == names[0].ToLower() && j.LastName.ToLower().StartsWith(names[1].ToLower()));
                if (q.Count() > 1)
                {
                    //find exact match if possible, otherwise return null
                    return await q.Where(j => j.LastName.ToLower() == names[1].ToLower()).FirstOrDefaultAsync();
                }

            }
            else
            {
                q = q.Where(j => j.LastName.ToLower().StartsWith(names[0].ToLower()));
                if (q.Count() > 1)
                {
                    //find exact match if possible, otherwise return null
                    return await q.Where(j => j.LastName.ToLower() == names[0].ToLower()).FirstOrDefaultAsync();
                }

            }
            return await q.FirstOrDefaultAsync();
        }

        public async Task<Judges> GetById(int id)
        {
            return await _context.Judges.FindAsync(id);
        }
    }
}
