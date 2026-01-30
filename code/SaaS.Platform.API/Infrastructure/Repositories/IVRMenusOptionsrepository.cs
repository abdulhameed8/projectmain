using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    public class IVRMenusOptionsRepository : GenericRepository<IVRMenusOptions>, IIVRMenusOptionsRepository
    {
        public IVRMenusOptionsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
