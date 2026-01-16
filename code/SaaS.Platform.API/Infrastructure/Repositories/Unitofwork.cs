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
        private IAuditLogsRepository? _auditLogsRepository;
        private ILeadsRepository? _leadsRepository;
        private IOpportunityRepository? _opportunityRepository;
        private IActivityRepository? _activityRepository;
        private INoteRepository? _noteRepository;
        private IDocumentsRepository? _documentsRepository;
        private ICampaignRepository? _campaignRepository;
        private ICallDispositionRepository? _callDispositionRepository;
        private IQueueRepository? _queueRepository;
        private IAgentRepository?  _agentRepository;
        private IAgentQueuesRepository? _agentQueuesRepository;
        private ICallRecordRepository? _callRecordRepository;
            


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


        public IAuditLogsRepository AuditLogs
        {
            get
            {
                _auditLogsRepository ??= new AuditLogsRepository(_context);
                return _auditLogsRepository;
            }
        }

        public ILeadsRepository Leads
        {
            get
            {
                _leadsRepository ??= new LeadsRepository(_context);
                return _leadsRepository;
            }
        }

        public IOpportunityRepository Opportunity
        {
            get
            {
                _opportunityRepository ??= new OpportunityRepository(_context);
                return _opportunityRepository;
            }
        }

        public IActivityRepository Activity
        {
            get
            {
                _activityRepository ??= new ActivityRepository(_context);
                return _activityRepository;
            }
        }

        public INoteRepository Note
        {
            get
            {
                _noteRepository ??= new NoteRepository(_context);
                return _noteRepository;
            }
        }

        public IDocumentsRepository Documents
        {
            get
            {
                _documentsRepository ??= new DocumentsRepository(_context);
                return _documentsRepository;
            }
        }

        public ICampaignRepository Campaigns
        {
            get
            {
                _campaignRepository ??= new CampaignRepository(_context);
                return _campaignRepository;
            }
        }

        public ICallDispositionRepository CallDispositions
        {
            get
            {
                _callDispositionRepository ??= new CallDispositionRepository(_context);
                return _callDispositionRepository;
            }
        }


        public IQueueRepository Queues
        {
            get
            {
                _queueRepository ??= new QueueRepository(_context);
                return _queueRepository;
            }
        }

        public IAgentRepository Agents
        {
            get
            {
                _agentRepository ??= new AgentRepository(_context);
                return _agentRepository;
            }
        }

        public IAgentQueuesRepository AgentQueues
        {
            get
            {
                _agentQueuesRepository ??= new AgentQueuesRepository(_context);
                return _agentQueuesRepository;
            }
        }

        public ICallRecordRepository CallRecord
        {
            get
            {
                _callRecordRepository ??= new CallRecordRepository(_context);
                return _callRecordRepository;
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