-- =============================================
-- Author:	Ted S
-- Create date: 20180613
-- Description:	[sp_MessagesSelectByArchiveType] 
-- =============================================
CREATE PROCEDURE [dbo].[sp_MessagesSelectByArchiveType]
	-- Add the parameters for the stored procedure here
	@archiveType varchar(50) = '',
	@maxResults int = 100
	
AS
BEGIN
	SET NOCOUNT ON;

SELECT TOP (@maxResults) *
FROM
(
   SELECT * 
	FROM
		(SELECT 
		'A' AS [TblSource], 
		[Limit1].[MessageId] AS [MessageId], 
		[Limit1].[InterchangeId] AS [InterchangeId], 
		[Limit1].[MessageType] AS [MessageType], 
		[Limit1].[ActivityId] AS [ActivityId], 
		[Limit1].[Tag] AS [Tag], 
		[Limit1].[InsertedDate] AS [InsertedDate], 
		[Limit1].[ExpiryDate] AS [ExpiryDate], 
		[Extent3].[Name] AS [ArchiveType], 
		[Extent3].[Id] AS [ArchiveTypeId],
		[Extent4].[Name] AS [SourceSystem], 
		[Extent5].[Name] AS [TargetSystem], 
		[Limit1].[Description] AS [Description], 
		[Extent2].[MessageId] AS [PartMessageId], 
		[Extent2].[PartId] AS [PartId], 
		CAST(LEN([Extent2].[TextData]) AS int) AS [TextDataLength], 
		SUBSTRING([Extent2].[TextData], 0 + 1, 40) AS [TextDataAbbreviated], 
		[Extent2].[TextData] AS [TextData]
		FROM   
		(SELECT [Extent1].[MessageId] AS [MessageId], [Extent1].[InterchangeId] AS [InterchangeId],
		[Extent1].[MessageType] AS [MessageType], [Extent1].[ActivityId] AS [ActivityId], [Extent1].[Tag] AS [Tag], 
		[Extent1].[InsertedDate] AS [InsertedDate], [Extent1].[ExpiryDate] AS [ExpiryDate], 
		[Extent1].[ArchiveTypeId] AS [ArchiveTypeId], [Extent1].[SourceSystemId] AS [SourceSystemId],
		[Extent1].[TargetSystemId] AS [TargetSystemId], [Extent1].[Description] AS [Description]
			FROM [$(MessageArchive)].[dbo].[Message] AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		INNER JOIN [$(MessageArchive)].[dbo].[Part] AS [Extent2] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent2].[MessageId]
		LEFT JOIN [$(MessageArchive)].[dbo].[ArchiveType] AS [Extent3] WITH(NOLOCK) ON [Limit1].[ArchiveTypeId] = [Extent3].[Id]
		LEFT JOIN [$(MessageArchive)].[dbo].[Endpoint] AS [Extent4] WITH(NOLOCK) ON [Limit1].[SourceSystemId] = [Extent4].[Id]
		LEFT JOIN [$(MessageArchive)].[dbo].[Endpoint] AS [Extent5] WITH(NOLOCK) ON [Limit1].[TargetSystemId] = [Extent5].[Id]
		WHERE [Extent3].[Name] = @archiveType) as ArchiveChunk

	UNION

	SELECT * 
	FROM
		(SELECT
		'H' AS [TblSource], 
		[Limit1].[MessageId] AS [MessageId], 
		[Limit1].[InterchangeId] AS [InterchangeId], 
		[Limit1].[MessageType] AS [MessageType], 
		[Limit1].[ActivityId] AS [ActivityId], 
		[Limit1].[Tag] AS [Tag], 
		[Limit1].[InsertedDate] AS [InsertedDate], 
		[Limit1].[ExpiryDate] AS [ExpiryDate], 
		[Extent3].[Name] AS [ArchiveType], 
		[Extent3].[Id] AS [ArchiveTypeId],
		[Extent4].[Name] AS [SourceSystem], 
		[Extent5].[Name] AS [TargetSystem], 
		[Limit1].[Description] AS [Description], 
		[Extent2].[MessageId] AS [PartMessageId], 
		[Extent2].[PartId] AS [PartId], 
		CAST(LEN([Extent2].[TextData]) AS int) AS [TextDataLength], 
		SUBSTRING([Extent2].[TextData], 0 + 1, 40) AS [TextDataAbbreviated], 
		[Extent2].[TextData] AS [TextData]
		FROM   
		(SELECT [Extent1].[MessageId] AS [MessageId], [Extent1].[InterchangeId] AS [InterchangeId], 
		[Extent1].[MessageType] AS [MessageType], [Extent1].[ActivityId] AS [ActivityId], 
		[Extent1].[Tag] AS [Tag], [Extent1].[InsertedDate] AS [InsertedDate], [Extent1].[ExpiryDate] AS [ExpiryDate], 
		[Extent1].[ArchiveTypeId] AS [ArchiveTypeId], [Extent1].[SourceSystemId] AS [SourceSystemId], 
		[Extent1].[TargetSystemId] AS [TargetSystemId], [Extent1].[Description] AS [Description]
			FROM [dbo].[Message] AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		INNER JOIN [dbo].[Part] AS [Extent2] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent2].[MessageId]
		LEFT JOIN [dbo].[ArchiveType] AS [Extent3] WITH(NOLOCK) ON [Limit1].[ArchiveTypeId] = [Extent3].[Id]
		LEFT JOIN [dbo].[Endpoint] AS [Extent4] WITH(NOLOCK) ON [Limit1].[SourceSystemId] = [Extent4].[Id]
		LEFT JOIN [dbo].[Endpoint] AS [Extent5] WITH(NOLOCK) ON [Limit1].[TargetSystemId] = [Extent5].[Id]
		WHERE [Extent3].[Name] = @archiveType) AS HistoryChunk
) AS Combined
END







GO

