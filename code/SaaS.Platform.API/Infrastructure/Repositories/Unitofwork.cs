using Microsoft.EntityFrameworkCore.Storage;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Unit of Work implementation for managing transactions and repository access
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;



        // Repository instances
        private ICustomerRepository? _customerRepository;
        private ITenantRepository? _tenantRepository;
        private IUserRepository? _userRepository;
        private IUserRolesRepository? _userRolesRepository;
        private ISubscriptionsPlanRepository?  _subscriptionPlanRepository;
        private IRolesRepository? _rolesRepository;
        private IPermissionRepository? _permissionRepository;
        private IRolePermissionsrepository? _rolePermissionsRepository;
            


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Customer repository instance
        /// </summary>
        public ICustomerRepository Customers
        {
            get
            {
                _customerRepository ??= new CustomerRepository(_context);
                return _customerRepository;
            }
        }

        public ITenantRepository Tenants
        {
            get
            {
                _tenantRepository ??= new TenantRepository(_context);
                return _tenantRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }


        public IUserRolesRepository UserRoles
        {
            get
            {
                _userRolesRepository ??= new UserRolesRepository(_context);
                return _userRolesRepository;
            }
        }


        public ISubscriptionsPlanRepository SubscriptionsPlans
        {
            get
            {
                _subscriptionPlanRepository ??= new SubscriptionsPlanRepository(_context);
                return _subscriptionPlanRepository;
            }
        }



        public IRolesRepository Roles
        {
            get
            {
                _rolesRepository ??= new RolesRepository(_context);
                return _rolesRepository;
            }
        }

        public IPermissionRepository Permissions
        {
            get
            {
                _permissionRepository ??= new PermissionRepository(_context);
                return _permissionRepository;
            }
        }

        public IRolePermissionsrepository  RolePermissions
        {
            get
            {
                _rolePermissionsRepository ??= new RolePermissionsRepository(_context);
                return _rolePermissionsRepository;
            }
        }





        // Add other repositories as properties following the same pattern
        // public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        /// <summary>
        /// Save all pending changes to the database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Save all pending changes to the database with cancellation token
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Begin a new database transaction
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Dispose the Unit of Work and release resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}