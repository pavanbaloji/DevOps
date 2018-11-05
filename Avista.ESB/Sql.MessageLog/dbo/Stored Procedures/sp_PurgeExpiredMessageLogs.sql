

-- ****************************************************************************
-- Stored Procedure:  sp_PurgeExpiredMessageLogs
-- ****************************************************************************
CREATE PROCEDURE [dbo].[sp_PurgeExpiredMessageLogs]
AS

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
 
DELETE FROM [MessageLog].[dbo].[EventMessage]
WHERE Logged < DATEADD(d, -45, GetUTCDate())

