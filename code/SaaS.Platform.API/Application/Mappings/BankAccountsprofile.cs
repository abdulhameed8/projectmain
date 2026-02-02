using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Bankaccounts;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Bankaccount entity mappings
    /// </summary>
    public class BankAccountsProfile : Profile
    {
        public BankAccountsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateBankAccountsdto, BankAccounts>()
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateBankaccountsdto, BankAccounts>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}