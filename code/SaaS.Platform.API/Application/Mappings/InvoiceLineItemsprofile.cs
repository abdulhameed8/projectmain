using AutoMapper;
using SaaS.Platform.API.Application.DTOs.InvoiceLineItems;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for InvoiceLineItems entity mappings
    /// </summary>
    public class InvoiceLineItemsProfile : Profile
    {
        public InvoiceLineItemsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateInvoiceLineItemsdto, InvoiceLineItems>()
                .ForMember(dest => dest.LineItemId, opt => opt.Ignore());
                


            // Update DTO to Entity
            CreateMap<UpdateInvoiceLineItemsdto, InvoiceLineItems>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}