using CoreDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Interfaces
{
    public interface IStyleAndClassService
    {
        Task<ICollection<Styles>> GetStyles();
        Task<ICollection<ClassTemplates>> GetClassTemplates();

        Task<ClassTemplates> GetClassById(int id);
        Task<Styles> GetStyleById(int id);
    }
}
