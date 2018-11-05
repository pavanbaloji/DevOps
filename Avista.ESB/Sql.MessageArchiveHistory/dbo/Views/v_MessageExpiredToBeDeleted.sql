

CREATE VIEW [dbo].[vMessage_ExpiredToBeDeleted]  AS
	SELECT TOP 5000 MessageID, ExpiryDate FROM dbo.Message Order By ExpiryDate;
GO


