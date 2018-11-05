CREATE PROCEDURE sp_PurgeExpiredCache 
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[Cache] WHERE (AbsoluteExpirationTime <= GETDATE())
END
GO