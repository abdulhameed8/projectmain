using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Branches;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Branches entity mappings
    /// </summary>
    public class BranchesProfile : Profile
    {
        public BranchesProfile()
        {
           

            // Create DTO to Entity
            CreateMap<CreateBranchesdto, Branches>()
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateBranchesdto, Branches>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}