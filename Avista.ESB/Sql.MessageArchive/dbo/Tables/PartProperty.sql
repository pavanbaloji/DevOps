CREATE TABLE [dbo].[PartProperty] (
    [PartId]        UNIQUEIDENTIFIER NOT NULL,
    [PropertyIndex] INT              NOT NULL,
    [Namespace]     NVARCHAR (256)   NOT NULL,
    [Name]          NVARCHAR (256)   NOT NULL,
    [Value]         NVARCHAR (MAX)   NOT NULL,
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_PartProperty] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PartProperty_PartId] FOREIGN KEY ([PartId]) REFERENCES [dbo].[Part] ([PartId]) ON DELETE CASCADE
);
GO

	CREATE UNIQUE NONCLUSTERED INDEX [U_PartIDPropertyIndex]
    ON [dbo].[PartProperty]([PartId] ASC, [PropertyIndex] ASC);
GO


