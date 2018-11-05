﻿CREATE TABLE [dbo].[MessageProperty](
	[MessageId] [uniqueidentifier] NOT NULL,
	[ContextData] [nvarchar](max) NOT NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_MessageProperty] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[MessageProperty]  WITH CHECK ADD  CONSTRAINT [FK_MessageProperty_MessageId] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[MessageProperty] CHECK CONSTRAINT [FK_MessageProperty_MessageId]
GO
CREATE UNIQUE NONCLUSTERED INDEX [U_MessageIDPropertyIndex] ON [dbo].[MessageProperty]
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO