declare @fileSize int;

select @fileSize = size from sys.master_files where name = 'MessageLog';

if (@fileSize < 524288)
	ALTER DATABASE MessageLog MODIFY FILE ( NAME = N'MessageLog', SIZE = 4096MB , MAXSIZE = UNLIMITED, FILEGROWTH = 256MB )


select @fileSize = size from sys.master_files where name = 'MessageLog_log';

if (@fileSize < 131072)
	ALTER DATABASE MessageLog MODIFY FILE ( NAME = N'MessageLog_log', SIZE = 1024MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB)