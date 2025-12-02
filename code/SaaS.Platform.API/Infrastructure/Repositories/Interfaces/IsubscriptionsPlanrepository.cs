using SaaS.Platform.API.Domain.Entities;
using System.Threading.Tasks;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for SubscriptionsPlan-specific operations
    /// </summary>
    public interface ISubscriptionsPlanRepository : IGenericRepository<SubscriptionsPlan>
    {

        Task<SubscriptionsPlan?> GetByPlanCodeAsync(string planCode);

        Task<bool> IsPlanCodeUniqueAsync(string planCode, Guid? excludeSubscriptionPlanId = null);


    }
}