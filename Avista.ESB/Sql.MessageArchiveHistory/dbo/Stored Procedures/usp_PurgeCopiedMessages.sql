

CREATE PROCEDURE [dbo].[usp_PurgeCopiedMessages]
(
	@BatchSize INT = 100
)
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @RowCount INT = 1;

	DECLARE @MessageIds TABLE (MessageId UNIQUEIDENTIFIER PRIMARY KEY)

	WHILE (@RowCount > 0)
	BEGIN
		DELETE FROM @MessageIds

		INSERT INTO @MessageIds (MessageId)
		SELECT TOP(@BatchSize) MessageId FROM [MessageArchiveHistory].[dbo].[CopiedMessage] WITH(NOLOCK) 

		SET @RowCount = @@ROWCOUNT;

		BEGIN TRANSACTION;
		DELETE FROM [MessageArchive].[dbo].[Message] WITH (ROWLOCK) WHERE MessageId IN (SELECT MessageId FROM @MessageIds)

		DELETE FROM [MessageArchiveHistory].[dbo].[CopiedMessage] WITH (ROWLOCK) WHERE MessageId IN (SELECT MessageId FROM @MessageIds)
		COMMIT TRANSACTION;
	END
END

GO
