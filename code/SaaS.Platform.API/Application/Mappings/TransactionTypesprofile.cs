using AutoMapper;
using SaaS.Platform.API.Application.DTOs.TransactionTypes;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for TransactionTypes entity mappings
    /// </summary>
    public class TransactionTypesProfile : Profile
    {
        public TransactionTypesProfile()
        {
           
            // Create DTO to Entity
            CreateMap<CreateTransactionTypesdto, TransactionTypes>()
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateTransactionTypesdto, TransactionTypes>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}