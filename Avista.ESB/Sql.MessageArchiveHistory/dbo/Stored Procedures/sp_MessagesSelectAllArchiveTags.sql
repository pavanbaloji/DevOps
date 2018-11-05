-- =============================================
-- Author:	Ted S
-- Create date: 20180613
-- Description:	sp_MessagesSelectAllArchiveTypes 
-- =============================================
CREATE PROCEDURE [dbo].[sp_MessagesSelectAllArchiveTags]
	-- Add the parameters for the stored procedure here

	
AS
BEGIN
	SET NOCOUNT ON;

   SELECT DISTINCT [TagName] 
	FROM
		(SELECT 
		[Limit1].[Tag] AS [TagName]
		FROM
		(SELECT [Extent1].[Tag] AS [Tag]
			FROM [$(MessageArchive)].[dbo].[Message] AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		) as ArchiveChunk

	UNION

	SELECT * 
	FROM
		(SELECT
		[Limit1].[Tag] AS [TagName]
		FROM   
		(SELECT [Extent1].[Tag] AS [Tag]
			FROM [dbo].[Message] AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		) AS HistoryChunk

END








GO

