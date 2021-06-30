using ABKCCommon.Models.DTOs;
using AutoMapper;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;

namespace CoreDAL.Mappings
{
    public class OwnerMapping : Profile
    {
        public OwnerMapping()
        {
            CreateMap<Owners, OwnerDTO>().ReverseMap();
            //  .ForMember(dest => dest.Payload,
            //      opts => opts.MapFrom(
            //          src => src.ParticleLocationMessage.Location
            //      ))
            //  .ForMember(dest => dest.FirmwareVersion,
            //      opts => opts.MapFrom(src => src.ParticleLocationMessage.FirmwareVersion))
            //  .ForMember(dest => dest.CoreId,
            //      opts => opts.MapFrom(src => src.ParticleLocationMessage.CoreId))
            //  .ForMember(dest => dest.SerialNumber,
            //      opts => opts.MapFrom(src => src.LocatorSerialNumber))
            //  .ReverseMap();

            // CreateMap<HeartBeatPayloadModel, HeartbeatPayloadDTO>()
            //     .ForMember(dest => dest.Payload,
            //         opts => opts.MapFrom(
            //             src => src.HeartBeatMessage.HeartBeat
            //         ))
            //     .ForMember(dest => dest.FirmwareVersion,
            //         opts => opts.MapFrom(src => src.HeartBeatMessage.FirmwareVersion))
            //     .ForMember(dest => dest.CoreId,
            //         opts => opts.MapFrom(src => src.HeartBeatMessage.CoreId))
            //     .ForMember(dest => dest.SerialNumber,
            //         opts => opts.MapFrom(src => src.LocatorSerialNumber))
            //     .ReverseMap();

            // CreateMap<LocationPayloadModel, LocatorAzureTableModel>()
            //     .ForMember(dest => dest.WasTransmitted,
            //         opts => opts.MapFrom(
            //             src => src.DateTransmitted.HasValue
            //         ))
            //     .ForMember(dest => dest.CoreId,
            //         opts => opts.MapFrom(src => src.ParticleLocationMessage.CoreId.Trim()))
            //     .ForMember(dest => dest.PartitionKey,
            //         opts => opts.MapFrom(src => src.ParticleLocationMessage.CoreId.Trim()))
            //     .ForMember(dest => dest.RowKey,
            //         opts => opts.MapFrom(src => src.Id))//needs to be unique?
            //     .ForMember(dest => dest.PayloadMessage,
            //         opts => opts.MapFrom(src => src.PayloadString.Trim()))
            //     .ForMember(dest => dest.DateReceived,
            //         opts => opts.MapFrom(
            //             src => src.DateCreated
            //         ))

            //     .ReverseMap();

        }
    }
}