-- *************************************************
	--   Add access for development team
	-- *************************************************
	IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$(DeploymentSsoUserGroup)')
	BEGIN
	  DROP USER [$(DeploymentSsoUserGroup)]
	END

	CREATE USER [$(DeploymentSsoUserGroup)] FOR LOGIN [$(DeploymentSsoUserGroup)]

	EXEC sp_addrolemember @rolename = 'db_datareader', @membername = N'$(DeploymentSsoUserGroup)'

	EXEC sp_addrolemember @rolename = 'db_datawriter', @membername = N'$(DeploymentSsoUserGroup)'