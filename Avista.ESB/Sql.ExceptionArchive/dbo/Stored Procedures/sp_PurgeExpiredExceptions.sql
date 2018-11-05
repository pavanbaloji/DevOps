

-- ****************************************************************************
-- Stored Procedure:  sp_PurgeExpiredExceptions
-- ****************************************************************************
CREATE PROCEDURE [dbo].[sp_PurgeExpiredExceptions]
AS

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
 
DELETE FROM [ExceptionArchive].[dbo].[Exception]
WHERE ExpiryDate < GetUTCDate()

