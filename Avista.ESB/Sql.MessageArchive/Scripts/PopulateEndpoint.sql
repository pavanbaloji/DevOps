USE [MessageArchive]
GO

MERGE INTO [Endpoint] AS Target
USING (VALUES
(0, N'Unknown'),
(1, N'Esb'),
(2, N'Afm'),
(3, N'AUWeb'),
(4, N'Ccb'),
(5, N'Ebs'),
(6, N'Kubra'),
(7, N'Maximo'),
(8, N'Mdm'),
(9, N'Regulus'),
(10, N'Twacs'),
(12, N'Omt'),
(13, N'MembershipDb'),
(14, N'ServiceSuite'),
(15, N'Aclara'),
(16, N'Smtp'),
(17, N'iFactor'),
(18, N'Fiserv'),
(19, N'ItronMdm'),
(20, N'OracleMdm'),
(21, N'Designer'),
(22, N'Nucleus')
) 
AS SOURCE (Id, Name)
ON Target.Id = Source.Id
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET Name = Source.Name 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, Name) 
VALUES (Id, Name) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN
DELETE

OUTPUT $action, inserted.*, deleted.*
;