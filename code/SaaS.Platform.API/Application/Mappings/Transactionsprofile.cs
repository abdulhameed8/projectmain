using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Transaction;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Transaction entity mappings
    /// </summary>
    public class TransactionsProfile : Profile
    {
        public TransactionsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateTransactionsdto, Transactions>()
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
               

            // Update DTO to Entity
            CreateMap<UpdateTransactionsdto, Transactions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}