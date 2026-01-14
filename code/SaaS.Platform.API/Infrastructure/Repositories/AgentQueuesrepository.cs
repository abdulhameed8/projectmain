using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for AgentQueues-specific operations
    /// </summary>
    public class AgentQueuesRepository : GenericRepository<AgentQueues>, IAgentQueuesRepository
    {
        public AgentQueuesRepository(ApplicationDbContext context) : base(context)
        {
        }

       

            
        }
    }
