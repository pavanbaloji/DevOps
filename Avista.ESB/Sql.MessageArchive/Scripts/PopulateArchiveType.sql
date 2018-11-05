USE [MessageArchive]
GO

MERGE INTO [ArchiveType] AS Target
USING (VALUES
(0, 'Unknown', 1, 10080),
(1, 'Filtered', 0, 0),
(2, 'Initial', 1, 21600),
(3, 'Internal', 1, 10080),
(4, 'Final', 1, 21600),
(5, 'Temporary', 1, 1440),
(6, 'Error', 1, 10080)
) 
AS SOURCE (Id, Name, Active, DefaultExpiry)
ON Target.Id = Source.Id
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET Name = Source.Name, Active = Source.Active, DefaultExpiry = Source.DefaultExpiry 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, Name, Active, DefaultExpiry) 
VALUES (Id, Name, Active, DefaultExpiry) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE

OUTPUT $action, inserted.*, deleted.*
;