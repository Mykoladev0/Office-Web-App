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
    public class StyleAndClassService : IStyleAndClassService
    {
        private readonly ABKCOnlineContext _context;

        public StyleAndClassService(ABKCOnlineContext context)
        {
            _context = context;
        }
        public async Task<ICollection<ClassTemplates>> GetClassTemplates()
        {
            ICollection<ClassTemplates> templates = await _context.ClassTemplates.ToListAsync();
            ICollection<Styles> styles = await GetStyles();
            foreach (var template in templates)
            {
                template.Style = styles.FirstOrDefault(s => s.Id == template.StyleId);
                template.Gender = template.Name.Contains("Male") ? "Male" : template.Name.Contains("Female") ? "Female" : "";
                template.Name = template.Name.Replace("Male", "").Replace("Female", "").Replace("()", "").Trim();
            }
            return templates.OrderBy(t => t.SortOrder).ToList();
        }

        public async Task<ICollection<Styles>> GetStyles()
        {
            return await _context.Styles.ToListAsync();
        }

        public async Task<ClassTemplates> GetClassById(int id)
        {
            //return await _context.ClassTemplates.Include(c => c.Style).FirstOrDefaultAsync(c=>c.ClassId == id);
            return await _context.ClassTemplates.FirstOrDefaultAsync(c => c.ClassId == id);
        }
        public async Task<Styles> GetStyleById(int id)
        {
            return await _context.Styles.FindAsync(id);
        }
    }
}
