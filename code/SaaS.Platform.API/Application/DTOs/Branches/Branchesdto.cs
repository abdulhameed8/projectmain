namespace SaaS.Platform.API.Application.DTOs.Branches
{
    public class CreateBranchesdto
    {
        public Guid BranchId { get; set; }
        public Guid TenantId { get; set; }
        public string? BranchName { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid BranchManagerUserId { get; set; }
        public bool IsHeadOffice { get; set; }
        public  DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
       
    }
    public class UpdateBranchesdto
    {
        public Guid BranchId { get; set; }
        public Guid TenantId { get; set; }
        public string? BranchName { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid BranchManagerUserId { get; set; }
        public bool IsHeadOffice { get; set; }
        public bool? IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class Branchesdto
    {
        public Guid BranchId { get; set; }
        public Guid TenantId { get; set; }
        public string? BranchName { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid BranchManagerUserId { get; set; }
        public bool IsHeadOffice { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }
    }
}
