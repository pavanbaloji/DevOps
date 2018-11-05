CREATE TABLE [dbo].[ArchiveType] (
    [Id]            INT           NOT NULL,
    [Name]          NVARCHAR (50) NOT NULL,
    [Active]        BIT           NOT NULL,
    [DefaultExpiry] INT           NOT NULL,
    CONSTRAINT [PK_ArchiveType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

