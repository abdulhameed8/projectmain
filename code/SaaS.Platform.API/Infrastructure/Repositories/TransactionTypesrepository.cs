using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for TransactionType-specific operations
    /// </summary>
    public class TransactionTypesRepository : GenericRepository<TransactionTypes>, ITransactionTypesRepository
    {
        public TransactionTypesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TransactionTypes?> GetByTypeCodeAsync(Guid tenantId, string typeCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.TypeCode == typeCode);
        }

        public async Task<IEnumerable<TransactionTypes>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionTypes>> GetActiveTransactionTypesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive)
                .ToListAsync();
        }

        
        public async Task<bool> IsTypeCodeUniqueAsync(Guid tenantId, string typeCode, Guid? excludeTransactionId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.TypeCode == typeCode);

            if (excludeTransactionId.HasValue)
            {
                query = query.Where(c => c.TransactionId != excludeTransactionId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<TransactionTypes>> SearchTransactionTypesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    ( c.TypeName != null && c.TypeName.ToLower().Contains(lowerSearchTerm) ||
                     c.TypeCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<TransactionTypes> Items, int TotalCount)> GetTransactionTypesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    (c.TypeName != null && c.TypeName.ToLower().Contains(lowerSearchTerm)) ||
                    ( c.TypeCode.ToLower().Contains(lowerSearchTerm)));
            }

           var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}