USE [msdb]
GO
/****** Object:  Job [CopyToMessageArchiveHistory]    Script Date: 5/16/2018 4:18:04 PM ******/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'CopyToMessageArchiveHistory')
Begin
		Exec sp_delete_job @job_name = N'CopyToMessageArchiveHistory'
		Print '--object CopyToMessageArchiveHistory deleted successfully---'
End
Else
	Print '--object CopyToMessageArchiveHistory is not found for delete---'

USE [msdb]
GO
/****** Object:  Job [MessageArchivePurge] ***/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'MessageArchivePurge')
Begin
		Exec sp_delete_job @job_name = N'MessageArchivePurge'
		Print '--object MessageArchivePurge deleted successfully---'
End
Else
	Print '--object MessageArchivePurge is not found for delete---'

USE [msdb]
GO
/****** Object:  Job [MessageArchiveHistoryPurge] ***/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'MessageArchiveHistoryPurge')
Begin
	Exec sp_delete_job @job_name = N'MessageArchiveHistoryPurge'
	Print '--object MessageArchiveHistoryPurge deleted successfully---'
End
Else
	Print '--object MessageArchiveHistoryPurge is not found for delete---'
Go

USE [msdb]
GO
/****** Object:  Job [MessageArchiveHistoryPurge] ***/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'MessageArchiveCopytoHistory')
Begin
	Exec sp_delete_job @job_name = N'MessageArchiveCopytoHistory'
	Print '--object MessageArchiveCopytoHistory deleted successfully---'
End
Else
	Print '--object MessageArchiveCopytoHistory is not found for delete---'
Go

USE [msdb]
GO
/****** Object:  Job [MessageArchiveHistoryPurge] ***/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'MessageArchiveHistoryExpiryPurge')
Begin
	Exec sp_delete_job @job_name = N'MessageArchiveHistoryExpiryPurge'
	Print '--object MessageArchiveHistoryExpiryPurge deleted successfully---'
End
Else
	Print '--object MessageArchiveHistoryExpiryPurge is not found for delete---'
Go

USE [msdb]
GO
/****** Object:  Job [MessageArchiveHistoryPurge] ***/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'MessageArchiveMasterCopytoHistoryandPurge')
Begin
	Exec sp_delete_job @job_name = N'MessageArchiveMasterCopytoHistoryandPurge'
	Print '--object MessageArchiveMasterCopytoHistoryandPurge deleted successfully---'
End
Else
	Print '--object MessageArchiveMasterCopytoHistoryandPurge is not found for delete---'
Go

USE [msdb]
GO
/****** Object:  Job [MessageArchiveHistoryPurge] ***/
IF EXISTS (SELECT * FROM msdb.dbo.sysjobs_view WHERE name = N'MessageArchivePurgeCopiedMessages')
Begin
	Exec sp_delete_job @job_name = N'MessageArchivePurgeCopiedMessages'
	Print '--object MessageArchivePurgeCopiedMessages deleted successfully---'
End
Else
	Print '--object MessageArchivePurgeCopiedMessages is not found for delete---'
Go

USE [MessageArchiveHistory]
GO
/****** Object:  StoredProcedure [dbo].[sp_CopyToMessageArchiveHistory]   ******/
If Object_Id('[MessageArchiveHistory].[dbo].[sp_CopyToMessageArchiveHistory]') is not null
Begin
	DROP PROCEDURE [dbo].[sp_CopyToMessageArchiveHistory]
	Print '--object sp_CopyToMessageArchiveHistory deleted successfully---'
End
Else
	Print '--object sp_CopyToMessageArchiveHistory is not found for delete---'
GO


USE [MessageArchiveHistory]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertExpiryDate]   ******/
If Object_Id('[dbo].[sp_InsertExpiryDate]') is not null
Begin
	DROP PROCEDURE [dbo].[sp_InsertExpiryDate]
	Print '--object sp_InsertExpiryDate deleted successfully---'
End
Else
	Print '--object sp_InsertExpiryDate is not found for delete---'
GO


USE [MessageArchiveHistory]
GO
/****** Object:  StoredProcedure [dbo].[sp_PurgeCopiedMessageArchiveMessages] ******/
If Object_Id('[dbo].[sp_PurgeCopiedMessageArchiveMessages]') is not null
Begin
	DROP PROCEDURE [dbo].[sp_PurgeCopiedMessageArchiveMessages]
	Print '--Object sp_PurgeCopiedMessageArchiveMessages deleted successfully---'
End
Else
	Print '--object sp_PurgeCopiedMessageArchiveMessages is not found for delete---'
GO


USE [MessageArchiveHistory]
GO
/****** Object:  StoredProcedure [dbo].[sp_PurgeExpiredMessages] ******/
If Object_Id('[dbo].[sp_PurgeExpiredMessages]') is not null
Begin
	DROP PROCEDURE [dbo].[sp_PurgeExpiredMessages]
	Print '--object [sp_PurgeExpiredMessages] deleted successfully---'
End
Else
	Print '--object [sp_PurgeExpiredMessages] is not found for delete---'
GO


USE [MessageArchiveHistory]
GO
/****** Object:  StoredProcedure [dbo].[sp_PurgeExpiredMessages] ******/
If Object_Id('[dbo].[usp_PurgeCopiedMessageArchiveMessages]') is not null
Begin
	DROP PROCEDURE [dbo].[usp_PurgeCopiedMessageArchiveMessages]
	Print '--object [usp_PurgeCopiedMessageArchiveMessages] deleted successfully---'
End
Else
	Print '--object [usp_PurgeCopiedMessageArchiveMessages] is not found for delete---'
GO
