-- =====================================================
-- SaaS Platform Database Creation Script
-- CRM + CTI + IVR + Core Banking Solution
-- Database: SaaSPlatformDB
-- =====================================================

-- Create Database (Optional - uncomment if needed)
/*
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SaaSPlatformDB')
BEGIN
    CREATE DATABASE SaaSPlatformDB;
END
GO

USE SaaSPlatformDB;
GO
*/

-- =====================================================
-- MODULE 1: CORE/IDENTITY MANAGEMENT
-- =====================================================

-- 1. SubscriptionPlans (Created first as Tenants references it)
CREATE TABLE SubscriptionPlans (
    SubscriptionPlanId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PlanName NVARCHAR(100) NOT NULL,
    PlanCode NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    BillingCycle NVARCHAR(20) NOT NULL CHECK (BillingCycle IN ('Monthly', 'Quarterly', 'Yearly')),
    Price DECIMAL(10,2) NOT NULL,
    CurrencyCode NVARCHAR(10) NOT NULL DEFAULT 'USD',
    MaxUsers INT NOT NULL DEFAULT 10,
    MaxStorageGB INT NOT NULL DEFAULT 10,
    Features NVARCHAR(MAX), -- JSON
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER
);

-- 2. Tenants
CREATE TABLE Tenants (
    TenantId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantName NVARCHAR(200) NOT NULL,
    TenantCode NVARCHAR(50) NOT NULL UNIQUE,
    SubscriptionPlanId UNIQUEIDENTIFIER,
    ContactEmail NVARCHAR(255),
    ContactPhone NVARCHAR(20),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100),
    PostalCode NVARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    SubscriptionStartDate DATETIME2,
    SubscriptionEndDate DATETIME2,
    MaxUsers INT DEFAULT 10,
    MaxStorageGB DECIMAL(10,2) DEFAULT 10,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Tenants_SubscriptionPlans FOREIGN KEY (SubscriptionPlanId) 
        REFERENCES SubscriptionPlans(SubscriptionPlanId) ON DELETE NO ACTION
);

-- 3. Users
CREATE TABLE Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    PasswordSalt NVARCHAR(500) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    ProfileImageUrl NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    EmailVerificationToken NVARCHAR(500),
    LastLoginDate DATETIME2,
    FailedLoginAttempts INT DEFAULT 0,
    LockoutEndDate DATETIME2,
    RefreshToken NVARCHAR(500),
    RefreshTokenExpiryTime DATETIME2,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Users_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION
);

-- 4. Roles
CREATE TABLE Roles (
    RoleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    RoleName NVARCHAR(100) NOT NULL,
    RoleDescription NVARCHAR(500),
    IsSystemRole BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Roles_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Roles_TenantRole UNIQUE (TenantId, RoleName)
);

-- 5. Permissions
CREATE TABLE Permissions (
    PermissionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PermissionName NVARCHAR(100) NOT NULL,
    PermissionCode NVARCHAR(50) NOT NULL UNIQUE,
    Module NVARCHAR(50) NOT NULL CHECK (Module IN ('CRM', 'Banking', 'CTI', 'IVR', 'System')),
    Description NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1
);

-- 6. RolePermissions
CREATE TABLE RolePermissions (
    RolePermissionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) 
        REFERENCES Roles(RoleId) ON DELETE CASCADE,
    CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) 
        REFERENCES Permissions(PermissionId) ON DELETE CASCADE,
    CONSTRAINT UQ_RolePermissions UNIQUE (RoleId, PermissionId)
);

-- 7. UserRoles
CREATE TABLE UserRoles (
    UserRoleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) 
        REFERENCES Roles(RoleId) ON DELETE NO ACTION,
    CONSTRAINT UQ_UserRoles UNIQUE (UserId, RoleId)
);

-- 8. AuditLogs
CREATE TABLE AuditLogs (
    AuditLogId BIGINT PRIMARY KEY IDENTITY(1,1),
    TenantId UNIQUEIDENTIFIER,
    UserId UNIQUEIDENTIFIER,
    Action NVARCHAR(50) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId NVARCHAR(50),
    OldValues NVARCHAR(MAX), -- JSON
    NewValues NVARCHAR(MAX), -- JSON
    IpAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_AuditLogs_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- =====================================================
-- MODULE 2: CRM (CUSTOMER RELATIONSHIP MANAGEMENT)
-- =====================================================

-- 9. Customers
CREATE TABLE Customers (
    CustomerId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CustomerCode NVARCHAR(50) NOT NULL,
    CustomerType NVARCHAR(20) NOT NULL CHECK (CustomerType IN ('Individual', 'Corporate')),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    CompanyName NVARCHAR(200),
    Email NVARCHAR(255),
    Phone NVARCHAR(20),
    Mobile NVARCHAR(20),
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100),
    PostalCode NVARCHAR(20),
    TaxId NVARCHAR(50),
    CustomerSegment NVARCHAR(50),
    CustomerStatus NVARCHAR(50) DEFAULT 'Active' CHECK (CustomerStatus IN ('Active', 'Inactive', 'Blocked')),
    AssignedUserId UNIQUEIDENTIFIER,
    CustomerSource NVARCHAR(50),
    Tags NVARCHAR(500),
    PreferredLanguage NVARCHAR(10) DEFAULT 'en',
    PreferredContactMethod NVARCHAR(20),
    CreditLimit DECIMAL(18,2) DEFAULT 0,
    CreditScore INT,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Customers_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Customers_Users FOREIGN KEY (AssignedUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Customers_TenantCode UNIQUE (TenantId, CustomerCode)
);

-- 10. Leads
CREATE TABLE Leads (
    LeadId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    LeadCode NVARCHAR(50),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    CompanyName NVARCHAR(200),
    Email NVARCHAR(255),
    Phone NVARCHAR(20),
    LeadSource NVARCHAR(50),
    LeadStatus NVARCHAR(50) DEFAULT 'New' CHECK (LeadStatus IN ('New', 'Contacted', 'Qualified', 'Lost', 'Converted')),
    LeadScore INT DEFAULT 0,
    Industry NVARCHAR(100),
    EstimatedValue DECIMAL(18,2),
    AssignedUserId UNIQUEIDENTIFIER,
    ConvertedToCustomerId UNIQUEIDENTIFIER,
    ConvertedDate DATETIME2,
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Leads_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Leads_Users FOREIGN KEY (AssignedUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Leads_Customers FOREIGN KEY (ConvertedToCustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION
);

-- 11. Opportunities
CREATE TABLE Opportunities (
    OpportunityId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    OpportunityCode NVARCHAR(50),
    OpportunityName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    OpportunityType NVARCHAR(50),
    Stage NVARCHAR(50) DEFAULT 'Prospecting' CHECK (Stage IN ('Prospecting', 'Qualification', 'Proposal', 'Negotiation', 'Closed Won', 'Closed Lost')),
    Probability INT CHECK (Probability BETWEEN 0 AND 100),
    Amount DECIMAL(18,2),
    ExpectedCloseDate DATE,
    ActualCloseDate DATE,
    AssignedUserId UNIQUEIDENTIFIER,
    LeadSource NVARCHAR(50),
    LostReason NVARCHAR(500),
    NextStepAction NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Opportunities_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Opportunities_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION,
    CONSTRAINT FK_Opportunities_Users FOREIGN KEY (AssignedUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- 12. Activities
CREATE TABLE Activities (
    ActivityId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ActivityType NVARCHAR(50) NOT NULL CHECK (ActivityType IN ('Call', 'Email', 'Meeting', 'Task')),
    Subject NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    ActivityStatus NVARCHAR(50) DEFAULT 'Scheduled' CHECK (ActivityStatus IN ('Scheduled', 'Completed', 'Cancelled')),
    Priority NVARCHAR(20) DEFAULT 'Medium' CHECK (Priority IN ('Low', 'Medium', 'High')),
    StartDateTime DATETIME2,
    EndDateTime DATETIME2,
    DurationMinutes INT,
    RelatedEntityType NVARCHAR(50),
    RelatedEntityId UNIQUEIDENTIFIER,
    AssignedUserId UNIQUEIDENTIFIER,
    CompletedDate DATETIME2,
    Outcome NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Activities_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Activities_Users FOREIGN KEY (AssignedUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- 13. Notes
CREATE TABLE Notes (
    NoteId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    RelatedEntityType NVARCHAR(50) NOT NULL,
    RelatedEntityId UNIQUEIDENTIFIER NOT NULL,
    NoteText NVARCHAR(MAX) NOT NULL,
    IsPrivate BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Notes_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION
);

-- 14. Documents
CREATE TABLE Documents (
    DocumentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    RelatedEntityType NVARCHAR(50) NOT NULL,
    RelatedEntityId UNIQUEIDENTIFIER NOT NULL,
    DocumentName NVARCHAR(255) NOT NULL,
    DocumentType NVARCHAR(50),
    FileExtension NVARCHAR(10),
    FileSizeBytes BIGINT,
    StoragePath NVARCHAR(500),
    FileUrl NVARCHAR(500),
    MimeType NVARCHAR(100),
    Description NVARCHAR(500),
    UploadedBy UNIQUEIDENTIFIER,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Documents_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Documents_Users FOREIGN KEY (UploadedBy) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- 15. Campaigns
CREATE TABLE Campaigns (
    CampaignId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CampaignName NVARCHAR(200) NOT NULL,
    CampaignType NVARCHAR(50),
    Description NVARCHAR(MAX),
    StartDate DATE,
    EndDate DATE,
    Budget DECIMAL(18,2),
    ActualCost DECIMAL(18,2),
    ExpectedRevenue DECIMAL(18,2),
    Status NVARCHAR(50) DEFAULT 'Planned' CHECK (Status IN ('Planned', 'Active', 'Completed', 'Cancelled')),
    TargetAudience NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Campaigns_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION
);

-- =====================================================
-- MODULE 3: CTI (COMPUTER TELEPHONY INTEGRATION)
-- =====================================================

-- 16. CallDispositions
CREATE TABLE CallDispositions (
    CallDispositionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    DispositionCode NVARCHAR(50) NOT NULL,
    DispositionName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Category NVARCHAR(50),
    RequiresFollowUp BIT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_CallDispositions_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_CallDispositions_TenantCode UNIQUE (TenantId, DispositionCode)
);

-- 17. Queues
CREATE TABLE Queues (
    QueueId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    QueueName NVARCHAR(100) NOT NULL,
    QueueCode NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    Priority INT DEFAULT 5,
    MaxWaitTimeSeconds INT,
    RoutingStrategy NVARCHAR(50) DEFAULT 'RoundRobin' CHECK (RoutingStrategy IN ('RoundRobin', 'LongestIdle', 'SkillBased')),
    WelcomeMessageUrl NVARCHAR(500),
    MusicOnHoldUrl NVARCHAR(500),
    MaxQueueSize INT,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Queues_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Queues_TenantCode UNIQUE (TenantId, QueueCode)
);

-- 18. Agents
CREATE TABLE Agents (
    AgentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    AgentCode NVARCHAR(50) NOT NULL,
    Extension NVARCHAR(20),
    AgentStatus NVARCHAR(50) DEFAULT 'Offline' CHECK (AgentStatus IN ('Available', 'Busy', 'OnBreak', 'Offline')),
    MaxConcurrentCalls INT DEFAULT 1,
    SkillLevel INT DEFAULT 1 CHECK (SkillLevel BETWEEN 1 AND 10),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Agents_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Agents_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Agents_TenantCode UNIQUE (TenantId, AgentCode)
);

-- 19. AgentQueues
CREATE TABLE AgentQueues (
    AgentQueueId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    QueueId UNIQUEIDENTIFIER NOT NULL,
    Priority INT DEFAULT 5,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_AgentQueues_Agents FOREIGN KEY (AgentId) 
        REFERENCES Agents(AgentId) ON DELETE CASCADE,
    CONSTRAINT FK_AgentQueues_Queues FOREIGN KEY (QueueId) 
        REFERENCES Queues(QueueId) ON DELETE CASCADE,
    CONSTRAINT UQ_AgentQueues UNIQUE (AgentId, QueueId)
);

-- 20. CallRecords
CREATE TABLE CallRecords (
    CallRecordId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CallId NVARCHAR(100),
    CallType NVARCHAR(20) CHECK (CallType IN ('Inbound', 'Outbound', 'Internal')),
    CallDirection NVARCHAR(20) CHECK (CallDirection IN ('Incoming', 'Outgoing')),
    CallerNumber NVARCHAR(20),
    CalledNumber NVARCHAR(20),
    AgentUserId UNIQUEIDENTIFIER,
    CustomerId UNIQUEIDENTIFIER,
    QueueId UNIQUEIDENTIFIER,
    CallStartTime DATETIME2,
    CallEndTime DATETIME2,
    CallDurationSeconds INT,
    WaitTimeSeconds INT,
    TalkTimeSeconds INT,
    HoldTimeSeconds INT,
    CallStatus NVARCHAR(50) CHECK (CallStatus IN ('Answered', 'Missed', 'Abandoned', 'Transferred')),
    CallDispositionId UNIQUEIDENTIFIER,
    TransferredToUserId UNIQUEIDENTIFIER,
    TransferCount INT DEFAULT 0,
    RecordingUrl NVARCHAR(500),
    CallNotes NVARCHAR(MAX),
    IVRPath NVARCHAR(500),
    CallCost DECIMAL(10,4),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_CallRecords_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_CallRecords_Agents FOREIGN KEY (AgentUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_CallRecords_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION,
    CONSTRAINT FK_CallRecords_Queues FOREIGN KEY (QueueId) 
        REFERENCES Queues(QueueId) ON DELETE NO ACTION,
    CONSTRAINT FK_CallRecords_Dispositions FOREIGN KEY (CallDispositionId) 
        REFERENCES CallDispositions(CallDispositionId) ON DELETE NO ACTION
);

-- 21. CallRecordings
CREATE TABLE CallRecordings (
    RecordingId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CallRecordId UNIQUEIDENTIFIER NOT NULL,
    RecordingUrl NVARCHAR(500) NOT NULL,
    RecordingDurationSeconds INT,
    RecordingFormat NVARCHAR(20),
    FileSizeBytes BIGINT,
    StoragePath NVARCHAR(500),
    IsTranscribed BIT DEFAULT 0,
    TranscriptionText NVARCHAR(MAX),
    RetentionDate DATE,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_CallRecordings_CallRecords FOREIGN KEY (CallRecordId) 
        REFERENCES CallRecords(CallRecordId) ON DELETE CASCADE
);

-- =====================================================
-- MODULE 4: IVR (INTERACTIVE VOICE RESPONSE)
-- =====================================================

-- 22. IVRFlows
CREATE TABLE IVRFlows (
    IVRFlowId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    FlowName NVARCHAR(100) NOT NULL,
    FlowCode NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    FlowType NVARCHAR(50),
    IsActive BIT NOT NULL DEFAULT 1,
    Version INT DEFAULT 1,
    FlowJson NVARCHAR(MAX), -- JSON structure
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_IVRFlows_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_IVRFlows_TenantCode UNIQUE (TenantId, FlowCode, Version)
);

-- 23. IVRPrompts
CREATE TABLE IVRPrompts (
    PromptId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    PromptName NVARCHAR(100) NOT NULL,
    PromptType NVARCHAR(50),
    PromptText NVARCHAR(MAX),
    AudioUrl NVARCHAR(500),
    Language NVARCHAR(10) DEFAULT 'en',
    DurationSeconds INT,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_IVRPrompts_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION
);

-- 24. IVRMenus
CREATE TABLE IVRMenus (
    IVRMenuId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IVRFlowId UNIQUEIDENTIFIER NOT NULL,
    MenuName NVARCHAR(100) NOT NULL,
    ParentMenuId UNIQUEIDENTIFIER,
    MenuLevel INT DEFAULT 1,
    PromptId UNIQUEIDENTIFIER,
    TimeoutSeconds INT DEFAULT 10,
    MaxRetries INT DEFAULT 3,
    InvalidInputAction NVARCHAR(50),
    SortOrder INT,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_IVRMenus_Flows FOREIGN KEY (IVRFlowId) 
        REFERENCES IVRFlows(IVRFlowId) ON DELETE CASCADE,
    CONSTRAINT FK_IVRMenus_ParentMenu FOREIGN KEY (ParentMenuId) 
        REFERENCES IVRMenus(IVRMenuId) ON DELETE NO ACTION,
    CONSTRAINT FK_IVRMenus_Prompts FOREIGN KEY (PromptId) 
        REFERENCES IVRPrompts(PromptId) ON DELETE NO ACTION
);

-- 25. IVRMenuOptions
CREATE TABLE IVRMenuOptions (
    MenuOptionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IVRMenuId UNIQUEIDENTIFIER NOT NULL,
    OptionKey NVARCHAR(10) NOT NULL,
    OptionDescription NVARCHAR(200),
    ActionType NVARCHAR(50) NOT NULL,
    ActionValue NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    SortOrder INT,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_IVRMenuOptions_Menus FOREIGN KEY (IVRMenuId) 
        REFERENCES IVRMenus(IVRMenuId) ON DELETE CASCADE,
    CONSTRAINT UQ_IVRMenuOptions UNIQUE (IVRMenuId, OptionKey)
);

-- 26. IVRSessions
CREATE TABLE IVRSessions (
    SessionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CallRecordId UNIQUEIDENTIFIER,
    IVRFlowId UNIQUEIDENTIFIER NOT NULL,
    CallerNumber NVARCHAR(20),
    SessionStartTime DATETIME2 NOT NULL,
    SessionEndTime DATETIME2,
    TotalDurationSeconds INT,
    MenuPathJson NVARCHAR(MAX), -- JSON array
    InputsJson NVARCHAR(MAX), -- JSON array
    FinalAction NVARCHAR(100),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_IVRSessions_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_IVRSessions_CallRecords FOREIGN KEY (CallRecordId) 
        REFERENCES CallRecords(CallRecordId) ON DELETE NO ACTION,
    CONSTRAINT FK_IVRSessions_Flows FOREIGN KEY (IVRFlowId) 
        REFERENCES IVRFlows(IVRFlowId) ON DELETE NO ACTION
);

-- =====================================================
-- MODULE 5: CORE BANKING
-- =====================================================

-- 27. Branches
CREATE TABLE Branches (
    BranchId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    BranchName NVARCHAR(200) NOT NULL,
    BranchCode NVARCHAR(50) NOT NULL,
    IFSCCode NVARCHAR(50),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100),
    PostalCode NVARCHAR(20),
    Phone NVARCHAR(20),
    Email NVARCHAR(255),
    BranchManagerUserId UNIQUEIDENTIFIER,
    IsHeadOffice BIT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Branches_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Branches_Users FOREIGN KEY (BranchManagerUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Branches_TenantCode UNIQUE (TenantId, BranchCode)
);

-- 28. AccountTypes
CREATE TABLE AccountTypes (
    AccountTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    AccountTypeName NVARCHAR(100) NOT NULL,
    AccountTypeCode NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    Category NVARCHAR(50),
    MinimumBalance DECIMAL(18,2),
    InterestRate DECIMAL(5,2),
    MonthlyFee DECIMAL(10,2),
    AllowsOverdraft BIT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_AccountTypes_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_AccountTypes_TenantCode UNIQUE (TenantId, AccountTypeCode)
);

-- 29. BankAccounts
CREATE TABLE BankAccounts (
    AccountId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    AccountNumber NVARCHAR(50) NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    AccountTypeId UNIQUEIDENTIFIER NOT NULL,
    AccountName NVARCHAR(200) NOT NULL,
    CurrencyCode NVARCHAR(10) DEFAULT 'USD',
    CurrentBalance DECIMAL(18,2) DEFAULT 0,
    AvailableBalance DECIMAL(18,2) DEFAULT 0,
    HoldAmount DECIMAL(18,2) DEFAULT 0,
    OpeningDate DATE NOT NULL,
    ClosingDate DATE,
    AccountStatus NVARCHAR(50) DEFAULT 'Active' CHECK (AccountStatus IN ('Active', 'Dormant', 'Closed', 'Blocked')),
    BranchId UNIQUEIDENTIFIER,
    InterestRate DECIMAL(5,2),
    MinimumBalance DECIMAL(18,2),
    OverdraftLimit DECIMAL(18,2),
    IsJointAccount BIT DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_BankAccounts_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_BankAccounts_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION,
    CONSTRAINT FK_BankAccounts_AccountTypes FOREIGN KEY (AccountTypeId) 
        REFERENCES AccountTypes(AccountTypeId) ON DELETE NO ACTION,
    CONSTRAINT FK_BankAccounts_Branches FOREIGN KEY (BranchId) 
        REFERENCES Branches(BranchId) ON DELETE NO ACTION,
    CONSTRAINT UQ_BankAccounts_TenantAccount UNIQUE (TenantId, AccountNumber)
);

-- 30. Charges
CREATE TABLE Charges (
    ChargeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ChargeName NVARCHAR(100) NOT NULL,
    ChargeCode NVARCHAR(50) NOT NULL,
    ChargeType NVARCHAR(50) CHECK (ChargeType IN ('Fixed', 'Percentage')),
    Amount DECIMAL(18,2),
    Percentage DECIMAL(5,2),
    MinAmount DECIMAL(18,2),
    MaxAmount DECIMAL(18,2),
    ApplicableOn NVARCHAR(50),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Charges_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Charges_TenantCode UNIQUE (TenantId, ChargeCode)
);

-- 31. TransactionTypes
CREATE TABLE TransactionTypes (
    TransactionTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    TypeName NVARCHAR(100) NOT NULL,
    TypeCode NVARCHAR(50) NOT NULL,
    Category NVARCHAR(50),
    IsDebit BIT NOT NULL,
    RequiresApproval BIT DEFAULT 0,
    ChargeTypeId UNIQUEIDENTIFIER,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_TransactionTypes_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_TransactionTypes_Charges FOREIGN KEY (ChargeTypeId) 
        REFERENCES Charges(ChargeId) ON DELETE NO ACTION,
    CONSTRAINT UQ_TransactionTypes_TenantCode UNIQUE (TenantId, TypeCode)
);

-- 32. Beneficiaries
CREATE TABLE Beneficiaries (
    BeneficiaryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    BeneficiaryName NVARCHAR(200) NOT NULL,
    BeneficiaryAccountNumber NVARCHAR(50) NOT NULL,
    BankName NVARCHAR(200),
    BankCode NVARCHAR(50),
    BranchName NVARCHAR(200),
    IFSCCode NVARCHAR(50),
    SwiftCode NVARCHAR(50),
    BeneficiaryType NVARCHAR(50),
    IsVerified BIT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Beneficiaries_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Beneficiaries_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION
);

-- 33. Transactions
CREATE TABLE Transactions (
    TransactionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    TransactionNumber NVARCHAR(50) NOT NULL,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    TransactionTypeId UNIQUEIDENTIFIER NOT NULL,
    TransactionDate DATETIME2 NOT NULL,
    ValueDate DATE NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    TransactionDirection NVARCHAR(10) CHECK (TransactionDirection IN ('Debit', 'Credit')),
    BalanceAfter DECIMAL(18,2),
    Description NVARCHAR(500),
    ReferenceNumber NVARCHAR(100),
    RelatedTransactionId UNIQUEIDENTIFIER,
    ToAccountId UNIQUEIDENTIFIER,
    BeneficiaryId UNIQUEIDENTIFIER,
    ChannelType NVARCHAR(50),
    TransactionStatus NVARCHAR(50) DEFAULT 'Pending' CHECK (TransactionStatus IN ('Pending', 'Completed', 'Failed', 'Reversed')),
    IsReversed BIT DEFAULT 0,
    ReversalReason NVARCHAR(500),
    AuthorizationCode NVARCHAR(100),
    ProcessedBy UNIQUEIDENTIFIER,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Transactions_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_Accounts FOREIGN KEY (AccountId) 
        REFERENCES BankAccounts(AccountId) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_TransactionTypes FOREIGN KEY (TransactionTypeId) 
        REFERENCES TransactionTypes(TransactionTypeId) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_RelatedTransaction FOREIGN KEY (RelatedTransactionId) 
        REFERENCES Transactions(TransactionId) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_ToAccount FOREIGN KEY (ToAccountId) 
        REFERENCES BankAccounts(AccountId) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_Beneficiaries FOREIGN KEY (BeneficiaryId) 
        REFERENCES Beneficiaries(BeneficiaryId) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_Users FOREIGN KEY (ProcessedBy) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Transactions_TenantNumber UNIQUE (TenantId, TransactionNumber)
);

-- 34. Cards
CREATE TABLE Cards (
    CardId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    CardNumber NVARCHAR(20) NOT NULL, -- Should be encrypted
    CardType NVARCHAR(50),
    CardNetwork NVARCHAR(50),
    NameOnCard NVARCHAR(200),
    IssueDate DATE,
    ExpiryDate DATE,
    CVV NVARCHAR(10), -- Should be encrypted
    CardStatus NVARCHAR(50) DEFAULT 'Active' CHECK (CardStatus IN ('Active', 'Blocked', 'Expired', 'Cancelled')),
    DailyLimit DECIMAL(18,2),
    MonthlyLimit DECIMAL(18,2),
    IsPinSet BIT DEFAULT 0,
    IsContactless BIT DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Cards_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Cards_Accounts FOREIGN KEY (AccountId) 
        REFERENCES BankAccounts(AccountId) ON DELETE NO ACTION,
    CONSTRAINT FK_Cards_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION
);

-- 35. LoanTypes
CREATE TABLE LoanTypes (
    LoanTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    LoanTypeName NVARCHAR(100) NOT NULL,
    LoanTypeCode NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    MinAmount DECIMAL(18,2),
    MaxAmount DECIMAL(18,2),
    MinTenureMonths INT,
    MaxTenureMonths INT,
    InterestRate DECIMAL(5,2),
    ProcessingFeePercentage DECIMAL(5,2),
    RequiresCollateral BIT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_LoanTypes_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_LoanTypes_TenantCode UNIQUE (TenantId, LoanTypeCode)
);

-- 36. Loans
CREATE TABLE Loans (
    LoanId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    LoanNumber NVARCHAR(50) NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    LoanTypeId UNIQUEIDENTIFIER NOT NULL,
    AccountId UNIQUEIDENTIFIER,
    PrincipalAmount DECIMAL(18,2) NOT NULL,
    InterestRate DECIMAL(5,2) NOT NULL,
    TenureMonths INT NOT NULL,
    EMIAmount DECIMAL(18,2),
    StartDate DATE,
    MaturityDate DATE,
    DisbursementDate DATE,
    OutstandingPrincipal DECIMAL(18,2),
    OutstandingInterest DECIMAL(18,2),
    TotalOutstanding DECIMAL(18,2),
    LoanStatus NVARCHAR(50) DEFAULT 'Applied' CHECK (LoanStatus IN ('Applied', 'Approved', 'Disbursed', 'Active', 'Closed', 'Defaulted')),
    Purpose NVARCHAR(500),
    CollateralDescription NVARCHAR(MAX),
    ApprovedBy UNIQUEIDENTIFIER,
    ApprovalDate DATE,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Loans_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Loans_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId) ON DELETE NO ACTION,
    CONSTRAINT FK_Loans_LoanTypes FOREIGN KEY (LoanTypeId) 
        REFERENCES LoanTypes(LoanTypeId) ON DELETE NO ACTION,
    CONSTRAINT FK_Loans_Accounts FOREIGN KEY (AccountId) 
        REFERENCES BankAccounts(AccountId) ON DELETE NO ACTION,
    CONSTRAINT FK_Loans_ApprovedBy FOREIGN KEY (ApprovedBy) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Loans_TenantNumber UNIQUE (TenantId, LoanNumber)
);

-- 37. LoanRepayments
CREATE TABLE LoanRepayments (
    RepaymentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LoanId UNIQUEIDENTIFIER NOT NULL,
    EMINumber INT NOT NULL,
    DueDate DATE NOT NULL,
    PaidDate DATE,
    PrincipalAmount DECIMAL(18,2) NOT NULL,
    InterestAmount DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PaidAmount DECIMAL(18,2),
    LateFee DECIMAL(18,2) DEFAULT 0,
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Overdue', 'Partial')),
    TransactionId UNIQUEIDENTIFIER,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_LoanRepayments_Loans FOREIGN KEY (LoanId) 
        REFERENCES Loans(LoanId) ON DELETE CASCADE,
    CONSTRAINT FK_LoanRepayments_Transactions FOREIGN KEY (TransactionId) 
        REFERENCES Transactions(TransactionId) ON DELETE NO ACTION
);

-- =====================================================
-- MODULE 6: SUBSCRIPTION & BILLING
-- =====================================================

-- 38. Subscriptions
CREATE TABLE Subscriptions (
    SubscriptionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    SubscriptionPlanId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT 'Active' CHECK (Status IN ('Active', 'Suspended', 'Cancelled', 'Expired')),
    BillingCycle NVARCHAR(20) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    NextBillingDate DATE,
    AutoRenew BIT DEFAULT 1,
    CancellationDate DATE,
    CancellationReason NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Subscriptions_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Subscriptions_Plans FOREIGN KEY (SubscriptionPlanId) 
        REFERENCES SubscriptionPlans(SubscriptionPlanId) ON DELETE NO ACTION
);

-- 39. PaymentMethods
CREATE TABLE PaymentMethods (
    PaymentMethodId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    MethodType NVARCHAR(50) NOT NULL,
    CardholderName NVARCHAR(200),
    Last4Digits NVARCHAR(4),
    ExpiryMonth INT,
    ExpiryYear INT,
    IsDefault BIT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_PaymentMethods_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION
);

-- 40. Invoices
CREATE TABLE Invoices (
    InvoiceId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    SubscriptionId UNIQUEIDENTIFIER NOT NULL,
    InvoiceNumber NVARCHAR(50) NOT NULL,
    InvoiceDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    SubTotal DECIMAL(10,2) NOT NULL,
    TaxAmount DECIMAL(10,2) DEFAULT 0,
    DiscountAmount DECIMAL(10,2) DEFAULT 0,
    TotalAmount DECIMAL(10,2) NOT NULL,
    PaidAmount DECIMAL(10,2) DEFAULT 0,
    Status NVARCHAR(50) DEFAULT 'Draft' CHECK (Status IN ('Draft', 'Sent', 'Paid', 'Overdue', 'Cancelled')),
    PaymentDate DATE,
    PaymentMethodId UNIQUEIDENTIFIER,
    Notes NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Invoices_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Invoices_Subscriptions FOREIGN KEY (SubscriptionId) 
        REFERENCES Subscriptions(SubscriptionId) ON DELETE NO ACTION,
    CONSTRAINT FK_Invoices_PaymentMethods FOREIGN KEY (PaymentMethodId) 
        REFERENCES PaymentMethods(PaymentMethodId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Invoices_Number UNIQUE (InvoiceNumber)
);

-- 41. InvoiceLineItems
CREATE TABLE InvoiceLineItems (
    LineItemId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    InvoiceId UNIQUEIDENTIFIER NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(10,2) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    TaxPercentage DECIMAL(5,2) DEFAULT 0,
    TaxAmount DECIMAL(10,2) DEFAULT 0,
    LineTotal DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_InvoiceLineItems_Invoices FOREIGN KEY (InvoiceId) 
        REFERENCES Invoices(InvoiceId) ON DELETE CASCADE
);

-- 42. Payments
CREATE TABLE Payments (
    PaymentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    InvoiceId UNIQUEIDENTIFIER NOT NULL,
    PaymentNumber NVARCHAR(50) NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethodId UNIQUEIDENTIFIER NOT NULL,
    TransactionReference NVARCHAR(100),
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Completed', 'Failed', 'Refunded')),
    Notes NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_Payments_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Payments_Invoices FOREIGN KEY (InvoiceId) 
        REFERENCES Invoices(InvoiceId) ON DELETE NO ACTION,
    CONSTRAINT FK_Payments_PaymentMethods FOREIGN KEY (PaymentMethodId) 
        REFERENCES PaymentMethods(PaymentMethodId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Payments_Number UNIQUE (PaymentNumber)
);

-- =====================================================
-- MODULE 7: WORKFLOW & PROCESS MANAGEMENT
-- =====================================================

-- 43. WorkflowDefinitions
CREATE TABLE WorkflowDefinitions (
    WorkflowDefinitionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    WorkflowName NVARCHAR(200) NOT NULL,
    WorkflowKey NVARCHAR(100) NOT NULL,
    Version INT DEFAULT 1,
    BpmnXml NVARCHAR(MAX),
    Category NVARCHAR(50),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_WorkflowDefinitions_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_WorkflowDefinitions UNIQUE (TenantId, WorkflowKey, Version)
);

-- 44. WorkflowInstances
CREATE TABLE WorkflowInstances (
    WorkflowInstanceId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    WorkflowDefinitionId UNIQUEIDENTIFIER NOT NULL,
    CamundaProcessInstanceId NVARCHAR(100),
    EntityType NVARCHAR(50),
    EntityId UNIQUEIDENTIFIER,
    Status NVARCHAR(50) DEFAULT 'Running' CHECK (Status IN ('Running', 'Completed', 'Terminated', 'Suspended')),
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2,
    StartedBy UNIQUEIDENTIFIER,
    Variables NVARCHAR(MAX), -- JSON
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_WorkflowInstances_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_WorkflowInstances_Definitions FOREIGN KEY (WorkflowDefinitionId) 
        REFERENCES WorkflowDefinitions(WorkflowDefinitionId) ON DELETE NO ACTION,
    CONSTRAINT FK_WorkflowInstances_Users FOREIGN KEY (StartedBy) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- 45. WorkflowTasks
CREATE TABLE WorkflowTasks (
    WorkflowTaskId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WorkflowInstanceId UNIQUEIDENTIFIER NOT NULL,
    CamundaTaskId NVARCHAR(100),
    TaskName NVARCHAR(200) NOT NULL,
    TaskKey NVARCHAR(100),
    AssignedUserId UNIQUEIDENTIFIER,
    AssignedRoleId UNIQUEIDENTIFIER,
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'InProgress', 'Completed', 'Cancelled')),
    Priority INT DEFAULT 5,
    DueDate DATETIME2,
    CompletedDate DATETIME2,
    CompletedBy UNIQUEIDENTIFIER,
    FormData NVARCHAR(MAX), -- JSON
    Comments NVARCHAR(MAX),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_WorkflowTasks_Instances FOREIGN KEY (WorkflowInstanceId) 
        REFERENCES WorkflowInstances(WorkflowInstanceId) ON DELETE CASCADE,
    CONSTRAINT FK_WorkflowTasks_AssignedUser FOREIGN KEY (AssignedUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_WorkflowTasks_AssignedRole FOREIGN KEY (AssignedRoleId) 
        REFERENCES Roles(RoleId) ON DELETE NO ACTION,
    CONSTRAINT FK_WorkflowTasks_CompletedBy FOREIGN KEY (CompletedBy) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- 46. WorkflowHistory
CREATE TABLE WorkflowHistory (
    HistoryId BIGINT PRIMARY KEY IDENTITY(1,1),
    WorkflowInstanceId UNIQUEIDENTIFIER NOT NULL,
    WorkflowTaskId UNIQUEIDENTIFIER,
    Action NVARCHAR(100) NOT NULL,
    ActorUserId UNIQUEIDENTIFIER,
    OldStatus NVARCHAR(50),
    NewStatus NVARCHAR(50),
    Comments NVARCHAR(MAX),
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_WorkflowHistory_Instances FOREIGN KEY (WorkflowInstanceId) 
        REFERENCES WorkflowInstances(WorkflowInstanceId) ON DELETE CASCADE,
    CONSTRAINT FK_WorkflowHistory_Tasks FOREIGN KEY (WorkflowTaskId) 
        REFERENCES WorkflowTasks(WorkflowTaskId) ON DELETE NO ACTION,
    CONSTRAINT FK_WorkflowHistory_Users FOREIGN KEY (ActorUserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION
);

-- =====================================================
-- MODULE 8: NOTIFICATIONS
-- =====================================================

-- 47. NotificationTemplates
CREATE TABLE NotificationTemplates (
    TemplateId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER,
    TemplateName NVARCHAR(100) NOT NULL,
    TemplateCode NVARCHAR(50) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL CHECK (NotificationType IN ('Email', 'SMS', 'Push', 'InApp')),
    Subject NVARCHAR(200),
    BodyTemplate NVARCHAR(MAX) NOT NULL,
    Variables NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_NotificationTemplates_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_NotificationTemplates UNIQUE (TenantId, TemplateCode)
);

-- 48. Notifications
CREATE TABLE Notifications (
    NotificationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER,
    NotificationType NVARCHAR(50) NOT NULL,
    TemplateId UNIQUEIDENTIFIER,
    Subject NVARCHAR(200),
    Body NVARCHAR(MAX) NOT NULL,
    RecipientEmail NVARCHAR(255),
    RecipientPhone NVARCHAR(20),
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Sent', 'Failed', 'Read')),
    SentDate DATETIME2,
    ReadDate DATETIME2,
    ErrorMessage NVARCHAR(500),
    RetryCount INT DEFAULT 0,
    Priority INT DEFAULT 5,
    RelatedEntityType NVARCHAR(50),
    RelatedEntityId UNIQUEIDENTIFIER,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Notifications_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Notifications_Templates FOREIGN KEY (TemplateId) 
        REFERENCES NotificationTemplates(TemplateId) ON DELETE NO ACTION
);

-- =====================================================
-- MODULE 9: SYSTEM CONFIGURATION
-- =====================================================

-- 49. SystemSettings
CREATE TABLE SystemSettings (
    SettingId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER,
    SettingKey NVARCHAR(100) NOT NULL,
    SettingValue NVARCHAR(MAX),
    DataType NVARCHAR(50) NOT NULL DEFAULT 'String' CHECK (DataType IN ('String', 'Integer', 'Decimal', 'Boolean', 'JSON')),
    Category NVARCHAR(50),
    Description NVARCHAR(500),
    IsEncrypted BIT DEFAULT 0,
    ModifiedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_SystemSettings_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_SystemSettings_Key UNIQUE (TenantId, SettingKey)
);

-- 50. EmailConfiguration
CREATE TABLE EmailConfiguration (
    EmailConfigId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    SMTPHost NVARCHAR(200) NOT NULL,
    SMTPPort INT NOT NULL DEFAULT 587,
    Username NVARCHAR(200) NOT NULL,
    Password NVARCHAR(500) NOT NULL, -- Should be encrypted
    EnableSSL BIT DEFAULT 1,
    FromEmail NVARCHAR(255) NOT NULL,
    FromName NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    ModifiedDate DATETIME2,
    ModifiedBy UNIQUEIDENTIFIER,
    CONSTRAINT FK_EmailConfiguration_Tenants FOREIGN KEY (TenantId) 
        REFERENCES Tenants(TenantId) ON DELETE NO ACTION
);

-- =====================================================
-- CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- Tenants
CREATE NONCLUSTERED INDEX IX_Tenants_TenantCode ON Tenants(TenantCode);
CREATE NONCLUSTERED INDEX IX_Tenants_IsActive ON Tenants(IsActive) WHERE IsActive = 1;

-- Users
CREATE NONCLUSTERED INDEX IX_Users_TenantId ON Users(TenantId);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_IsActive ON Users(IsActive) WHERE IsActive = 1;

-- Roles
CREATE NONCLUSTERED INDEX IX_Roles_TenantId ON Roles(TenantId);

-- AuditLogs
CREATE NONCLUSTERED INDEX IX_AuditLogs_TenantId ON AuditLogs(TenantId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_EntityName ON AuditLogs(EntityName);
CREATE NONCLUSTERED INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp DESC);

-- Customers
CREATE NONCLUSTERED INDEX IX_Customers_TenantId ON Customers(TenantId);
CREATE NONCLUSTERED INDEX IX_Customers_CustomerCode ON Customers(CustomerCode);
CREATE NONCLUSTERED INDEX IX_Customers_Email ON Customers(Email);
CREATE NONCLUSTERED INDEX IX_Customers_AssignedUserId ON Customers(AssignedUserId);
CREATE NONCLUSTERED INDEX IX_Customers_CustomerStatus ON Customers(CustomerStatus);

-- Leads
CREATE NONCLUSTERED INDEX IX_Leads_TenantId ON Leads(TenantId);
CREATE NONCLUSTERED INDEX IX_Leads_AssignedUserId ON Leads(AssignedUserId);
CREATE NONCLUSTERED INDEX IX_Leads_LeadStatus ON Leads(LeadStatus);

-- Opportunities
CREATE NONCLUSTERED INDEX IX_Opportunities_TenantId ON Opportunities(TenantId);
CREATE NONCLUSTERED INDEX IX_Opportunities_CustomerId ON Opportunities(CustomerId);
CREATE NONCLUSTERED INDEX IX_Opportunities_AssignedUserId ON Opportunities(AssignedUserId);
CREATE NONCLUSTERED INDEX IX_Opportunities_Stage ON Opportunities(Stage);

-- Activities
CREATE NONCLUSTERED INDEX IX_Activities_TenantId ON Activities(TenantId);
CREATE NONCLUSTERED INDEX IX_Activities_AssignedUserId ON Activities(AssignedUserId);
CREATE NONCLUSTERED INDEX IX_Activities_RelatedEntity ON Activities(RelatedEntityType, RelatedEntityId);
CREATE NONCLUSTERED INDEX IX_Activities_StartDateTime ON Activities(StartDateTime);

-- CallRecords
CREATE NONCLUSTERED INDEX IX_CallRecords_TenantId ON CallRecords(TenantId);
CREATE NONCLUSTERED INDEX IX_CallRecords_AgentUserId ON CallRecords(AgentUserId);
CREATE NONCLUSTERED INDEX IX_CallRecords_CustomerId ON CallRecords(CustomerId);
CREATE NONCLUSTERED INDEX IX_CallRecords_CallStartTime ON CallRecords(CallStartTime DESC);
CREATE NONCLUSTERED INDEX IX_CallRecords_CallStatus ON CallRecords(CallStatus);

-- BankAccounts
CREATE NONCLUSTERED INDEX IX_BankAccounts_TenantId ON BankAccounts(TenantId);
CREATE NONCLUSTERED INDEX IX_BankAccounts_CustomerId ON BankAccounts(CustomerId);
CREATE NONCLUSTERED INDEX IX_BankAccounts_AccountNumber ON BankAccounts(AccountNumber);
CREATE NONCLUSTERED INDEX IX_BankAccounts_AccountStatus ON BankAccounts(AccountStatus);

-- Transactions
CREATE NONCLUSTERED INDEX IX_Transactions_TenantId ON Transactions(TenantId);
CREATE NONCLUSTERED INDEX IX_Transactions_AccountId ON Transactions(AccountId);
CREATE NONCLUSTERED INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate DESC);
CREATE NONCLUSTERED INDEX IX_Transactions_TransactionNumber ON Transactions(TransactionNumber);
CREATE NONCLUSTERED INDEX IX_Transactions_TransactionStatus ON Transactions(TransactionStatus);

-- Loans
CREATE NONCLUSTERED INDEX IX_Loans_TenantId ON Loans(TenantId);
CREATE NONCLUSTERED INDEX IX_Loans_CustomerId ON Loans(CustomerId);
CREATE NONCLUSTERED INDEX IX_Loans_LoanNumber ON Loans(LoanNumber);
CREATE NONCLUSTERED INDEX IX_Loans_LoanStatus ON Loans(LoanStatus);

-- Invoices
CREATE NONCLUSTERED INDEX IX_Invoices_TenantId ON Invoices(TenantId);
CREATE NONCLUSTERED INDEX IX_Invoices_SubscriptionId ON Invoices(SubscriptionId);
CREATE NONCLUSTERED INDEX IX_Invoices_InvoiceNumber ON Invoices(InvoiceNumber);
CREATE NONCLUSTERED INDEX IX_Invoices_Status ON Invoices(Status);
CREATE NONCLUSTERED INDEX IX_Invoices_InvoiceDate ON Invoices(InvoiceDate DESC);

-- Notifications
CREATE NONCLUSTERED INDEX IX_Notifications_TenantId ON Notifications(TenantId);
CREATE NONCLUSTERED INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE NONCLUSTERED INDEX IX_Notifications_Status ON Notifications(Status);
CREATE NONCLUSTERED INDEX IX_Notifications_CreatedDate ON Notifications(CreatedDate DESC);

-- WorkflowInstances
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_TenantId ON WorkflowInstances(TenantId);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_WorkflowDefinitionId ON WorkflowInstances(WorkflowDefinitionId);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_EntityType ON WorkflowInstances(EntityType, EntityId);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_Status ON WorkflowInstances(Status);

-- WorkflowTasks
CREATE NONCLUSTERED INDEX IX_WorkflowTasks_WorkflowInstanceId ON WorkflowTasks(WorkflowInstanceId);
CREATE NONCLUSTERED INDEX IX_WorkflowTasks_AssignedUserId ON WorkflowTasks(AssignedUserId);
CREATE NONCLUSTERED INDEX IX_WorkflowTasks_Status ON WorkflowTasks(Status);

GO

PRINT 'Database schema created successfully!';
PRINT 'Total tables created: 50';
PRINT '';
PRINT 'Modules included:';
PRINT '1. Core/Identity Management (8 tables)';
PRINT '2. CRM (7 tables)';
PRINT '3. CTI (6 tables)';
PRINT '4. IVR (5 tables)';
PRINT '5. Core Banking (11 tables)';
PRINT '6. Subscription & Billing (6 tables)';
PRINT '7. Workflow Management (4 tables)';
PRINT '8. Notifications (2 tables)';
PRINT '9. System Configuration (2 tables)';
