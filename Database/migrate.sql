IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [ImgPath] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [FullName] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        [Address] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [PaymentsInfo] (
        [Id] int NOT NULL IDENTITY,
        [ApiKey] nvarchar(max) NULL,
        [IntegrationId] nvarchar(max) NULL,
        [IframeId] nvarchar(max) NULL,
        [HmacKey] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_PaymentsInfo] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [ScheduledJobs] (
        [Id] int NOT NULL IDENTITY,
        [HangfireJobId] nvarchar(max) NOT NULL,
        [EntityType] nvarchar(max) NOT NULL,
        [EntityId] int NOT NULL,
        [JobType] nvarchar(max) NOT NULL,
        [ScheduledFor] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CompletedAt] datetime2 NULL,
        [ErrorMessage] nvarchar(max) NULL,
        CONSTRAINT [PK_ScheduledJobs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [Organizations] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [PaymentId] int NULL,
        [AdminUserId] nvarchar(450) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_Organizations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Organizations_AspNetUsers_AdminUserId] FOREIGN KEY ([AdminUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Organizations_PaymentsInfo_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [PaymentsInfo] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [Campaigns] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [ImgPath] nvarchar(max) NULL,
        [Target] float NULL,
        [Achieved] float NULL,
        [Status] int NULL,
        [Type] int NULL,
        [OrganizationId] int NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        [Deadline] datetime2 NULL,
        [CompletionDate] datetime2 NULL,
        [CampaignType] nvarchar(8) NOT NULL,
        [CreatorOrganizationId] int NULL,
        CONSTRAINT [PK_Campaigns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Campaigns_Organizations_CreatorOrganizationId] FOREIGN KEY ([CreatorOrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Campaigns_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [DonatedItems] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [ItemCategory] int NULL,
        [IsAvailable] bit NULL,
        [OrganizationId] int NOT NULL,
        [DonorId] nvarchar(450) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NOT NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_DonatedItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DonatedItems_AspNetUsers_DonorId] FOREIGN KEY ([DonorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DonatedItems_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [OrganizationContactMethods] (
        [Id] int NOT NULL IDENTITY,
        [Value] nvarchar(max) NULL,
        [Type] int NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        [CompanyId] int NULL,
        CONSTRAINT [PK_OrganizationContactMethods] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationContactMethods_Organizations_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Organizations] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [OrganizationRoles] (
        [Id] int NOT NULL IDENTITY,
        [OrganizationId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Role] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NOT NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_OrganizationRoles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrganizationRoles_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [Donations] (
        [Id] int NOT NULL IDENTITY,
        [Amount] float NULL,
        [UserId] nvarchar(450) NOT NULL,
        [CampaignId] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NOT NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_Donations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Donations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Donations_Campaigns_CampaignId] FOREIGN KEY ([CampaignId]) REFERENCES [Campaigns] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [SharedCampaignInvites] (
        [Id] int NOT NULL IDENTITY,
        [SharedCampaignId] int NOT NULL,
        [OrganizationId] int NOT NULL,
        [InvitedByUserId] nvarchar(450) NOT NULL,
        [Status] int NOT NULL,
        [RespondedAt] datetime2 NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NOT NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_SharedCampaignInvites] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SharedCampaignInvites_AspNetUsers_InvitedByUserId] FOREIGN KEY ([InvitedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SharedCampaignInvites_Campaigns_SharedCampaignId] FOREIGN KEY ([SharedCampaignId]) REFERENCES [Campaigns] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SharedCampaignInvites_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [SharedCampaignOrganizations] (
        [OrganizationsId] int NOT NULL,
        [SharedCampaignsId] int NOT NULL,
        CONSTRAINT [PK_SharedCampaignOrganizations] PRIMARY KEY ([OrganizationsId], [SharedCampaignsId]),
        CONSTRAINT [FK_SharedCampaignOrganizations_Campaigns_SharedCampaignsId] FOREIGN KEY ([SharedCampaignsId]) REFERENCES [Campaigns] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_SharedCampaignOrganizations_Organizations_OrganizationsId] FOREIGN KEY ([OrganizationsId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [Attachments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [Path] nvarchar(max) NULL,
        [FileSize] bigint NULL,
        [ContentType] nvarchar(max) NULL,
        [IsItemAttachment] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        [DonatedItemId] int NULL,
        [DonatedItemId1] int NULL,
        CONSTRAINT [PK_Attachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Attachments_DonatedItems_DonatedItemId] FOREIGN KEY ([DonatedItemId]) REFERENCES [DonatedItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Attachments_DonatedItems_DonatedItemId1] FOREIGN KEY ([DonatedItemId1]) REFERENCES [DonatedItems] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE TABLE [ItemImages] (
        [Id] int NOT NULL IDENTITY,
        [Path] nvarchar(max) NULL,
        [DonatedItemId] int NULL,
        [IsMain] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetime2 NULL,
        [RegistrationDate] datetime2 NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_ItemImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ItemImages_DonatedItems_DonatedItemId] FOREIGN KEY ([DonatedItemId]) REFERENCES [DonatedItems] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Attachments_DonatedItemId] ON [Attachments] ([DonatedItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Attachments_DonatedItemId1] ON [Attachments] ([DonatedItemId1]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Campaigns_CreatorOrganizationId] ON [Campaigns] ([CreatorOrganizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Campaigns_OrganizationId] ON [Campaigns] ([OrganizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_DonatedItems_DonorId] ON [DonatedItems] ([DonorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_DonatedItems_OrganizationId] ON [DonatedItems] ([OrganizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Donations_CampaignId] ON [Donations] ([CampaignId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Donations_UserId] ON [Donations] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_ItemImages_DonatedItemId] ON [ItemImages] ([DonatedItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_OrganizationContactMethods_CompanyId] ON [OrganizationContactMethods] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_OrganizationRoles_OrganizationId] ON [OrganizationRoles] ([OrganizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_OrganizationRoles_UserId] ON [OrganizationRoles] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Organizations_AdminUserId] ON [Organizations] ([AdminUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_Organizations_PaymentId] ON [Organizations] ([PaymentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_SharedCampaignInvites_InvitedByUserId] ON [SharedCampaignInvites] ([InvitedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_SharedCampaignInvites_OrganizationId] ON [SharedCampaignInvites] ([OrganizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_SharedCampaignInvites_SharedCampaignId] ON [SharedCampaignInvites] ([SharedCampaignId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    CREATE INDEX [IX_SharedCampaignOrganizations_SharedCampaignsId] ON [SharedCampaignOrganizations] ([SharedCampaignsId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824054328_initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260824054328_initial', N'8.0.0');
END;
GO

COMMIT;
GO

