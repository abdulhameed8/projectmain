using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    public class IVRMenusRepository : GenericRepository<IVRMenus>, IIVRMenusRepository
    {
        public IVRMenusRepository(ApplicationDbContext context) : base(context)
        {
        }

    
    }
}
