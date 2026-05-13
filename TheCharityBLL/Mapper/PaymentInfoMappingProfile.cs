using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.PaymentInfoDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    public class PaymentInfoMappingProfile : Profile
    {
        public PaymentInfoMappingProfile()
        {
            // PaymentInfo → PaymentInfoResponseDto
            CreateMap<PaymentInfo, PaymentInfoResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => src.ApiKey ?? string.Empty))
                .ForMember(dest => dest.IntegrationId, opt => opt.MapFrom(src => src.IntegrationId ?? string.Empty))
                .ForMember(dest => dest.IframeId, opt => opt.MapFrom(src => src.IframeId ?? string.Empty))
                .ForMember(dest => dest.HmacKey, opt => opt.MapFrom(src => src.HmacKey ?? string.Empty))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));

            // CreatePaymentInfoDto → PaymentInfo
            CreateMap<CreatePaymentInfoDto, PaymentInfo>()
                .ConstructUsing(src => new PaymentInfo(
                    src.ApiKey,
                    src.IntegrationId,
                    src.IframeId,
                    src.HmacKey));
        }
    }
}
