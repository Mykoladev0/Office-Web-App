using ABKCCommon.Models.DTOs;
using AutoMapper;
using CoreDAL.Models.v2;

namespace CoreDAL.Mappings
{
    public class ABKCUserMapping : Profile
    {

        public ABKCUserMapping()
        {
            CreateMap<UserModel, ABKCUserDTO>()
                .ForMember(r => r.Id, opt => opt.MapFrom(u => u.Id));
            CreateMap<RoleType, RoleDTO>()
                .ForMember(r => r.Name, opt => opt.MapFrom(role => role.Type.ToString()))
                .ForMember(r => r.RoleTypeId, opt => opt.MapFrom(role => role.Type));

        }
    }
}