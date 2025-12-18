namespace SaaS.Platform.API.Application.DTOs.Activities
{
    public class CreateActivitydto
    {
        public Guid ActivityId { get; set; }

        public Guid? TenantId { get; set; }
        public string ActivityType { get; set; } = "Individual";
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string ActivityStatus { get; set; } = "Active"; // Active, Inactive
        public string? Priority { get; set; }
        public string? StartDateTime { get; set; }
        public required string EndDateTime { get; set; }
        public DateTime DurationMinutes { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }

    public class UpdateActivitydto
    {
        public Guid ActivityId { get; set; }

        public Guid TenantId { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? ActivityStatus { get; set; } 
        public string? Priority { get; set; }
        public string? StartDateTime { get; set; }
        public required string EndDateTime { get; set; }
        public DateTime DurationMinutes { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid RelatedEntityId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }

    public class Activitydto
    {
        public Guid ActivityId { get; set; }
        public Guid TenantId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string ActivityStatus { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public string? StartDateTime { get; set; }
        public required string EndDateTime { get; set; }
        public DateTime DurationMinutes { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid RelatedEntityId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid? ModifiedBy { get; set; }


    }
}
