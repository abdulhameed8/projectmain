using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Invoices;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Invoice entity mappings
    /// </summary>
    public class InvoicesProfile : Profile
    {
        public InvoicesProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateInvoicesdto, Invoices>()
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());


            // Update DTO to Entity
            CreateMap<UpdateInvoicesdto, Invoices>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}