declare @fileSize int;

select @fileSize = size from sys.master_files where name = 'MessageArchive';

if (@fileSize < 524288)
	ALTER DATABASE [MessageArchive] MODIFY FILE ( NAME = N'MessageArchive', SIZE = 4096MB , MAXSIZE = UNLIMITED, FILEGROWTH = 256MB )


select @fileSize = size from sys.master_files where name = 'MessageArchive_log';

if (@fileSize < 131072)
	ALTER DATABASE [MessageArchive] MODIFY FILE ( NAME = N'MessageArchive_log', SIZE = 1024MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB)