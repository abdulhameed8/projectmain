using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    public class WorkflowHistoryRepository : GenericRepository<WorkflowHistory>, IWorkflowHistoryRepository
    {
        public WorkflowHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
