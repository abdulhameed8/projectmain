using SaaS.Platform.API.Domain.Common;
    

namespace SaaS.Platform.API.Domain.Entities
    {
        public class AccountTypes : BaseEntity
        {
            public Guid AccountTypeId { get; set; }
            public Guid TenantId { get; set; }
            public string? AccountTypeName { get; set; }
            public string AccountTypeCode { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string? Category { get; set; }
            public decimal MinimumBalance { get; set; }
            public decimal  InterestRate { get; set; }
            public decimal MonthlyFee { get; set; }
            public bool AllowsOverdraft { get; set; }
            public bool IsActive { get; set; } = true;
            public new DateTime CreatedDate { get; set; }
            public new Guid? CreatedBy { get; set; }
           



        }
    }



