using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for WorkflowTask-specific operations
    /// </summary>
    public class WorkflowTaskRepository : GenericRepository<WorkflowTasks>, IWorkflowTaskRepository
    {
        public WorkflowTaskRepository(ApplicationDbContext context) : base(context)
        {
        }



    }
}