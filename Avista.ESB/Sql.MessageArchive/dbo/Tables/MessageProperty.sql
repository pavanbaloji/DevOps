CREATE TABLE [dbo].[MessageProperty] (
    [MessageId]     UNIQUEIDENTIFIER NOT NULL,    
    [ContextData]         NVARCHAR (MAX)   NOT NULL,  
	[ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_MessageProperty] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MessageProperty_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[Message] ([MessageId]) ON DELETE CASCADE
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [U_MessageIDPropertyIndex]
    ON [dbo].[MessageProperty]([MessageId] ASC);
GO



