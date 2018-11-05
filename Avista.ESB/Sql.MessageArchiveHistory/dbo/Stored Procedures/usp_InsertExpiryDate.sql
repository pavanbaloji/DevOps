
-- ****************************************************************************
-- Stored Procedure:  usp_InsertExpiryDate
-- Modified by: Pavan
-- Modified Date: 
--  
-- 
-- ****************************************************************************

CREATE PROCEDURE [dbo].[usp_InsertExpiryDate]
(
       @BatchSize INT = 100
)
AS
BEGIN
       SET NOCOUNT ON

       DECLARE @DefaultFinalExpiryHours int
       DECLARE @DefaultErrorExpiryHours int
       DECLARE @DefaultUnknownExpiryHours int

       SET @DefaultFinalExpiryHours = (SELECT top 1 FinalExpiryHours  FROM [dbo].[PurgeConfiguration] WHERE [Tag] ='Default')
       SET @DefaultErrorExpiryHours = (SELECT top 1 ErrorExpiryHours  FROM [dbo].[PurgeConfiguration] WHERE [Tag] ='Default')
       SET @DefaultUnknownExpiryHours = (SELECT top 1 UnknownExpiryHours  FROM [dbo].[PurgeConfiguration] WHERE [Tag] ='Default')

       IF OBJECT_ID('tempdb..#tempMessage') IS NOT NULL
       DROP TABLE #tempMessage
       
       SELECT [InterchangeID], [Tag], [ArchiveTypeId], [InsertedDate], [ExpiryDate] INTO #tempMessage 
       FROM [dbo].[Message] WITH (NOLOCK)
       WHERE  [ExpiryDate] IS NULL AND [InsertedDate] < DATEADD(MI,-15, GETUTCDATE())

       --Update all Errored Interchange Ids
       UPDATE msg
       SET msg.[ExpiryDate] = CAST(calc.ExpiryDate AS datetime)
       FROM #tempMessage msg WITH (ROWLOCK)
       INNER JOIN
       (      
              SELECT x.InterchangeId, DATEADD(HOUR, COALESCE(p.ErrorExpiryHours, @DefaultErrorExpiryHours), x.InsertedDate) AS ExpiryDate FROM PurgeConfiguration p 
              RIGHT JOIN
              (
                     SELECT Initial.InterchangeId, Initial.Tag, Initial.[InsertedDate] 
                     FROM
                     (
                           SELECT [InterchangeID], [Tag], [ArchiveTypeId], [InsertedDate] FROM #tempMessage
                           WHERE [ArchiveTypeId] = 2 AND ExpiryDate IS NULL
                     ) Initial
                     JOIN
                     (
                          SELECT DISTINCT [InterchangeID] FROM #tempMessage
                           WHERE [ArchiveTypeId] = 6 AND ExpiryDate IS NULL
                     ) error ON Initial.[InterchangeId] = error.[InterchangeId] AND Initial.[ArchiveTypeId] =2
              ) x
              ON p.Tag = x.Tag
       ) calc
       ON msg.[InterchangeId] = calc.[InterchangeId]

    --Update all Final Interchange Ids
       UPDATE msg
       SET msg.[ExpiryDate] = CAST(calc.ExpiryDate AS datetime)
       FROM #tempMessage msg WITH (ROWLOCK)
       INNER JOIN
       (
              SELECT x.InterchangeId, DATEADD(HOUR, COALESCE(p.FinalExpiryHours, @DefaultFinalExpiryHours), x.InsertedDate) AS ExpiryDate FROM PurgeConfiguration p 
              RIGHT JOIN
              (
                     SELECT Initial.InterchangeId, Initial.Tag, Initial.[InsertedDate] 
                     FROM
                     (
                           SELECT [InterchangeID], [Tag], [ArchiveTypeId], [InsertedDate] FROM #tempMessage
                           WHERE [ArchiveTypeId] = 2 AND ExpiryDate IS NULL
                     ) Initial
                     JOIN
                     (
                          SELECT DISTINCT [InterchangeID] FROM #tempMessage
                           WHERE [ArchiveTypeId] = 4 AND ExpiryDate IS NULL
                     ) final ON Initial.[InterchangeId] = final.[InterchangeId] AND Initial.[ArchiveTypeId] =2
              ) x
              ON p.Tag = x.Tag
       ) calc
       ON msg.[InterchangeId] = calc.[InterchangeId]


       --Update all Unknown Interchange Ids
       UPDATE msg
       SET msg.[ExpiryDate] = CAST(calc.ExpiryDate AS datetime)
       FROM #tempMessage msg WITH (ROWLOCK)
       INNER JOIN
       (
              SELECT x.InterchangeId, DATEADD(HOUR, COALESCE(p.UnknownExpiryHours, @DefaultUnknownExpiryHours), x.InsertedDate) AS ExpiryDate FROM PurgeConfiguration p 
              RIGHT JOIN
              (
                     SELECT [InterchangeID], [Tag], [ArchiveTypeId], [InsertedDate] FROM #tempMessage
                     WHERE ExpiryDate IS NULL
              ) x
              ON p.Tag = x.Tag
       ) calc
       ON msg.[InterchangeId] = calc.[InterchangeId]

       --Update actual Message Table
		DECLARE @DistinctInterchangeIds TABLE (InterchangeId NVARCHAR(36), ExpiryDate DATETIME, Flag INT)
		DECLARE @SmallBatchInterchangeIds TABLE (InterchangeId NVARCHAR(36), ExpiryDate DATETIME)
		DECLARE @RowCount INT

		INSERT INTO @DistinctInterchangeIds (InterchangeId, ExpiryDate, Flag)
		SELECT DISTINCT InterchangeId, ExpiryDate, 0 FROM #tempMessage

		SET @RowCount = 1;

		WHILE (@RowCount > 0)
			BEGIN
			DELETE FROM @SmallBatchInterchangeIds

			UPDATE TOP(@BatchSize) @DistinctInterchangeIds
			SET Flag = 1 
			OUTPUT inserted.InterchangeId, inserted.ExpiryDate
			INTO @SmallBatchInterchangeIds
			WHERE Flag =0;

			SET @RowCount = @@ROWCOUNT;

			UPDATE msg
			SET msg.[ExpiryDate] = CAST(calc.ExpiryDate AS datetime)
			FROM [dbo].[Message] msg WITH (ROWLOCK)
			INNER JOIN
			(
					SELECT InterchangeId, ExpiryDate FROM @SmallBatchInterchangeIds
			) calc
			ON msg.[InterchangeId] = calc.[InterchangeId] 
		END
END


GO


