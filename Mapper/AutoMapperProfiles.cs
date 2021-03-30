using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.DTOs.Payment;
using WalletQ.DTOs.User;
using WalletQ.Models;

namespace WalletQ.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, PaymentUserDTO>();
            CreateMap<Payment, PaymentDTO>()
                .ForMember(destinationMember => destinationMember.creator,
                memberOptions => memberOptions.MapFrom(
                    sourceMember => sourceMember.creator
                    ))
                .ForMember(destinationMember => destinationMember.validUntill,
                memberOptions => memberOptions.MapFrom(
                    sourceMember => sourceMember.CreatedAt.AddMinutes(sourceMember.validationTime)
                    ))
                .ForMember(destinationMember => destinationMember.WalletId,
                memberOptions => memberOptions.MapFrom(
                    sourceMember => sourceMember.creator.wallet.Id
                    ));
        }
    }
}
