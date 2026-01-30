using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public interface IVRFlows 
    {
        public Guid TenantId { get; set; }
        public Guid IVRFLowId { get; set; }
        public string FlowCode { get; set; } 
        public string? FlowName { get; set; } 
        public string? Description { get; set; }
        public string FlowType { get; set; }
        public bool IsActive { get; set; } 
        public int Version { get; set; }
        public  string? FlowJson { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public  Guid? ModifiedBy { get; set; }



    }
}



   
