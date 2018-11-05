-- =============================================
-- Author:	Ted S
-- Create date: 20180615
-- Description:	[sp_MessagesSearch] 
-- =============================================
CREATE PROCEDURE [dbo].[sp_MessagesSearch]
	-- Add the parameters for the stored procedure here
	@archiveType nvarchar(50) = null,
	@tag nvarchar(50) = null,
	@startUtc dateTime = null,
	@endUtc dateTime = null,
	@text nvarchar(100) = null,
	@maxResults int = null,
	@queryStmt nvarchar(max) output

AS
BEGIN
	SET NOCOUNT ON;

	declare @now datetime = GETUTCDATE()

	if (@maxResults is null)
		set @maxResults = 250

	if (@startUtc is null)
		set @startUtc =  DATEADD(hour,-4,@now)

	if (@endUtc is null)
		set @endUtc = @now
	   

	DECLARE @archiveSelectStmt nvarchar(2000) = NULL
	DECLARE @historySelectStmt nvarchar(2000) = NULL
	DECLARE @archiveFullStmt nvarchar(max) = NULL
	DECLARE @historyFullStmt nvarchar(max) = NULL
	DECLARE @query nvarchar(max) = NULL
	DECLARE @whereClause nvarchar(2000) = NULL
	

	if (@archiveType is not null)
		begin
			set @whereClause = N'([Extent3].[Name] = @archiveType)'
		end
	if (@tag is not null)
		 begin
			if (@whereClause is not null)
				set @whereClause = @whereClause + N' AND ([Limit1].[Tag] LIKE ''%' + @tag + '%'')'
			else
				set @whereClause = N'([Limit1].[Tag] LIKE ''%' + @tag + '%'')'
		 end
	if (@startUtc is not null)
		 Begin
			if (@whereClause is not null)
				set @whereClause = @whereClause + N' AND ([Limit1].[InsertedDate] >= @startUtc)'
			else
				set @whereClause = N'([Limit1].[InsertedDate] >= @startUtc)'
		 End
	if (@endUtc is not null)
		 Begin
	 		if (@whereClause is not null)
				set @whereClause = @whereClause + N' AND ([Limit1].[InsertedDate] <= @endUtc)'
			else
				set @whereClause = N'([Limit1].[InsertedDate] <= @endUtc)'
		 End
	if (@text is not null)
	 Begin
	 	if (@whereClause is not null)
			set @whereClause = @whereClause + N' AND ([Extent2].[TextData] LIKE ''%' + @text + '%'')'
		else
		    set @whereClause = N'([Extent2].[TextData] LIKE ''%' + @text + '%'')'
	 End
	

	SET @archiveSelectStmt = N'SELECT 
		''A'' AS [TblSource], 
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
			FROM [MessageArchive].[dbo].[Message] AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		INNER JOIN [MessageArchive].[dbo].[Part] AS [Extent2] WITH(NOLOCK) ON [Limit1].[MessageId] = [Extent2].[MessageId]
		LEFT JOIN [MessageArchive].[dbo].[ArchiveType] AS [Extent3] WITH(NOLOCK) ON [Limit1].[ArchiveTypeId] = [Extent3].[Id]
		LEFT JOIN [MessageArchive].[dbo].[Endpoint] AS [Extent4] WITH(NOLOCK) ON [Limit1].[SourceSystemId] = [Extent4].[Id]
		LEFT JOIN [MessageArchive].[dbo].[Endpoint] AS [Extent5] WITH(NOLOCK) ON [Limit1].[TargetSystemId] = [Extent5].[Id]'

	SET @historySelectStmt = N'SELECT
		''H'' AS [TblSource], 
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
		LEFT JOIN [dbo].[Endpoint] AS [Extent5] WITH(NOLOCK) ON [Limit1].[TargetSystemId] = [Extent5].[Id]'

	if (@whereClause is not null)
	   set @whereClause = N' WHERE ' + @whereClause

	SET @archiveFullStmt = @archiveSelectStmt + ISNULL(@whereClause, '')
	SET @historyFullStmt = @historySelectStmt + ISNULL(@whereClause, '')

	
	SET @query = N'Select TOP(@maxResults) * FROM 
					(SELECT * FROM (' + @archiveFullStmt + ') AS ArchiveChunk 
					UNION
					SELECT * FROM (' + @historyFullStmt + ') AS HistoryChunk) AS C'

	SET @queryStmt = @query

	DECLARE @spExecParams nvarchar(150) = 
		N'@archiveType nvarchar(50),
		  @startUtc datetime, 
		  @endUtc datetime,
		  @maxResults int'

	PRINT @query
   	EXEC sp_executesql 
		@query, 
		@spExecParams,
		@archiveType,
		@startUtc, 
		@endUtc,
		@maxResults

END































GO

