CREATE TABLE [dbo].[Part] (
    [MessageId]   UNIQUEIDENTIFIER NOT NULL,
    [PartId]      UNIQUEIDENTIFIER CONSTRAINT [DF_Part_PartId] DEFAULT (newid()) NOT NULL,
    [PartName]    NVARCHAR (256)   NOT NULL,
    [PartIndex]   INT              NOT NULL,
    [ContentType] NVARCHAR (50)    NOT NULL,
    [CharSet]     NVARCHAR (50)    NOT NULL,
    [TextData]    NVARCHAR (MAX)   NULL,
    [ImageData]   IMAGE            NULL,
    [ID]          BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Part] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Part_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[Message] ([MessageId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Part_MessageId_PartId]
    ON [dbo].[Part]([MessageId] ASC)
    INCLUDE([PartId]);
	
GO

CREATE UNIQUE NONCLUSTERED INDEX [U_PartID]
    ON [dbo].[Part]([PartId] ASC);
GO
