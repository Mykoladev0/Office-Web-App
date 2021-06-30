using CoreDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Interfaces
{
    public interface IJudgeService
    {
        Task<Judges> GetById(int id);
        Task<Judges> FindByName(string name);
    }
}
