CREATE TABLE [dbo].[EventMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[PartialMessage] [nvarchar](MAX) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[MachineName] [nvarchar](50) NOT NULL,
	[PartialException] [nvarchar](MAX) NULL,
	[Exception] [nvarchar](max) NULL,
	[EventID] [int] NOT NULL,
	[EventSource] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_Logged] ON [dbo].[EventMessage]
(
	[Logged] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO