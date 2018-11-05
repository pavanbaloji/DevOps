CREATE TABLE [dbo].[Message] (
    [MessageId]      UNIQUEIDENTIFIER CONSTRAINT [DF_Message_MessageId] DEFAULT (newid()) NOT NULL,
    [InterchangeId]  NVARCHAR (36)    NULL,
    [MessageType]    NVARCHAR (256)   NULL,
    [ActivityId]     NVARCHAR (36)    NULL,
    [Tag]            NVARCHAR (50)    NULL,
    [InsertedDate]   DATETIME         CONSTRAINT [DF_Message_InsertedDate] DEFAULT (getutcdate()) NULL,
    [ExpiryDate]     DATETIME         NULL,
    [ArchiveTypeId]  INT              NULL,
    [SourceSystemId] INT              NULL,
    [TargetSystemId] INT              NULL,
    [Description]    NVARCHAR (250)   NULL,
    [ID]             BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Message_ArchiveTypeId] FOREIGN KEY ([ArchiveTypeId]) REFERENCES [dbo].[ArchiveType] ([Id]),
    CONSTRAINT [FK_Message_SourceEndpointId] FOREIGN KEY ([SourceSystemId]) REFERENCES [dbo].[Endpoint] ([Id]),
    CONSTRAINT [FK_Message_TargetEndpointId] FOREIGN KEY ([TargetSystemId]) REFERENCES [dbo].[Endpoint] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_InsertedDate]
    ON [dbo].[Message]([InsertedDate] DESC);

GO
CREATE NONCLUSTERED INDEX [IX_Message_ArchiveType]
    ON [dbo].[Message]([ArchiveTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Message_SourceSystem]
    ON [dbo].[Message]([SourceSystemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Message_TargetSystem]
    ON [dbo].[Message]([TargetSystemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Message_ExpiryDate]
    ON [dbo].[Message]([ExpiryDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Message_InterchangeId_0C7C0]
    ON [dbo].[Message]([InterchangeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Message_Tag_86DD3]
    ON [dbo].[Message]([Tag] ASC)
    INCLUDE([MessageId], [InterchangeId], [InsertedDate], [ExpiryDate], [ArchiveTypeId], [SourceSystemId], [TargetSystemId], [Description]);


GO

CREATE NONCLUSTERED INDEX [IX_Message_InterchangeId_MessageId_InsertedDate_ExpiryDate]
 ON [dbo].[Message] ([InterchangeId])
 INCLUDE ([MessageId],[InsertedDate],[ExpiryDate])

 GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180518-103730]
    ON [dbo].[Message]([InsertedDate] DESC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [U_MessageID]
    ON [dbo].[Message]([MessageId] ASC);
GO