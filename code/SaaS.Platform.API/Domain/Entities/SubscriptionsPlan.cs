using AutoMapper.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace SaaS.Platform.API.Domain.Entities
{
    public class SubscriptionsPlan
    {

        public Guid? SubscriptionPlanId { get; set; }
        public string? PlanName { get; set; }
        public required string PlanCode { get; set; }
        public required string Description { get; set; }
        public required string BillingCycle { get; set; }
        public required string CurrencyCode { get; set; }
        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public required string Features { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }

        public  Guid? ModifiedBy { get; set; }

    }
}
