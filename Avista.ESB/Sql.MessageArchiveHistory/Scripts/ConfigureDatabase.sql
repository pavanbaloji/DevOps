declare @fileSize int;

select @fileSize = size from sys.master_files where name = 'MessageArchiveHistory';

if (@fileSize < 524288)
	ALTER DATABASE [MessageArchiveHistory] MODIFY FILE ( NAME = N'MessageArchiveHistory', SIZE = 4096MB , MAXSIZE = UNLIMITED, FILEGROWTH = 256MB )


select @fileSize = size from sys.master_files where name = 'MessageArchiveHistory_log';

if (@fileSize < 131072)
	ALTER DATABASE [MessageArchiveHistory] MODIFY FILE ( NAME = N'MessageArchiveHistory_log', SIZE = 1024MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB)