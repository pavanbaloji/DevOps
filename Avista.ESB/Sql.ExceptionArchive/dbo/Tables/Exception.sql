CREATE TABLE [dbo].[Exception] (
    [ExceptionId]      NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [ExceptionMessage] NVARCHAR (MAX) NOT NULL,
    [InsertedDate]     DATETIME       NOT NULL,
    [ExpiryDate]       DATETIME       NOT NULL,
    [EventId]          INT            NULL,
    [EventType]        NVARCHAR (50)  NULL,
    [MachineName]      NVARCHAR (50)  NULL,
    [StackTrace]       NVARCHAR (MAX) NULL,
    [Username]         NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Exception] PRIMARY KEY CLUSTERED ([ExceptionId] ASC)
);

