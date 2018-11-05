declare @fileSize int;

select @fileSize = size from sys.master_files where name = 'ExceptionArchive';

if (@fileSize < 524288)
	ALTER DATABASE ExceptionArchive MODIFY FILE ( NAME = N'ExceptionArchive', SIZE = 4096MB , MAXSIZE = UNLIMITED, FILEGROWTH = 256MB )


select @fileSize = size from sys.master_files where name = 'ExceptionArchive_log';

if (@fileSize < 131072)
	ALTER DATABASE ExceptionArchive MODIFY FILE ( NAME = N'ExceptionArchive_log', SIZE = 1024MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB)