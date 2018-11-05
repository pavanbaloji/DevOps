-- ****************************************************************************
-- Stored Procedure:  sp_InsertExpiryDate
-- ****************************************************************************
CREATE PROCEDURE [dbo].[sp_InsertExpiryDate]
(
       @Debug BIT = 0
)
AS
BEGIN
       SET NOCOUNT ON
       SET TRANSACTION ISOLATION LEVEL READ COMMITTED

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
       UPDATE msg
       SET msg.[ExpiryDate] = CAST(calc.ExpiryDate AS datetime)
       FROM [dbo].[Message] msg WITH (ROWLOCK)
       INNER JOIN
       (
              SELECT DISTINCT InterchangeId, ExpiryDate FROM #tempMessage
       ) calc
       ON msg.[InterchangeId] = calc.[InterchangeId]
END