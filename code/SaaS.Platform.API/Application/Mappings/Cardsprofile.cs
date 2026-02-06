using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Cards;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Cards entity mappings
    /// </summary>
    public class CardsProfile : Profile
    {
        public CardsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateCardsdto, Cards>()
                .ForMember(dest => dest.CardId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateCardsdto, Cards>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}