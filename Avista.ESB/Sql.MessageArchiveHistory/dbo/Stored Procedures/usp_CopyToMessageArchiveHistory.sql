
CREATE PROCEDURE [dbo].[usp_CopyToMessageArchiveHistory]
(
       @BatchSize INT = 100000
)
AS
BEGIN
    SET NOCOUNT ON

	SELECT sj.name, sja.*
	FROM msdb.dbo.sysjobactivity AS sja
	INNER JOIN msdb.dbo.sysjobs AS sj ON sja.job_id = sj.job_id and sj.name= 'MessageArchivePurgeExpiredMessagesFromHistory'
	WHERE sja.start_execution_date IS NOT NULL AND sja.stop_execution_date IS NULL
	
	IF @@rowcount = 0
	BEGIN
	
		DECLARE @RowCount INT = 1;

		-- ****************************************************************************
		-- Merge [ArchiveType] table
		-- ****************************************************************************
		MERGE INTO [dbo].[ArchiveType] AS target
		USING [MessageArchive].[dbo].[ArchiveType] AS source
		ON target.Id = source.Id
		WHEN NOT MATCHED BY TARGET THEN
		INSERT ([Id],[Name],[Active],[DefaultExpiry])
		VALUES (source.[Id],source.[Name],source.[Active],source.[DefaultExpiry]);

		-- ****************************************************************************
		--Merge [Endpoint] table
		-- ****************************************************************************
		MERGE INTO [dbo].[Endpoint] AS target
		USING [MessageArchive].[dbo].[Endpoint] AS source
		ON target.Id = source.Id
		WHEN NOT MATCHED BY TARGET THEN
		INSERT ([Id],[Name])
		VALUES (source.[Id],source.[Name]);

		-- ****************************************************************************
		--Copy rows from [Message] table
		-- ****************************************************************************
		IF OBJECT_ID('tempdb..#tempMessage') IS NOT NULL
		DROP TABLE #tempMessage

		SELECT [MessageId], 0 AS [Flag] 	
		INTO #tempMessage 
		FROM [MessageArchive].[dbo].[Message] src with(nolock)
		WHERE [InsertedDate] < DATEADD(MI,-10, GETUTCDATE())
		AND NOT EXISTS (
		SELECT 1 FROM [dbo].[CopiedMessage] dst with(nolock)
		WHERE dst.[MessageId] = src.[MessageId]
		)


		DECLARE @MessageIds TABLE (MessageId UNIQUEIDENTIFIER PRIMARY KEY)

		WHILE (@RowCount > 0)
		BEGIN

			DELETE FROM @MessageIds

			UPDATE TOP(@BatchSize) #tempMessage
			SET Flag = 1 
			OUTPUT inserted.MessageId
			INTO @MessageIds
			WHERE Flag =0;

			INSERT INTO [dbo].[Message]  ([MessageId],[InterchangeId],[MessageType],[ActivityId],[Tag],[InsertedDate],[ExpiryDate],[ArchiveTypeId],[SourceSystemId],[TargetSystemId],[Description])
			SELECT TOP(@BatchSize) src.[MessageId],src.[InterchangeId],src.[MessageType],src.[ActivityId],src.[Tag],src.[InsertedDate],src.[ExpiryDate],src.[ArchiveTypeId],src.[SourceSystemId],src.[TargetSystemId],src.[Description] 
			FROM [MessageArchive].[dbo].[Message] AS src 
			INNER JOIN @MessageIds AS msgIds ON src.MessageId = msgIds.MessageId

			SET @RowCount = @@ROWCOUNT;
		END

		-- ****************************************************************************
		--Copy rows from [Part] table
		-- ****************************************************************************
		IF OBJECT_ID('tempdb..#tempParts') IS NOT NULL
		DROP TABLE #tempParts

		SELECT prt.[MessageId], prt.[PartId], 0 AS [Flag] INTO #tempParts FROM [MessageArchive].[dbo].[Part] prt WITH (NOLOCK)
		INNER JOIN #tempMessage m ON prt.MessageId = m.MessageId

		SET @RowCount = 1;

		DECLARE @Parts TABLE (MessageId UNIQUEIDENTIFIER, PartId UNIQUEIDENTIFIER PRIMARY KEY)

		WHILE (@RowCount > 0)
		BEGIN

			DELETE FROM @Parts

			UPDATE TOP(@BatchSize) #tempParts
			SET Flag = 1 
			OUTPUT inserted.MessageId, inserted.PartId
			INTO @Parts
			WHERE Flag =0;

			INSERT INTO [dbo].[Part] WITH (TABLOCK) ([MessageId],[PartId],[PartName],[PartIndex],[ContentType],[CharSet],[TextData],[ImageData])
			SELECT TOP(@BatchSize) src.[MessageId], src.[PartId], src.[PartName], src.[PartIndex], src.[ContentType], src.[CharSet], src.[TextData], src.[ImageData] 
			FROM [MessageArchive].[dbo].[Part] AS src 
			INNER JOIN @Parts AS msgPrt ON src.PartId = msgPrt.PartId AND src.MessageId = msgPrt.MessageId

			SET @RowCount = @@ROWCOUNT;
		END

		-- ****************************************************************************
		--Copy rows from [PartProperty] table
		-- ****************************************************************************
		SET @BatchSize = @BatchSize*4

		IF OBJECT_ID('tempdb..#tempPartProperty') IS NOT NULL
		DROP TABLE #tempPartProperty

		SELECT prtProp.[PartId], prtProp.[PropertyIndex], 0 AS [Flag] INTO #tempPartProperty FROM [MessageArchive].[dbo].[PartProperty] prtProp WITH (NOLOCK)
		INNER JOIN #tempParts AS part ON prtProp.PartId=part.PartId

		SET @RowCount = 1;

		DECLARE @PartProperties TABLE (PartId UNIQUEIDENTIFIER, PropertyIndex INT PRIMARY KEY(PartId, PropertyIndex))

		WHILE (@RowCount > 0)
		BEGIN

			DELETE FROM @PartProperties

			UPDATE TOP(@BatchSize) #tempPartProperty
			SET Flag = 1 
			OUTPUT inserted.PartId, inserted.PropertyIndex
			INTO @PartProperties
			WHERE Flag =0;

			INSERT INTO [dbo].[PartProperty] ([PartId],[PropertyIndex],[Namespace],[Name],[Value])
			SELECT TOP(@BatchSize) src.[PartId],src.[PropertyIndex],src.[Namespace],src.[Name],src.[Value] 
			FROM [MessageArchive].[dbo].[PartProperty] AS src 
			INNER JOIN @PartProperties AS prtProp ON src.PropertyIndex = prtProp.PropertyIndex AND src.PartId = prtProp.PartId

			SET @RowCount = @@ROWCOUNT;
		END

		-- ****************************************************************************
		--Copy rows from [MessageProperty] table
		-- ****************************************************************************
		IF OBJECT_ID('tempdb..#tempMessageProperty') IS NOT NULL
		DROP TABLE #tempMessageProperty

		SELECT mp.[MessageId], 0 AS [Flag] INTO #tempMessageProperty FROM [MessageArchive].[dbo].[MessageProperty] mp 
		INNER JOIN #tempMessage m ON mp.MessageId = m.MessageId

		SET @RowCount = 1;

		DECLARE @MessageProperties TABLE (MessageId UNIQUEIDENTIFIER PRIMARY KEY(MessageId))
		WHILE (@RowCount > 0)
		BEGIN

			DELETE FROM @MessageProperties

			UPDATE TOP(@BatchSize) #tempMessageProperty
			SET Flag = 1 
			OUTPUT inserted.MessageId
			INTO @MessageProperties
			WHERE Flag =0;

			INSERT INTO [dbo].[MessageProperty]  ([MessageId],[ContextData])
			SELECT TOP(@BatchSize) src.[MessageId],src.[ContextData] 
			FROM [MessageArchive].[dbo].[MessageProperty] AS src 
			INNER JOIN @MessageProperties AS msgProp ON src.MessageId = msgProp.MessageId

			SET @RowCount = @@ROWCOUNT;
		END

		-- ****************************************************************************
		--Add MessageIds to Copied Messages to ensure we dont try to copy same msg again
		--This record is kept until the expired purge then it will be deleted along with
		--history record.
		-- ****************************************************************************
		INSERT INTO [dbo].[CopiedMessage]([MessageId])
		SELECT [MessageId] FROM #tempMessage

		-- ****************************************************************************
		--Clean up all temp tables
		-- ****************************************************************************
		IF OBJECT_ID('tempdb..#tempMessage') IS NOT NULL
		DROP TABLE #tempMessage

		IF OBJECT_ID('tempdb..#tempParts') IS NOT NULL
		DROP TABLE #tempParts

		IF OBJECT_ID('tempdb..#tempPartProperty') IS NOT NULL
		DROP TABLE #tempPartProperty

		IF OBJECT_ID('tempdb..#tempMessageProperty') IS NOT NULL
		DROP TABLE #tempMessageProperty
    END
    ELSE RETURN
END

GO
