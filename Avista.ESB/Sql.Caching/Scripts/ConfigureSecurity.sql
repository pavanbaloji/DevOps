-- ****************************************************************************
-- Configure Security
-- ****************************************************************************
USE [Caching]
GO

-- ****************************************************************************
-- Set database owner.
-- ****************************************************************************
sp_changedbowner @loginame = 'sa'
GO

-- *************************************************
--   Add access for host instances.
-- *************************************************
-- BizTalkServerHostGroup
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$(BizTalkServerHostGroup)')
BEGIN
  DROP USER [$(BizTalkServerHostGroup)]
END
GO

CREATE USER [$(BizTalkServerHostGroup)] FOR LOGIN [$(BizTalkServerHostGroup)]
GO

EXEC sp_addrolemember @rolename = 'db_datareader', @membername = N'$(BizTalkServerHostGroup)'
GO

EXEC sp_addrolemember @rolename = 'db_datawriter', @membername = N'$(BizTalkServerHostGroup)'
GO

-- BizTalkServerIsolatedHostGroup
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$(BizTalkServerIsolatedHostGroup)')
BEGIN
  DROP USER [$(BizTalkServerIsolatedHostGroup)]
END
GO

CREATE USER [$(BizTalkServerIsolatedHostGroup)] FOR LOGIN [$(BizTalkServerIsolatedHostGroup)]
GO

EXEC sp_addrolemember @rolename = 'db_datareader', @membername = N'$(BizTalkServerIsolatedHostGroup)'
GO

EXEC sp_addrolemember @rolename = 'db_datawriter', @membername = N'$(BizTalkServerIsolatedHostGroup)'
GO

GRANT EXECUTE ON Caching.dbo.sp_PurgeExpiredCache to [$(BizTalkServerIsolatedHostGroup)]
GO

GRANT EXECUTE ON Caching.dbo.sp_PurgeExpiredCache to [$(BizTalkServerHostGroup)]
GO