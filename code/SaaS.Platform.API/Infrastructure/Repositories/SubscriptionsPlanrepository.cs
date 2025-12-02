using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for SubscriptionsPlan-specific operations
    /// </summary>
    public class SubscriptionsPlanRepository : GenericRepository<SubscriptionsPlan>, ISubscriptionsPlanRepository
    {
        public SubscriptionsPlanRepository(ApplicationDbContext context) : base(context)
        {
        }



        public async Task<SubscriptionsPlan?> GetByPlanCodeAsync(string planCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.PlanCode == planCode);
        }






        public async Task<bool> IsPlanCodeUniqueAsync(string planCode, Guid? excludeSubscriptionPlanId = null)
        {
            var query = _dbSet.Where(c => c.PlanCode == planCode);

            if (excludeSubscriptionPlanId.HasValue)
            {
                query = query.Where(c => c.SubscriptionPlanId != excludeSubscriptionPlanId.Value);
            }

            return !await query.AnyAsync();
        }

    }
}
        