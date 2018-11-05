-- ****************************************************************************
-- Configure Security
-- ****************************************************************************
USE [ExceptionArchive]
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

-- BizTalkServerHostAccount
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$(BizTalkServerHostAccount)')
BEGIN
  DROP USER [$(BizTalkServerHostAccount)]
END
GO

CREATE USER [$(BizTalkServerHostAccount)] FOR LOGIN [$(BizTalkServerHostAccount)]
GO

EXEC sp_addrolemember @rolename = 'db_datareader', @membername = N'$(BizTalkServerHostAccount)'
GO

EXEC sp_addrolemember @rolename = 'db_datawriter', @membername = N'$(BizTalkServerHostAccount)'
GO

-- BizTalkServerIsolatedHostAccount
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$(BizTalkServerIsolatedHostAccount)')
BEGIN
  DROP USER [$(BizTalkServerIsolatedHostAccount)]
END
GO

CREATE USER [$(BizTalkServerIsolatedHostAccount)] FOR LOGIN [$(BizTalkServerIsolatedHostAccount)]
GO

EXEC sp_addrolemember @rolename = 'db_datareader', @membername = N'$(BizTalkServerIsolatedHostAccount)'
GO

EXEC sp_addrolemember @rolename = 'db_datawriter', @membername = N'$(BizTalkServerIsolatedHostAccount)'
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





IF ('$(Environment)'='DEV')
BEGIN
	:r .\ConfigureDevSecurity.sql
END