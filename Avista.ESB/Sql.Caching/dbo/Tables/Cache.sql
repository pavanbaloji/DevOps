CREATE TABLE [dbo].[Cache](
	[ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Cache_ID] DEFAULT (newsequentialid()),
	[Key] [nvarchar](250) NOT NULL,
	[Value] [varbinary](max) NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_Cache_Created]  DEFAULT (getdate()),
	[LastAccess] [datetime] NOT NULL,
	[SlidingExpirationTimeInMinutes] [int] NULL,
	[AbsoluteExpirationTime] [datetime] NULL,
	[ObjectType] [nvarchar](250) NOT NULL
CONSTRAINT [PK_Cache] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[Key] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
