using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for CallRecording-specific operations
    /// </summary>
    public class CallRecordingRepository : GenericRepository<CallRecordings>, ICallRecordingRepository
    {
        public CallRecordingRepository(ApplicationDbContext context) : base(context)
        {
        }


        
    }
}