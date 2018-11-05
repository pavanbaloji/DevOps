-- =============================================
-- Author:	Ted S
-- Create date: 20180613
-- Description:	[sp_MessagesSelectByMessageId] 
-- =============================================
CREATE PROCEDURE [dbo].[sp_MessagesSelectByMessageId]
	-- Add the parameters for the stored procedure here
	@messageId varchar(40) = ''
	
AS
BEGIN
	SET NOCOUNT ON;

   SELECT TOP 100 * 
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
		[Extent2].[MessageId] AS [PropMessageId], 
		[Extent2].[ContextData] AS [PropContextData],
		[Extent6].[PartId] AS [PartId],
		[Extent6].[TextData] AS [TextData]
		FROM   
		(SELECT [Extent1].[MessageId] AS [MessageId], [Extent1].[InterchangeId] AS [InterchangeId],
		[Extent1].[MessageType] AS [MessageType], [Extent1].[ActivityId] AS [ActivityId], [Extent1].[Tag] AS [Tag], 
		[Extent1].[InsertedDate] AS [InsertedDate], [Extent1].[ExpiryDate] AS [ExpiryDate], 
		[Extent1].[ArchiveTypeId] AS [ArchiveTypeId], [Extent1].[SourceSystemId] AS [SourceSystemId],
		[Extent1].[TargetSystemId] AS [TargetSystemId], [Extent1].[Description] AS [Description]
			FROM [$(MessageArchive)].[dbo].[Message]  AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		INNER JOIN [$(MessageArchive)].[dbo].[Part] AS [Extent6] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent6].[MessageId]
		INNER JOIN [$(MessageArchive)].[dbo].[MessageProperty] AS [Extent2] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent2].[MessageId]
		LEFT JOIN [$(MessageArchive)].[dbo].[ArchiveType] AS [Extent3] WITH(NOLOCK) ON [Limit1].[ArchiveTypeId] = [Extent3].[Id]
		LEFT JOIN [$(MessageArchive)].[dbo].[Endpoint] AS [Extent4] WITH(NOLOCK) ON [Limit1].[SourceSystemId] = [Extent4].[Id]
		LEFT JOIN [$(MessageArchive)].[dbo].[Endpoint] AS [Extent5] WITH(NOLOCK) ON [Limit1].[TargetSystemId] = [Extent5].[Id]
		WHERE [Limit1].MessageId = @messageId) as ArchiveChunk 

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
		[Extent2].[MessageId] AS [PropMessageId], 
		[Extent2].[ContextData] AS [PropContextData],
		[Extent6].[PartId] AS [PartId],
		[Extent6].[TextData] AS [TextData]
		FROM   
		(SELECT [Extent1].[MessageId] AS [MessageId], [Extent1].[InterchangeId] AS [InterchangeId], 
		[Extent1].[MessageType] AS [MessageType], [Extent1].[ActivityId] AS [ActivityId], 
		[Extent1].[Tag] AS [Tag], [Extent1].[InsertedDate] AS [InsertedDate], [Extent1].[ExpiryDate] AS [ExpiryDate], 
		[Extent1].[ArchiveTypeId] AS [ArchiveTypeId], [Extent1].[SourceSystemId] AS [SourceSystemId], 
		[Extent1].[TargetSystemId] AS [TargetSystemId], [Extent1].[Description] AS [Description]
			FROM [dbo].[Message] AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		INNER JOIN [dbo].[Part] AS [Extent6] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent6].[MessageId]
		INNER JOIN [dbo].[MessageProperty] AS [Extent2] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent2].[MessageId]
		LEFT JOIN [dbo].[ArchiveType] AS [Extent3] WITH(NOLOCK) ON [Limit1].[ArchiveTypeId] = [Extent3].[Id]
		LEFT JOIN [dbo].[Endpoint] AS [Extent4] WITH(NOLOCK) ON [Limit1].[SourceSystemId] = [Extent4].[Id]
		LEFT JOIN [dbo].[Endpoint] AS [Extent5] WITH(NOLOCK) ON [Limit1].[TargetSystemId] = [Extent5].[Id]
		WHERE [Limit1].MessageId = @messageId) AS HistoryChunk

END





GO

