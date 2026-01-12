using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Agents : BaseEntity
    {
        public Guid AgentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public required string Extension { get; set; }
        public string AgentStatus { get; set; } = "Active"; // Active, Inactive
        public int? MaxConcurrentCalls { get; set; }
        public int? SkillLevel { get; set; }
        public bool IsActive { get; set; } = true;
        public  new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid? ModifiedBy { get; set; }




    }
}
