using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Branches : BaseEntity
    {
        public Guid BranchId { get; set; }
        public Guid TenantId { get; set; }
        public string? BranchName { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string? IFSCCode{ get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid BranchManagerUserId { get; set; }
        public bool IsHeadOffice { get; set; }
        public bool IsActive { get; set; } = true;
        public new  DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public  new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }



    }
}

   