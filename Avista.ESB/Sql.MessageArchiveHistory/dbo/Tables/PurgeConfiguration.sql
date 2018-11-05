CREATE TABLE [dbo].[PurgeConfiguration]
(
	[Tag] [nvarchar](50) NOT NULL,
	[FinalExpiryHours] [int] NOT NULL,
	[ErrorExpiryHours] [int] NOT NULL,
	[UnknownExpiryHours] [int] NOT NULL
)
