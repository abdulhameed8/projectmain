# Database Design Documentation
## SaaS Platform: CRM + CTI + IVR + Core Banking Solution

### Overview
This document provides comprehensive database design for a multi-tenant SaaS platform with integrated CRM, CTI (Computer Telephony Integration), IVR (Interactive Voice Response), and Core Banking capabilities.

---

## Module 1: Core/Identity Management

### 1.1 Tenants (Organizations)
Multi-tenancy support for SaaS model
- TenantId (PK, UNIQUEIDENTIFIER)
- TenantName (NVARCHAR(200), NOT NULL)
- TenantCode (NVARCHAR(50), UNIQUE)
- SubscriptionPlanId (FK)
- ContactEmail (NVARCHAR(255))
- ContactPhone (NVARCHAR(20))
- Address (NVARCHAR(500))
- City, State, Country, PostalCode
- IsActive (BIT, DEFAULT 1)
- SubscriptionStartDate (DATETIME2)
- SubscriptionEndDate (DATETIME2)
- MaxUsers (INT)
- MaxStorageGB (DECIMAL(10,2))
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 1.2 Users
System users across all tenants
- UserId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- Username (NVARCHAR(100), UNIQUE)
- Email (NVARCHAR(255), UNIQUE)
- PasswordHash (NVARCHAR(500))
- PasswordSalt (NVARCHAR(500))
- FirstName (NVARCHAR(100))
- LastName (NVARCHAR(100))
- PhoneNumber (NVARCHAR(20))
- ProfileImageUrl (NVARCHAR(500))
- IsActive (BIT)
- IsEmailVerified (BIT)
- EmailVerificationToken (NVARCHAR(500))
- LastLoginDate (DATETIME2)
- FailedLoginAttempts (INT)
- LockoutEndDate (DATETIME2)
- RefreshToken (NVARCHAR(500))
- RefreshTokenExpiryTime (DATETIME2)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 1.3 Roles
User roles within tenants
- RoleId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- RoleName (NVARCHAR(100))
- RoleDescription (NVARCHAR(500))
- IsSystemRole (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 1.4 Permissions
Granular permission system
- PermissionId (PK, UNIQUEIDENTIFIER)
- PermissionName (NVARCHAR(100))
- PermissionCode (NVARCHAR(50), UNIQUE)
- Module (NVARCHAR(50)) -- CRM, Banking, CTI, IVR
- Description (NVARCHAR(500))
- IsActive (BIT)

### 1.5 RolePermissions
Many-to-Many relationship
- RolePermissionId (PK, UNIQUEIDENTIFIER)
- RoleId (FK)
- PermissionId (FK)
- CreatedDate, CreatedBy

### 1.6 UserRoles
Many-to-Many relationship
- UserRoleId (PK, UNIQUEIDENTIFIER)
- UserId (FK)
- RoleId (FK)
- CreatedDate, CreatedBy

### 1.7 AuditLogs
System-wide audit trail
- AuditLogId (PK, BIGINT, IDENTITY)
- TenantId (FK)
- UserId (FK)
- Action (NVARCHAR(50))
- EntityName (NVARCHAR(100))
- EntityId (NVARCHAR(50))
- OldValues (NVARCHAR(MAX)) -- JSON
- NewValues (NVARCHAR(MAX)) -- JSON
- IpAddress (NVARCHAR(50))
- UserAgent (NVARCHAR(500))
- Timestamp (DATETIME2)

---

## Module 2: CRM (Customer Relationship Management)

### 2.1 Customers
Primary customer/contact entity
- CustomerId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- CustomerCode (NVARCHAR(50), UNIQUE)
- CustomerType (NVARCHAR(20)) -- Individual, Corporate
- FirstName (NVARCHAR(100))
- LastName (NVARCHAR(100))
- CompanyName (NVARCHAR(200))
- Email (NVARCHAR(255))
- Phone (NVARCHAR(20))
- Mobile (NVARCHAR(20))
- DateOfBirth (DATE)
- Gender (NVARCHAR(10))
- Address (NVARCHAR(500))
- City, State, Country, PostalCode
- TaxId (NVARCHAR(50))
- CustomerSegment (NVARCHAR(50))
- CustomerStatus (NVARCHAR(50)) -- Active, Inactive, Blocked
- AssignedUserId (FK to Users)
- CustomerSource (NVARCHAR(50)) -- Website, Referral, Campaign
- Tags (NVARCHAR(500))
- PreferredLanguage (NVARCHAR(10))
- PreferredContactMethod (NVARCHAR(20))
- CreditLimit (DECIMAL(18,2))
- CreditScore (INT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 2.2 Leads
Sales leads before conversion
- LeadId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- LeadCode (NVARCHAR(50))
- FirstName, LastName
- CompanyName (NVARCHAR(200))
- Email, Phone
- LeadSource (NVARCHAR(50))
- LeadStatus (NVARCHAR(50)) -- New, Contacted, Qualified, Lost
- LeadScore (INT)
- Industry (NVARCHAR(100))
- EstimatedValue (DECIMAL(18,2))
- AssignedUserId (FK)
- ConvertedToCustomerId (FK to Customers)
- ConvertedDate (DATETIME2)
- Notes (NVARCHAR(MAX))
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 2.3 Opportunities
Sales opportunities
- OpportunityId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- CustomerId (FK)
- OpportunityCode (NVARCHAR(50))
- OpportunityName (NVARCHAR(200))
- Description (NVARCHAR(MAX))
- OpportunityType (NVARCHAR(50))
- Stage (NVARCHAR(50)) -- Prospecting, Qualification, Proposal, Negotiation, Closed Won, Closed Lost
- Probability (INT) -- 0-100
- Amount (DECIMAL(18,2))
- ExpectedCloseDate (DATE)
- ActualCloseDate (DATE)
- AssignedUserId (FK)
- LeadSource (NVARCHAR(50))
- LostReason (NVARCHAR(500))
- NextStepAction (NVARCHAR(500))
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 2.4 Activities
Calls, meetings, emails, tasks
- ActivityId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- ActivityType (NVARCHAR(50)) -- Call, Email, Meeting, Task
- Subject (NVARCHAR(200))
- Description (NVARCHAR(MAX))
- ActivityStatus (NVARCHAR(50)) -- Scheduled, Completed, Cancelled
- Priority (NVARCHAR(20)) -- Low, Medium, High
- StartDateTime (DATETIME2)
- EndDateTime (DATETIME2)
- DurationMinutes (INT)
- RelatedEntityType (NVARCHAR(50)) -- Customer, Lead, Opportunity
- RelatedEntityId (UNIQUEIDENTIFIER)
- AssignedUserId (FK)
- CompletedDate (DATETIME2)
- Outcome (NVARCHAR(500))
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 2.5 Notes
Notes attached to various entities
- NoteId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- RelatedEntityType (NVARCHAR(50))
- RelatedEntityId (UNIQUEIDENTIFIER)
- NoteText (NVARCHAR(MAX))
- IsPrivate (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 2.6 Documents
Document attachments
- DocumentId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- RelatedEntityType (NVARCHAR(50))
- RelatedEntityId (UNIQUEIDENTIFIER)
- DocumentName (NVARCHAR(255))
- DocumentType (NVARCHAR(50))
- FileExtension (NVARCHAR(10))
- FileSizeBytes (BIGINT)
- StoragePath (NVARCHAR(500))
- FileUrl (NVARCHAR(500))
- MimeType (NVARCHAR(100))
- Description (NVARCHAR(500))
- UploadedBy (FK to Users)
- CreatedDate

### 2.7 Campaigns
Marketing campaigns
- CampaignId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- CampaignName (NVARCHAR(200))
- CampaignType (NVARCHAR(50)) -- Email, SMS, Social
- Description (NVARCHAR(MAX))
- StartDate, EndDate
- Budget (DECIMAL(18,2))
- ActualCost (DECIMAL(18,2))
- ExpectedRevenue (DECIMAL(18,2))
- Status (NVARCHAR(50)) -- Planned, Active, Completed, Cancelled
- TargetAudience (NVARCHAR(500))
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

---

## Module 3: CTI (Computer Telephony Integration)

### 3.1 CallRecords
Complete call history
- CallRecordId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- CallId (NVARCHAR(100)) -- External system call ID
- CallType (NVARCHAR(20)) -- Inbound, Outbound, Internal
- CallDirection (NVARCHAR(20)) -- Incoming, Outgoing
- CallerNumber (NVARCHAR(20))
- CalledNumber (NVARCHAR(20))
- AgentUserId (FK to Users)
- CustomerId (FK to Customers)
- QueueId (FK)
- CallStartTime (DATETIME2)
- CallEndTime (DATETIME2)
- CallDurationSeconds (INT)
- WaitTimeSeconds (INT)
- TalkTimeSeconds (INT)
- HoldTimeSeconds (INT)
- CallStatus (NVARCHAR(50)) -- Answered, Missed, Abandoned, Transferred
- CallDispositionId (FK)
- TransferredToUserId (FK to Users)
- TransferCount (INT)
- RecordingUrl (NVARCHAR(500))
- CallNotes (NVARCHAR(MAX))
- IVRPath (NVARCHAR(500))
- CallCost (DECIMAL(10,4))
- CreatedDate

### 3.2 CallDispositions
Call outcome codes
- CallDispositionId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- DispositionCode (NVARCHAR(50))
- DispositionName (NVARCHAR(100))
- Description (NVARCHAR(500))
- Category (NVARCHAR(50)) -- Resolved, FollowUp, Complaint
- RequiresFollowUp (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy

### 3.3 Agents
Call center agents
- AgentId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- UserId (FK)
- AgentCode (NVARCHAR(50))
- Extension (NVARCHAR(20))
- AgentStatus (NVARCHAR(50)) -- Available, Busy, OnBreak, Offline
- MaxConcurrentCalls (INT)
- SkillLevel (INT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 3.4 Queues
Call queues
- QueueId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- QueueName (NVARCHAR(100))
- QueueCode (NVARCHAR(50))
- Description (NVARCHAR(500))
- Priority (INT)
- MaxWaitTimeSeconds (INT)
- RoutingStrategy (NVARCHAR(50)) -- RoundRobin, LongestIdle, SkillBased
- WelcomeMessageUrl (NVARCHAR(500))
- MusicOnHoldUrl (NVARCHAR(500))
- MaxQueueSize (INT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 3.5 AgentQueues
Agent to Queue assignment
- AgentQueueId (PK, UNIQUEIDENTIFIER)
- AgentId (FK)
- QueueId (FK)
- Priority (INT)
- IsActive (BIT)
- CreatedDate

### 3.6 CallRecordings
Call recording metadata
- RecordingId (PK, UNIQUEIDENTIFIER)
- CallRecordId (FK)
- RecordingUrl (NVARCHAR(500))
- RecordingDurationSeconds (INT)
- RecordingFormat (NVARCHAR(20))
- FileSizeBytes (BIGINT)
- StoragePath (NVARCHAR(500))
- IsTranscribed (BIT)
- TranscriptionText (NVARCHAR(MAX))
- RetentionDate (DATE)
- CreatedDate

---

## Module 4: IVR (Interactive Voice Response)

### 4.1 IVRFlows
IVR flow definitions
- IVRFlowId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- FlowName (NVARCHAR(100))
- FlowCode (NVARCHAR(50))
- Description (NVARCHAR(500))
- FlowType (NVARCHAR(50)) -- MainMenu, Survey, Callback
- IsActive (BIT)
- Version (INT)
- FlowJson (NVARCHAR(MAX)) -- JSON structure
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 4.2 IVRMenus
IVR menu options
- IVRMenuId (PK, UNIQUEIDENTIFIER)
- IVRFlowId (FK)
- MenuName (NVARCHAR(100))
- ParentMenuId (FK to IVRMenus) -- Self-reference
- MenuLevel (INT)
- PromptId (FK)
- TimeoutSeconds (INT)
- MaxRetries (INT)
- InvalidInputAction (NVARCHAR(50))
- SortOrder (INT)
- CreatedDate, CreatedBy

### 4.3 IVRMenuOptions
Individual menu options
- MenuOptionId (PK, UNIQUEIDENTIFIER)
- IVRMenuId (FK)
- OptionKey (NVARCHAR(10)) -- 1, 2, 3, etc.
- OptionDescription (NVARCHAR(200))
- ActionType (NVARCHAR(50)) -- GoToMenu, TransferToQueue, PlayMessage, EndCall
- ActionValue (NVARCHAR(500)) -- MenuId, QueueId, MessageUrl, etc.
- IsActive (BIT)
- SortOrder (INT)
- CreatedDate, CreatedBy

### 4.4 IVRPrompts
Audio prompts
- PromptId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- PromptName (NVARCHAR(100))
- PromptType (NVARCHAR(50)) -- Welcome, Menu, Error, Goodbye
- PromptText (NVARCHAR(MAX)) -- Text-to-speech
- AudioUrl (NVARCHAR(500))
- Language (NVARCHAR(10))
- DurationSeconds (INT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 4.5 IVRSessions
IVR session tracking
- SessionId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- CallRecordId (FK)
- IVRFlowId (FK)
- CallerNumber (NVARCHAR(20))
- SessionStartTime (DATETIME2)
- SessionEndTime (DATETIME2)
- TotalDurationSeconds (INT)
- MenuPathJson (NVARCHAR(MAX)) -- JSON array of visited menus
- InputsJson (NVARCHAR(MAX)) -- JSON array of user inputs
- FinalAction (NVARCHAR(100))
- CreatedDate

---

## Module 5: Core Banking

### 5.1 BankAccounts
Customer bank accounts
- AccountId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- AccountNumber (NVARCHAR(50), UNIQUE)
- CustomerId (FK)
- AccountTypeId (FK)
- AccountName (NVARCHAR(200))
- CurrencyCode (NVARCHAR(10))
- CurrentBalance (DECIMAL(18,2))
- AvailableBalance (DECIMAL(18,2))
- HoldAmount (DECIMAL(18,2))
- OpeningDate (DATE)
- ClosingDate (DATE)
- AccountStatus (NVARCHAR(50)) -- Active, Dormant, Closed, Blocked
- BranchId (FK)
- InterestRate (DECIMAL(5,2))
- MinimumBalance (DECIMAL(18,2))
- OverdraftLimit (DECIMAL(18,2))
- IsJointAccount (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 5.2 AccountTypes
Account type definitions
- AccountTypeId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- AccountTypeName (NVARCHAR(100))
- AccountTypeCode (NVARCHAR(50))
- Description (NVARCHAR(500))
- Category (NVARCHAR(50)) -- Savings, Current, Fixed Deposit
- MinimumBalance (DECIMAL(18,2))
- InterestRate (DECIMAL(5,2))
- MonthlyFee (DECIMAL(10,2))
- AllowsOverdraft (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy

### 5.3 Transactions
Financial transactions
- TransactionId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- TransactionNumber (NVARCHAR(50), UNIQUE)
- AccountId (FK)
- TransactionTypeId (FK)
- TransactionDate (DATETIME2)
- ValueDate (DATE)
- Amount (DECIMAL(18,2))
- TransactionDirection (NVARCHAR(10)) -- Debit, Credit
- BalanceAfter (DECIMAL(18,2))
- Description (NVARCHAR(500))
- ReferenceNumber (NVARCHAR(100))
- RelatedTransactionId (FK to Transactions) -- For reversals
- ToAccountId (FK to BankAccounts) -- For transfers
- BeneficiaryId (FK)
- ChannelType (NVARCHAR(50)) -- ATM, Online, Branch, Mobile
- TransactionStatus (NVARCHAR(50)) -- Pending, Completed, Failed, Reversed
- IsReversed (BIT)
- ReversalReason (NVARCHAR(500))
- AuthorizationCode (NVARCHAR(100))
- ProcessedBy (FK to Users)
- CreatedDate

### 5.4 TransactionTypes
Transaction type definitions
- TransactionTypeId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- TypeName (NVARCHAR(100))
- TypeCode (NVARCHAR(50))
- Category (NVARCHAR(50)) -- Deposit, Withdrawal, Transfer, Fee
- IsDebit (BIT)
- RequiresApproval (BIT)
- ChargeTypeId (FK)
- IsActive (BIT)
- CreatedDate, CreatedBy

### 5.5 Cards
Debit/Credit cards
- CardId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- AccountId (FK)
- CustomerId (FK)
- CardNumber (NVARCHAR(20)) -- Encrypted
- CardType (NVARCHAR(50)) -- Debit, Credit
- CardNetwork (NVARCHAR(50)) -- Visa, Mastercard
- NameOnCard (NVARCHAR(200))
- IssueDate (DATE)
- ExpiryDate (DATE)
- CVV (NVARCHAR(10)) -- Encrypted
- CardStatus (NVARCHAR(50)) -- Active, Blocked, Expired
- DailyLimit (DECIMAL(18,2))
- MonthlyLimit (DECIMAL(18,2))
- IsPinSet (BIT)
- IsContactless (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 5.6 Loans
Customer loans
- LoanId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- LoanNumber (NVARCHAR(50), UNIQUE)
- CustomerId (FK)
- LoanTypeId (FK)
- AccountId (FK) -- Disbursement account
- PrincipalAmount (DECIMAL(18,2))
- InterestRate (DECIMAL(5,2))
- TenureMonths (INT)
- EMIAmount (DECIMAL(18,2))
- StartDate (DATE)
- MaturityDate (DATE)
- DisbursementDate (DATE)
- OutstandingPrincipal (DECIMAL(18,2))
- OutstandingInterest (DECIMAL(18,2))
- TotalOutstanding (DECIMAL(18,2))
- LoanStatus (NVARCHAR(50)) -- Applied, Approved, Disbursed, Active, Closed, Defaulted
- Purpose (NVARCHAR(500))
- CollateralDescription (NVARCHAR(MAX))
- ApprovedBy (FK to Users)
- ApprovalDate (DATE)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 5.7 LoanTypes
Loan product definitions
- LoanTypeId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- LoanTypeName (NVARCHAR(100))
- LoanTypeCode (NVARCHAR(50))
- Description (NVARCHAR(500))
- MinAmount (DECIMAL(18,2))
- MaxAmount (DECIMAL(18,2))
- MinTenureMonths (INT)
- MaxTenureMonths (INT)
- InterestRate (DECIMAL(5,2))
- ProcessingFeePercentage (DECIMAL(5,2))
- RequiresCollateral (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy

### 5.8 LoanRepayments
Loan EMI payments
- RepaymentId (PK, UNIQUEIDENTIFIER)
- LoanId (FK)
- EMINumber (INT)
- DueDate (DATE)
- PaidDate (DATE)
- PrincipalAmount (DECIMAL(18,2))
- InterestAmount (DECIMAL(18,2))
- TotalAmount (DECIMAL(18,2))
- PaidAmount (DECIMAL(18,2))
- LateFee (DECIMAL(18,2))
- PaymentStatus (NVARCHAR(50)) -- Pending, Paid, Overdue
- TransactionId (FK)
- CreatedDate

### 5.9 Beneficiaries
Saved beneficiaries for transfers
- BeneficiaryId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- CustomerId (FK)
- BeneficiaryName (NVARCHAR(200))
- BeneficiaryAccountNumber (NVARCHAR(50))
- BankName (NVARCHAR(200))
- BankCode (NVARCHAR(50))
- BranchName (NVARCHAR(200))
- IFSCCode (NVARCHAR(50))
- SwiftCode (NVARCHAR(50))
- BeneficiaryType (NVARCHAR(50)) -- Within Bank, Other Bank, International
- IsVerified (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 5.10 Branches
Bank branches
- BranchId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- BranchName (NVARCHAR(200))
- BranchCode (NVARCHAR(50), UNIQUE)
- IFSCCode (NVARCHAR(50))
- Address (NVARCHAR(500))
- City, State, Country, PostalCode
- Phone (NVARCHAR(20))
- Email (NVARCHAR(255))
- BranchManagerUserId (FK to Users)
- IsHeadOffice (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 5.11 Charges
Banking charges and fees
- ChargeId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- ChargeName (NVARCHAR(100))
- ChargeCode (NVARCHAR(50))
- ChargeType (NVARCHAR(50)) -- Fixed, Percentage
- Amount (DECIMAL(18,2))
- Percentage (DECIMAL(5,2))
- MinAmount (DECIMAL(18,2))
- MaxAmount (DECIMAL(18,2))
- ApplicableOn (NVARCHAR(50)) -- Transaction, Account, Card
- IsActive (BIT)
- CreatedDate, CreatedBy

---

## Module 6: Subscription & Billing

### 6.1 SubscriptionPlans
SaaS subscription plans
- SubscriptionPlanId (PK, UNIQUEIDENTIFIER)
- PlanName (NVARCHAR(100))
- PlanCode (NVARCHAR(50))
- Description (NVARCHAR(500))
- BillingCycle (NVARCHAR(20)) -- Monthly, Quarterly, Yearly
- Price (DECIMAL(10,2))
- CurrencyCode (NVARCHAR(10))
- MaxUsers (INT)
- MaxStorageGB (INT)
- Features (NVARCHAR(MAX)) -- JSON
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 6.2 Subscriptions
Tenant subscriptions
- SubscriptionId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- SubscriptionPlanId (FK)
- StartDate (DATE)
- EndDate (DATE)
- Status (NVARCHAR(50)) -- Active, Suspended, Cancelled, Expired
- BillingCycle (NVARCHAR(20))
- Amount (DECIMAL(10,2))
- NextBillingDate (DATE)
- AutoRenew (BIT)
- CancellationDate (DATE)
- CancellationReason (NVARCHAR(500))
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 6.3 Invoices
Subscription invoices
- InvoiceId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- SubscriptionId (FK)
- InvoiceNumber (NVARCHAR(50), UNIQUE)
- InvoiceDate (DATE)
- DueDate (DATE)
- SubTotal (DECIMAL(10,2))
- TaxAmount (DECIMAL(10,2))
- DiscountAmount (DECIMAL(10,2))
- TotalAmount (DECIMAL(10,2))
- PaidAmount (DECIMAL(10,2))
- Status (NVARCHAR(50)) -- Draft, Sent, Paid, Overdue, Cancelled
- PaymentDate (DATE)
- PaymentMethodId (FK)
- Notes (NVARCHAR(500))
- CreatedDate, CreatedBy

### 6.4 InvoiceLineItems
Invoice line items
- LineItemId (PK, UNIQUEIDENTIFIER)
- InvoiceId (FK)
- Description (NVARCHAR(500))
- Quantity (DECIMAL(10,2))
- UnitPrice (DECIMAL(10,2))
- Amount (DECIMAL(10,2))
- TaxPercentage (DECIMAL(5,2))
- TaxAmount (DECIMAL(10,2))
- LineTotal (DECIMAL(10,2))

### 6.5 Payments
Payment records
- PaymentId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- InvoiceId (FK)
- PaymentNumber (NVARCHAR(50))
- PaymentDate (DATE)
- Amount (DECIMAL(10,2))
- PaymentMethodId (FK)
- TransactionReference (NVARCHAR(100))
- Status (NVARCHAR(50)) -- Pending, Completed, Failed, Refunded
- Notes (NVARCHAR(500))
- CreatedDate, CreatedBy

### 6.6 PaymentMethods
Payment method configurations
- PaymentMethodId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- MethodType (NVARCHAR(50)) -- CreditCard, DebitCard, BankTransfer, PayPal
- CardholderName (NVARCHAR(200))
- Last4Digits (NVARCHAR(4))
- ExpiryMonth (INT)
- ExpiryYear (INT)
- IsDefault (BIT)
- IsActive (BIT)
- CreatedDate, CreatedBy

---

## Module 7: Workflow & Process Management

### 7.1 WorkflowDefinitions
Workflow process definitions (Camunda integration)
- WorkflowDefinitionId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- WorkflowName (NVARCHAR(200))
- WorkflowKey (NVARCHAR(100))
- Version (INT)
- BpmnXml (NVARCHAR(MAX))
- Category (NVARCHAR(50)) -- Loan Approval, Account Opening, etc.
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 7.2 WorkflowInstances
Running workflow instances
- WorkflowInstanceId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- WorkflowDefinitionId (FK)
- CamundaProcessInstanceId (NVARCHAR(100))
- EntityType (NVARCHAR(50)) -- Loan, Account, etc.
- EntityId (UNIQUEIDENTIFIER)
- Status (NVARCHAR(50)) -- Running, Completed, Terminated
- StartDate (DATETIME2)
- EndDate (DATETIME2)
- StartedBy (FK to Users)
- Variables (NVARCHAR(MAX)) -- JSON
- CreatedDate

### 7.3 WorkflowTasks
User tasks in workflows
- WorkflowTaskId (PK, UNIQUEIDENTIFIER)
- WorkflowInstanceId (FK)
- CamundaTaskId (NVARCHAR(100))
- TaskName (NVARCHAR(200))
- TaskKey (NVARCHAR(100))
- AssignedUserId (FK to Users)
- AssignedRoleId (FK to Roles)
- Status (NVARCHAR(50)) -- Pending, Completed, Cancelled
- Priority (INT)
- DueDate (DATETIME2)
- CompletedDate (DATETIME2)
- CompletedBy (FK to Users)
- FormData (NVARCHAR(MAX)) -- JSON
- Comments (NVARCHAR(MAX))
- CreatedDate

### 7.4 WorkflowHistory
Workflow audit trail
- HistoryId (PK, BIGINT, IDENTITY)
- WorkflowInstanceId (FK)
- WorkflowTaskId (FK)
- Action (NVARCHAR(100))
- ActorUserId (FK to Users)
- OldStatus (NVARCHAR(50))
- NewStatus (NVARCHAR(50))
- Comments (NVARCHAR(MAX))
- Timestamp (DATETIME2)

---

## Module 8: Notifications

### 8.1 NotificationTemplates
Notification message templates
- TemplateId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- TemplateName (NVARCHAR(100))
- TemplateCode (NVARCHAR(50))
- NotificationType (NVARCHAR(50)) -- Email, SMS, Push, InApp
- Subject (NVARCHAR(200))
- BodyTemplate (NVARCHAR(MAX))
- Variables (NVARCHAR(500)) -- Comma-separated
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

### 8.2 Notifications
Notification queue
- NotificationId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- UserId (FK)
- NotificationType (NVARCHAR(50))
- TemplateId (FK)
- Subject (NVARCHAR(200))
- Body (NVARCHAR(MAX))
- RecipientEmail (NVARCHAR(255))
- RecipientPhone (NVARCHAR(20))
- Status (NVARCHAR(50)) -- Pending, Sent, Failed, Read
- SentDate (DATETIME2)
- ReadDate (DATETIME2)
- ErrorMessage (NVARCHAR(500))
- RetryCount (INT)
- Priority (INT)
- RelatedEntityType (NVARCHAR(50))
- RelatedEntityId (UNIQUEIDENTIFIER)
- CreatedDate

---

## Module 9: System Configuration

### 9.1 SystemSettings
System-wide settings
- SettingId (PK, UNIQUEIDENTIFIER)
- TenantId (FK) -- NULL for global settings
- SettingKey (NVARCHAR(100), UNIQUE)
- SettingValue (NVARCHAR(MAX))
- DataType (NVARCHAR(50))
- Category (NVARCHAR(50))
- Description (NVARCHAR(500))
- IsEncrypted (BIT)
- ModifiedDate, ModifiedBy

### 9.2 EmailConfiguration
Email service settings
- EmailConfigId (PK, UNIQUEIDENTIFIER)
- TenantId (FK)
- SMTPHost (NVARCHAR(200))
- SMTPPort (INT)
- Username (NVARCHAR(200))
- Password (NVARCHAR(500)) -- Encrypted
- EnableSSL (BIT)
- FromEmail (NVARCHAR(255))
- FromName (NVARCHAR(200))
- IsActive (BIT)
- CreatedDate, CreatedBy
- ModifiedDate, ModifiedBy

---

## Key Design Principles

1. **Multi-Tenancy**: TenantId in all core tables for data isolation
2. **Soft Delete**: IsActive flags instead of hard deletes
3. **Audit Trail**: CreatedDate, CreatedBy, ModifiedDate, ModifiedBy in all tables
4. **Security**: Password hashing, encryption for sensitive data
5. **Scalability**: GUID primary keys, proper indexing strategy
6. **Relationships**: Foreign keys with ON DELETE NO ACTION to prevent cascading deletes
7. **Normalization**: Properly normalized to 3NF
8. **Flexibility**: JSON columns for extensible data
9. **Performance**: Appropriate indexes on foreign keys and commonly queried columns

---

## Indexing Strategy

### Primary Keys
All tables have clustered index on PK (UNIQUEIDENTIFIER or BIGINT IDENTITY)

### Recommended Non-Clustered Indexes
- TenantId on all multi-tenant tables
- Email on Users (UNIQUE)
- Username on Users (UNIQUE)
- CustomerCode on Customers
- AccountNumber on BankAccounts
- TransactionNumber on Transactions
- TransactionDate on Transactions
- CallStartTime on CallRecords
- InvoiceNumber on Invoices
- Foreign key columns
- Status columns frequently used in WHERE clauses
- Date columns used in range queries

---

This design provides a comprehensive foundation for your SaaS platform and can be extended as needed. The schema supports multi-tenancy, scalability, and integrates all four major modules (CRM, CTI, IVR, Core Banking).
