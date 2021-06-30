
using ABKCCommon.Models.DTOs;
using AutoMapper;
using CoreApp.Models;
using CoreDAL.Models.v2.Registrations;

public class RegistrationDisplayMapping : Profile
{
    public RegistrationDisplayMapping()
    {
        CreateMap<IRegistration, RegistrationListItemDTO>()
            .ForMember(dest => dest.DogInfo, opt => opt.MapFrom(src => src.PrimaryDogInfo))
            .ForMember(dest => dest.RegistrationStatus, opt => opt.MapFrom(src => src.CurStatus))
            .ForMember(dest => dest.DateSubmitted, opt => opt.MapFrom(src => src.DateSubmitted))
            .ForMember(dest => dest.RegistrationType, opt => opt.MapFrom(src => src.RegistrationType))
            .ForMember(dest => dest.SubmittedBy, opt => opt.MapFrom(src => src.SubmittedBy))
            .ForMember(dest => dest.IsInternational, opt => opt.MapFrom(src => src.IsInternationalRegistration));

        // CreateMap<IRegistration, RegistrationResultDTO>()
        //     .ForMember(dest => dest., opt => opt.MapFrom(src => src.PrimaryDogInfo))
        //     .ForMember(dest => dest.RegistrationStatus, opt => opt.MapFrom(src => src.CurStatus))
        //     .ForMember(dest => dest.DateSubmitted, opt => opt.MapFrom(src => src.DateSubmitted))
        //     .ForMember(dest => dest.RegistrationType, opt => opt.MapFrom(src => src.RegistrationType))
        //     .ForMember(dest => dest.SubmittedBy, opt => opt.MapFrom(src => src.SubmittedBy))
        //     .ForMember(dest => dest.IsInternational, opt => opt.MapFrom(src => src.IsInternationalRegistration));


        CreateMap<PuppyRegistrationModel, PuppyRegistrationDisplayDTO>()
            // .ForMember(dest => dest.PuppyABKCNumber, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo.ABKCNumber : ""))
            .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
            .ForMember(dest => dest.SellDate, opts => opts.MapFrom(src => src.DateOfSale))
            // .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
            .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
            .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
            .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));
        CreateMap<LitterRegistrationModel, LitterRegistrationDisplayDTO>()
            // .ForMember(dest => dest.PuppyABKCNumber, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo.ABKCNumber : ""))
            .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
            // .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
            .ForMember(dest => dest.SireInfo, opts => opts.MapFrom(src => src.Sire))
            .ForMember(dest => dest.DamInfo, opts => opts.MapFrom(src => src.Dam))

            .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
            .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
            .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));

        CreateMap<RegistrationModel, PedigreeRegistrationDisplayDTO>()
            // .ForMember(dest => dest.DogInfo, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo.ABKCNumber : ""))
            .ForMember(dest => dest.Owner, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo.Owner : null))
            .ForMember(dest => dest.CoOwner, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo.CoOwner : null))
            .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
            // .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
            .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
            .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
            .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));
        CreateMap<BullyIdRequestModel, BullyIdRequestDisplayDTO>()
            .ForMember(dest => dest.DogInfo, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo : null));

        CreateMap<JuniorHandlerRegistrationModel, CoreDAL.Models.DTOs.JuniorHandlerRegistrationDTO>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
            // .ForMember(dest => dest.PuppyABKCNumber, opts => opts.MapFrom(src => src.DogInfo != null ? src.DogInfo.ABKCNumber : ""))
            .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
            // .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
            .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
            .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
            .ForMember(dest => dest.CertificateNumber, opts => opts.Ignore())
            .ForMember(dest => dest.IsInternational, opts => opts.MapFrom(src => src.IsInternationalRegistration))
            .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));

        // CreateMap<RegistrationModel, RegistrationListItemDTO>()
        //     .ForMember(dest => dest.RegistrationStatus, opts => opts.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.Status.ToString() : "UNKNOWN"))
        //     .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.Pedigree))
        //     .ForMember(dest => dest.OvernightRequested, opts => opts.MapFrom(src => src.OvernightRequested))
        //     .ForMember(dest => dest.RushRequested, opts => opts.MapFrom(src => src.RushRequested))
        //     .ForMember(dest => dest.IsInternational, opts => opts.MapFrom(src => src.IsInternationalRegistration))
        //     .ForMember(dest => dest.DogInfo, opts => opts.MapFrom(src => src.DogInfo))
        //     .ForMember(dest => dest.SubmittedBy, opts => opts.MapFrom(src => src.SubmittedBy));
        // // .ForMember(dest => dest.RegistrationType, opts => opts.MapFrom(src => RegistrationTypeEnum.JuniorHandler));

    }
}