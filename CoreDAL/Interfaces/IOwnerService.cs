using ABKCCommon.Models.DTOs;
using CoreDAL.Models;
using CoreDAL.Models.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Interfaces
{
    public interface IOwnerService
    {
        Task<Owners> GetById(string id);
        Task<Owners> GetById(int id);
        IQueryable<Owners> GetOwnersQueryStartsWith(string searchText);
        Task<int> GetOwnersCount();
        IQueryable<Owners> GetOwnersQuery(bool changeTracking = true);

        Task<Owners> AddOwner(Owners ownerToAdd, UserModel modifiedBy);
        Task<Owners> UpdateOwner(Owners ownerToUpdate, UserModel modifiedBy);
        Task<Owners> UpdateFromDTO(Owners owner, OwnerDTO ownerEdits, bool saveChanges);
    }
}
