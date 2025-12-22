using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Documents;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Leads entity mappings
    /// </summary>
    public class DocumentsProfile : Profile
    {
        public DocumentsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateDocumentsdto, Documents>()
                .ForMember(dest => dest.DocumentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateDocumentsdto, Documents>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}