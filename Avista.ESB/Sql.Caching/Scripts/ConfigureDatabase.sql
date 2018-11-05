declare @fileSize int;

select @fileSize = size from sys.master_files where name = 'Caching';

if (@fileSize < 524288)
	ALTER DATABASE Caching MODIFY FILE ( NAME = N'Caching', SIZE = 4096MB , MAXSIZE = UNLIMITED, FILEGROWTH = 256MB )


select @fileSize = size from sys.master_files where name = 'Caching_log';

if (@fileSize < 131072)
	ALTER DATABASE Caching MODIFY FILE ( NAME = N'Caching_log', SIZE = 1024MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB)