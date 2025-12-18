using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Activities : BaseEntity
    {
      public Guid ActivityId { get; set; }
      public Guid TenantId { get; set; }
      public string ActivityType { get; set; } = "Individual";
      public string? Subject { get; set; } = null;
      public string? Description { get; set; }
      public string ActivityStatus { get; set; } = "Active"; // Active, Inactive
      public string? Priority { get; set; }
      public   string? StartDateTime { get; set; }
      public  required string EndDateTime { get; set; }
      public DateTime DurationMinutes { get; set; }
      public string? RelatedEntityType { get; set; }
      public Guid RelatedEntityId { get; set; }
      public Guid? AssignedUserId { get; set; }
      public DateTime? CompletedDate { get; set; }
      public string? Outcome { get; set; }
      public new DateTime CreatedDate { get; set; }
      public new Guid? CreatedBy { get; set; }
      public new DateTime ModifiedDate { get; set; }
      public new Guid? ModifiedBy { get; set; }


    }
}
