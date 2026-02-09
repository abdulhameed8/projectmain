using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    public class LoanRepaymentsRepository : GenericRepository<LoanRepayments>, ILoanRepaymentsRepository
    {
        public LoanRepaymentsRepository(ApplicationDbContext context) : base(context)
        {
        }    
    }
}
