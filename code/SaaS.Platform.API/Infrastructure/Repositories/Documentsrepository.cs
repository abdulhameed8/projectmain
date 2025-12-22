using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Documents-specific operations
    /// </summary>
    public class DocumentsRepository : GenericRepository<Documents>, IDocumentsRepository
    {
        public DocumentsRepository(ApplicationDbContext context) : base(context)
        {
        }

        
        public async Task<IEnumerable<Documents>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Documents>> GetActiveDocumentsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

       
        

        public async Task<IEnumerable<Documents>> SearchDocumentsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                      c.DocumentName != null && c.DocumentName.ToLower().Contains(lowerSearchTerm))
                     .ToListAsync();
        }

        public async Task<(IEnumerable<Documents> Items, int TotalCount)> GetDocumentsPagedAsync(
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
                    (c.DocumentName != null && c.DocumentName.ToLower().Contains(lowerSearchTerm)));
                    
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