-- =============================================
-- Author:	Ted S
-- Create date: 20180613
-- Description:	[sp_MessagesSelectAllArchiveTypes] 
-- =============================================
CREATE PROCEDURE [dbo].[sp_MessagesSelectAllArchiveTypes]
	-- Add the parameters for the stored procedure here

	
AS
BEGIN
	SET NOCOUNT ON;

   SELECT DISTINCT [TypeName] 
	FROM
		(SELECT  
		[Limit1].[TypeName] AS [TypeName]
		FROM
		(SELECT DISTINCT [Extent1].Name AS [TypeName]
			FROM [$(MessageArchive)].[dbo].ArchiveType AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		) as ArchiveChunk

	UNION

	SELECT * 
	FROM
		(SELECT
		[Limit1].[TypeName] AS [TypeName]
		FROM   
		(SELECT DISTINCT [Extent1].Name AS [TypeName]
			FROM [dbo].ArchiveType AS [Extent1] WITH(NOLOCK)) AS [Limit1]
		) AS HistoryChunk

END










GO

