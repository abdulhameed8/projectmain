using AutoMapper;
using SaaS.Platform.API.Application.DTOs.AuditLogs;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for AudirLogs entity mappings
    /// </summary>
    public class AuditLogsProfile : Profile
    {
        public AuditLogsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateAuditLogsdto, AuditLogs>()
                .ForMember(dest => dest.AuditLogId, opt => opt.Ignore());
                
               
            // Update DTO to Entity
            CreateMap<UpdateAuditLogsdto, AuditLogs>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}