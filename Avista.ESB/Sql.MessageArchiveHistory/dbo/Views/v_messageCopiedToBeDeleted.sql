CREATE VIEW [dbo].[vMessage_CopiedToBeDeleted] AS
	SELECT TOP(1000) cm.MessageId 
	FROM [dbo].[Message] m WITH(NOLOCK)
	JOIN [dbo].[CopiedMessage] cm WITH(NOLOCK) ON m.MessageID=cm.MessageID
	Order by cm.messageid
GO
