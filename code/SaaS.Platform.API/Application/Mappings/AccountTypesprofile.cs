using AutoMapper;
using SaaS.Platform.API.Application.DTOs.AccountTypes;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for AccountTypes entity mappings
    /// </summary>
    public class AccountTypesProfile : Profile
    {
        public AccountTypesProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateAccountTypesdto, AccountTypes>()
                .ForMember(dest => dest.AccountTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateAccountTypesdto, AccountTypes>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}