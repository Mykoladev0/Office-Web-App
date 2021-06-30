using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoreApp.Models;
using CoreDAL.Models.v2;

namespace BullsBluffCore.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<UserModel, FullABKCUserDTO>()
            .ForMember(dest => dest.ABKCRolesUserBelongsTo,
                opts => opts.MapFrom(src => src.Roles != null ? src.Roles.Select(r => r.Type.ToString()) : new List<string>()));
            CreateMap<UserModel, RepresentativeDTO>();
        }


    }
}