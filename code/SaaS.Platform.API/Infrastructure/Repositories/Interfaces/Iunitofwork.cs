using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Unit of Work interface for managing transactions and repository access
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }

        ITenantRepository Tenants { get; }

        IUserRepository Users { get; }

        IUserRolesRepository UserRoles { get; }

        ISubscriptionsPlanRepository SubscriptionsPlans { get; }

        IRolesRepository Roles { get; }
         
        IPermissionRepository Permissions { get; }

        IRolePermissionsrepository RolePermissions { get; }

        IAuditLogsRepository AuditLogs { get; }

        ILeadsRepository Leads { get; }

        IOpportunityRepository Opportunity { get; }

        IActivityRepository Activity { get; }

        INoteRepository Note { get; }

        IDocumentsRepository Documents { get; }

        ICampaignRepository Campaigns { get; }

        ICallDispositionRepository CallDispositions { get; }

        IQueueRepository Queues { get; }

        IAgentRepository Agents { get; }
        IAgentQueuesRepository AgentQueues { get; }
        ICallRecordRepository CallRecord { get; }
        ICallRecordingRepository CallRecordings { get; }
        IIVRFlowsRepository IVRFlows { get; }
        IIVRPromptsRepository IVRPrompts { get; }
        IIVRMenusRepository IVRMenus { get; }
        IIVRMenusOptionsRepository IVRMenusOptions { get; }
        IBranchesRepository Branches { get; }
        IAccountTypesRepository AccountTypes { get; }
        IBankAccountsRepository BankAccounts { get; }
        IChargesRepository Charges { get; }

             




        // Add other repositories as needed
        // IUserRepository Users { get; }
        // ILeadRepository Leads { get; }
        // etc.

        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}