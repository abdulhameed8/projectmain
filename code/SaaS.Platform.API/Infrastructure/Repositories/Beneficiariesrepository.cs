using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Beneficiry-specific operations
    /// </summary>
    public class BeneficiariesRepository : GenericRepository<Beneficiaries>, IBeneficiariesRepository
    {
        public BeneficiariesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Beneficiaries?> GetByBankCodeAsync(Guid tenantId, string bankCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.BankCode == bankCode);
        }

        public async Task<IEnumerable<Beneficiaries>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Beneficiaries>> GetActiveBeneficiariesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

       

        public async Task<bool> IsBankCodeUniqueAsync(Guid tenantId, string bankCode, Guid? excludeBeneficiaryId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.BankCode == bankCode);

            if (excludeBeneficiaryId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeBeneficiaryId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Beneficiaries>> SearchBeneficiariesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (
                     c.BankName != null && c.BankName.ToLower().Contains(lowerSearchTerm) ||
                     c.BankCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Beneficiaries> Items, int TotalCount)> GetBeneficiariesPagedAsync(
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
                    (c.BankName != null && c.BankName.ToLower().Contains(lowerSearchTerm)) ||
                    c.BankCode.ToLower().Contains(lowerSearchTerm));
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