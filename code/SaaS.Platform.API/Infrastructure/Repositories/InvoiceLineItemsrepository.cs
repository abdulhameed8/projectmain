using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    
        public class InvoiceLineItemsRepository : GenericRepository<InvoiceLineItems>, IInvoiceLineItemsRepository
        {
            public InvoiceLineItemsRepository(ApplicationDbContext context) : base(context)
            {
            }
        }
}
