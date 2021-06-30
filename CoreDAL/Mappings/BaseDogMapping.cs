using AutoMapper;
using CoreDAL.Models.v2;
using CoreDAL.Models.DTOs;
using System.Linq;
using System.Collections.Generic;
using System;
using static CoreDAL.Models.v2.BaseDogModel;
using CoreDAL.Models;
using ABKCCommon.Models.DTOs;

namespace CoreDAL.Mappings
{
    public class BaseDogMapping : Profile
    {
        public BaseDogMapping()
        {
            CreateMap<BaseDogModel, BaseDogDTO>()
                 .ForMember(dest => dest.OwnerId,
                    opts => opts.MapFrom(src => src.Owner != null ? src.Owner.Id : -1
                 ))
                 .ForMember(dest => dest.CoOwnerId,
                    opts => opts.MapFrom(src => src.CoOwner != null ? src.CoOwner.Id : -1
                 ))
                 .ForMember(dest => dest.BreedId,
                    opts => opts.MapFrom(src => src.Breed != null ? src.Breed.Id : -1
                 ))
                .ForMember(dest => dest.SireId,
                    opts => opts.MapFrom(src => src.Sire != null ? src.Sire.Id : -1
                 ))
                .ForMember(dest => dest.DamId,
                    opts => opts.MapFrom(src => src.Dam != null ? src.Dam.Id : -1
                 ))
                .ForMember(dest => dest.ColorId,
                    opts => opts.MapFrom(src => src.Color != null ? src.Color.Id : -1
                 ))
                .ForMember(dest => dest.Gender,
                    opts => opts.MapFrom(src => src.Gender.ToString()
                 ))
                 .ReverseMap()
                 .ForMember(x => x.Owner, x => x.Ignore())
                 .ForMember(x => x.CoOwner, x => x.Ignore())
                 .ForMember(x => x.CoOwner, x => x.Ignore())
                 .ForMember(x => x.Breed, x => x.Ignore())
                 .ForMember(x => x.Color, x => x.Ignore())
                 .ForMember(x => x.Sire, x => x.Ignore())
                 .ForMember(x => x.Dam, x => x.Ignore())
                 // .ForMember(x => x.Sire, map =>
                 //     map.MapFrom((from, dog) =>
                 //     {
                 //         if (from.SireId == null || from.SireId < 1)
                 //         {
                 //             return null;
                 //         }
                 //         return Mapper.Map<BaseDogModel>(from);
                 //     }))
                 // .ForMember(x => x.Dam, map =>
                 //     map.MapFrom((from, dog) =>
                 //     {
                 //         if (from.DamId == null || from.DamId < 1)
                 //         {
                 //             dog.Dam = null;
                 //         }
                 //         else
                 //         {
                 //             dog.Dam = Mapper.Map<BaseDogModel>(from);
                 //         }
                 //         return dog.Dam;
                 //     }))
                 .ForMember(x => x.Gender, map =>
                       map.MapFrom((from, dog) =>
                       {
                           if (Enum.TryParse<GenderEnum>(from.Gender, true, out GenderEnum result))
                           {
                               dog.Gender = result;
                           }
                           else
                           {
                               dog.Gender = GenderEnum.Unknown;
                           }
                           return dog.Gender;
                       })
                   );
            // .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //mapping between new dog structure and old school format UGGH!
            CreateMap<BaseDogModel, Dogs>()
                .ForMember(dest => dest.OwnerId,
                    opts => opts.MapFrom(src => src.Owner != null ? src.Owner.Id : -1
                ))
                .ForMember(dest => dest.CoOwnerId,
                    opts => opts.MapFrom(src => src.CoOwner != null ? src.CoOwner.Id : -1
                ))
                .ForMember(dest => dest.Breed,
                    opts => opts.MapFrom(src => src.Breed != null ? src.Breed.Breed : "NOT FOUND"
                ))
                .ForMember(dest => dest.SireNo,
                    opts => opts.Ignore())//will do it manually

                //    .ForMember(dest => dest.SireNo,
                //        opts => opts.MapFrom(src => src.Sire != null ? src.Sire.Id : -1
                //     ))
                .ForMember(dest => dest.DamNo,
                    opts => opts.Ignore())//will do it manually
                                          //    .ForMember(dest => dest.DamId,
                                          //        opts => opts.MapFrom(src => src.Dam != null ? src.Dam.Id : -1
                                          //     ))
                .ForMember(dest => dest.Color,
                    opts => opts.MapFrom(src => src.Color != null ? src.Color.Color : "NOT FOUND"
                ))
                .ForMember(dest => dest.Gender,
                    opts => opts.MapFrom(src => src.Gender.ToString()
                ))
                .ForMember(dest => dest.Birthdate, opts => opts.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.AbkcNo, opts => opts.Ignore())

                .ReverseMap()
                .ForMember(x => x.Owner, opts => opts.MapFrom((src, dog) => { dog.Owner = src.OwnerId > 0 ? new Owners { Id = src.OwnerId } : null; return dog.Owner; }))
                .ForMember(x => x.CoOwner, opts => opts.MapFrom((src, dog) => { dog.CoOwner = src.CoOwnerId.HasValue && src.CoOwnerId > 0 ? new Owners { Id = src.CoOwnerId.Value } : null; return dog.CoOwner; }))
                .ForMember(x => x.Breed, x => x.Ignore())
                .ForMember(x => x.Color, x => x.Ignore())
                .ForMember(x => x.OriginalDogTableId, opts => opts.MapFrom(d => d.Id))
                .ForMember(x => x.Gender, map =>
                        map.MapFrom((from, dog) =>
                        {
                            if (Enum.TryParse<GenderEnum>(from.Gender, true, out GenderEnum result))
                            {
                                dog.Gender = result;
                            }
                            else
                            {
                                dog.Gender = GenderEnum.Unknown;
                            }
                            return dog.Gender;
                        })
                    );

            //mapping to Display DTO

            CreateMap<BaseDogModel, DogInfoDTO>()
                           .ForMember(dest => dest.Breed,
                              opts => opts.MapFrom(src => src.Breed != null ? src.Breed.Breed : ""
                           ))
                          .ForMember(dest => dest.Color,
                              opts => opts.MapFrom(src => src.Color != null ? src.Color.Color : ""
                           ))
                          .ForMember(dest => dest.DateOfBirth,
                              opts => opts.MapFrom(src => src.DateOfBirth != DateTime.MinValue ? src.DateOfBirth.ToShortDateString() : ""
                           ))
                          .ForMember(dest => dest.Gender,
                              opts => opts.MapFrom(src => src.Gender.ToString()
                           ));
            //    .ForMember(x => x.Owner, map =>
            //        map.MapFrom((from, dog) =>
            //        {
            //            if (from.Owner == null)
            //            {
            //                return null;
            //            }
            //            return Mapper.Map<OwnerDTO>(from);
            //        }))
            //     .ForMember(x => x.CoOwner, map =>
            //        map.MapFrom((from, dog) =>
            //        {
            //            if (from.CoOwner == null)
            //            {
            //                return null;
            //            }
            //            return Mapper.Map<OwnerDTO>(from);
            //        }))
            //    .ForMember(x => x.Sire, map =>
            //        map.MapFrom((from, dog) =>
            //        {
            //            if (from.Sire == null)
            //            {
            //                return null;
            //            }
            //            return Mapper.Map<DogInfoDTO>(from);
            //        }))
            //    .ForMember(x => x.Dam, map =>
            //        map.MapFrom((from, dog) =>
            //        {
            //            if (from.Dam == null)
            //            {
            //                dog.Dam = null;
            //            }
            //            else
            //            {
            //                dog.Dam = Mapper.Map<DogInfoDTO>(from);
            //            }
            //            return dog.Dam;
            //        }));

            CreateMap<Dogs, ABKCDogDTO>()
                .ForMember(dest => dest.Id,
                    opts => opts.UseDestinationValue())//want to force originaltable id usage everywhere
                .ForMember(dest => dest.OriginalTableId,
                    opts => opts.MapFrom(src => src.Id
                 ))
                .ForMember(dest => dest.ABKCNumber,
                    opts => opts.MapFrom(src => src.AbkcNo
                 ))
                 .ForMember(dest => dest.OwnerId,
                    opts => opts.MapFrom(src => src.OwnerId != 1 ? src.OwnerId : 0
                 ))
                 .ForMember(dest => dest.CoOwnerId,
                    opts => opts.MapFrom(src => src.CoOwnerId != 1 ? src.CoOwnerId : 0
                 ))
                 .ForMember(dest => dest.LitterId,
                    opts => opts.MapFrom(src => src.LitterNo
                 ))
                .ForMember(dest => dest.SireId,
                    opts => opts.MapFrom(src => src.SireNo != 1 ? src.SireNo : 0
                 ))
                .ForMember(dest => dest.DamId,
                    opts => opts.MapFrom(src => src.DamNo != 1 ? src.DamNo : 0
                 ))
                .ForMember(dest => dest.Gender,
                    opts => opts.MapFrom(src => src.Gender)
                 );

        }
    }
}