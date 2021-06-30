using AutoMapper;
using CoreDAL.Models.DTOs;
using System.Linq;
using System.Collections.Generic;
using CoreDAL.Models.v2.Registrations;
using ABKCCommon.Models.DTOs;

namespace CoreDAL.Mappings
{
    public class RegistrationMapping : Profile
    {
        public RegistrationMapping()
        {
            // CreateMap<RegistrationModel, RegistrationDTO>()
            //  .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
            //     .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
            //     .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
            //     .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
            //     .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
            //     .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));
            // CreateMap<JuniorHandlerRegistrationModel, JuniorHandlerRegistrationDTO>();

            CreateMap<RegistrationModel, RegistrationListItemDTO>()
                .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
                .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
                .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
                .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
                .ForMember(dest => dest.IsInternational, opts => opts.MapFrom(src => src.IsInternationalRegistration))
                .ForMember(dest => dest.DogInfo, opts => opts.MapFrom(src => src.DogInfo))
                .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));
            // .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.JuniorHandler));

        }
    }
}