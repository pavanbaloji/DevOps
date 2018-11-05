USE [MessageArchiveHistory]
GO

-- Check for column that is to be deleted by new Msg props
--  do some prep if so for success
IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Name'
          AND Object_ID = Object_ID(N'[dbo].[MessageProperty]'))
	BEGIN
		Print 'Message props table state is pre-consolidation of props, truncating table'
		truncate table [dbo].[MessageProperty]
		Print '-->done.'
	END
Else
	Print 'Message props table is fresh.'
GO