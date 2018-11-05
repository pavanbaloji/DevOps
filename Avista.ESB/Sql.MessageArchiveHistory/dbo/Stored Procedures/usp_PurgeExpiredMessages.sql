
-- ****************************************************************************
-- Stored Procedure:  usp_PurgeExpiredMessages
-- -- Description: procedure to delete expired records from the messagearchivehistory 
-- Modified by:
-- Modified Date: 
--  --
--
-- ****************************************************************************

CREATE PROCEDURE [dbo].[usp_PurgeExpiredMessages]
(
	@BatchSize INT = 100
)
AS
BEGIN
	SET NOCOUNT ON
 
	DECLARE @RowCount INT = 1
	DECLARE @CurrentDate DATETIME = GETUTCDATE()

	WHILE (@RowCount > 0)
	BEGIN
		BEGIN TRANSACTION;
		DELETE TOP (@BatchSize)  [dbo].[Message] WITH (ROWLOCK) WHERE ExpiryDate < @CurrentDate

		SET @RowCount = @@ROWCOUNT;	
		COMMIT TRANSACTION;
	END
END
GO
