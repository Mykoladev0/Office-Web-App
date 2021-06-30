using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoreDAL.Models;
using CoreDAL.Models.v2;
using ABKCCommon.Models.DTOs.Pedigree;
namespace CoreDAL.Mappings
{
    public class PedigreeMapping : Profile
    {
        private const string NOTAVAILSTR = "NOT AVAILABLE";
        public PedigreeMapping()
        {
            CreateMap<BaseDogModel, PedigreeDTO>()
                 .ForMember(dest => dest.DogId,
                    opts => opts.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Name,
                    opts => opts.MapFrom(src => src.DogName))
                 .ForMember(dest => dest.Breed,
                    opts => opts.MapFrom(src => src.Breed != null ? src.Breed.Breed : NOTAVAILSTR))
                 .ForMember(dest => dest.Color,
                    opts => opts.MapFrom(src => src.Color != null ? src.Color.Color : NOTAVAILSTR))
                .ForMember(dest => dest.DamOwnerName,
                    opts => opts.MapFrom(src => src.Dam != null ? BuildOwnerString(src.Dam.Owner, src.Dam.CoOwner) : NOTAVAILSTR))
                .ForMember(dest => dest.SireOwnerName,
                    opts => opts.MapFrom(src => src.Sire != null ? BuildOwnerString(src.Sire.Owner, src.Sire.CoOwner) : NOTAVAILSTR))
                .ForMember(dest => dest.ABKCNumber,
                    opts => opts.MapFrom(src => "NEED TO UPDATE MODEL"))
                .ForMember(dest => dest.Gender,
                    opts => opts.MapFrom(src => src.Gender.ToString()
                 ))
                .ForMember(dest => dest.Address1,
                    opts => opts.MapFrom(src => src.Owner != null ? $"{src.Owner.Address1}" : NOTAVAILSTR))
                .ForMember(dest => dest.Address2,
                    opts => opts.MapFrom(src => src.Owner != null ? $"{src.Owner.Address2}" : NOTAVAILSTR))
                .ForMember(dest => dest.NumberOfPups, opt => opt.Ignore())//will get entered after the fact
                .ForMember(dest => dest.Sire, opt => opt.Ignore())//will get entered after the fact
                .ForMember(dest => dest.Dam, opt => opt.Ignore());//will get entered after the fact

            CreateMap<BaseDogModel, PedigreeAncestorDTO>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.DogName))
                .ForMember(dest => dest.Color,
                    opts => opts.MapFrom(src => src.Color != null ? src.Color.Color : NOTAVAILSTR));

            CreateMap<Dogs, PedigreeDTO>()
                           .ForMember(dest => dest.DogId,
                              opts => opts.MapFrom(src => src.Id))
                           .ForMember(dest => dest.Name,
                              opts => opts.MapFrom(src => src.DogName))
                           .ForMember(dest => dest.Breed,
                              opts => opts.MapFrom(src => src.Breed))
                           .ForMember(dest => dest.Color,
                              opts => opts.MapFrom(src => src.Color))
                            .ForMember(dest => dest.DateOfBirth,
                              opts => opts.MapFrom(src => src.Birthdate))
                              .ForMember(dest => dest.MicrochipNumber,
                              opts => opts.MapFrom(src => src.ChipNo))
                            .ForMember(dest => dest.DamOwnerName, opt => opt.Ignore())
                            .ForMember(dest => dest.SireOwnerName, opt => opt.Ignore())
                            .ForMember(dest => dest.Address1, opt => opt.Ignore())
                            .ForMember(dest => dest.Address2, opt => opt.Ignore())
                            .ForMember(dest => dest.Address3, opt => opt.Ignore())

                          //       opts => opts.MapFrom(src => src.Dam != null ? BuildOwnerString(src.Dam.Owner, src.Dam.CoOwner) : NOTAVAILSTR))
                          //   .ForMember(dest => dest.SireOwnerName,
                          //       opts => opts.MapFrom(src => src.Sire != null ? BuildOwnerString(src.Sire.Owner, src.Sire.CoOwner) : NOTAVAILSTR))
                          .ForMember(dest => dest.ABKCNumber,
                              opts => opts.MapFrom(src => src.AbkcNo))
                          .ForMember(dest => dest.Gender,
                              opts => opts.MapFrom(src => src.Gender))
                          //   .ForMember(dest => dest.Address1,
                          //       opts => opts.MapFrom(src => src.Owner != null ? $"{src.Owner.Address1}" : NOTAVAILSTR))
                          //   .ForMember(dest => dest.Address2,
                          //       opts => opts.MapFrom(src => src.Owner != null ? $"{src.Owner.Address2}" : NOTAVAILSTR))
                          .ForMember(dest => dest.NumberOfPups, opt => opt.Ignore())//will get entered after the fact
                          .ForMember(dest => dest.Sire, opt => opt.Ignore())//will get entered after the fact
                          .ForMember(dest => dest.Dam, opt => opt.Ignore());//will get entered after the fact

            CreateMap<Dogs, PedigreeAncestorDTO>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.DogName))
                .ForMember(dest => dest.ABKCNumber, opts => opts.MapFrom(src => src.AbkcNo))
                .ForMember(dest => dest.Certifications, opts => opts.MapFrom(src => src.WpTitle))//needs to be calculated!
                .ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Color));

        }

        private string BuildOwnerString(Owners owner, Owners coOwner)
        {
            if (owner == null && coOwner == null)
            {
                return NOTAVAILSTR;
            }
            List<string> names = new List<string> { owner.FullName };
            if (coOwner != null)
            {
                names.Add(coOwner.FullName);
            }
            string str = string.Join("AND ", names);
            return str;
        }
    }

}