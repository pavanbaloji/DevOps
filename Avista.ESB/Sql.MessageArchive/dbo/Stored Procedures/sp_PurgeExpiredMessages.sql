
-- ****************************************************************************
-- Stored Procedure:  sp_PurgeExpiredMessages
-- ****************************************************************************
CREATE PROCEDURE [dbo].[sp_PurgeExpiredMessages]
(
	@Debug BIT = 0
)
AS

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
 
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
 
DECLARE @Log NVARCHAR(250)
DECLARE @CurrentDate DATETIME = GETUTCDATE()
DECLARE @MessageIds TABLE (MessageId UNIQUEIDENTIFIER NOT NULL)

DECLARE @LoopCtr INT = 0
DECLARE @DeleteCount FLOAT = (SELECT COUNT(*) FROM [MessageArchive].[dbo].[Message] WITH(NOLOCK) WHERE ExpiryDate < @CurrentDate)
DECLARE @MaxLoopCount INT = CEILING(@DeleteCount/100)

IF (@Debug = 1)
BEGIN
	SET @Log = 'Current Date: ' + CONVERT(NVARCHAR, @CurrentDate, 120)
	RAISERROR (@Log, 10, 1) WITH NOWAIT
	SET @Log = 'Total Records to Delete: ' + CONVERT(NVARCHAR, @DeleteCount)
	RAISERROR (@Log, 10, 1) WITH NOWAIT
	SET @Log = 'Batch Size: 100'
	RAISERROR (@Log, 10, 1) WITH NOWAIT
	SET @Log = 'Max Loop Count: ' + CONVERT(NVARCHAR, @MaxLoopCount)
	RAISERROR (@Log, 10, 1) WITH NOWAIT
END

WHILE @LoopCtr < @MaxLoopCount AND (SELECT COUNT(*) FROM [MessageArchive].[dbo].[Message] WITH(NOLOCK) WHERE ExpiryDate < @CurrentDate) > 0
BEGIN
	DELETE FROM @MessageIds
	
	INSERT INTO @MessageIds (MessageId)
	SELECT TOP 100 MessageId FROM [MessageArchive].[dbo].[Message] WITH(NOLOCK) WHERE ExpiryDate < @CurrentDate
	
	SET @LoopCtr = @LoopCtr + 1
	DELETE FROM [MessageArchive].[dbo].[Message] WITH (ROWLOCK) WHERE MessageId IN (SELECT MessageId FROM @MessageIds)
	
	IF (@Debug = 1)
	BEGIN
		SET @Log = 'Loop Count: ' + CONVERT(NVARCHAR, @LoopCtr) +' - ' + CONVERT(NVARCHAR, GETUTCDATE(), 120)
		RAISERROR (@Log, 10, 1) WITH NOWAIT
	END
	CONTINUE
END

