USE [MessageArchiveHistory]
GO

MERGE INTO [PurgeConfiguration] AS Target
USING (VALUES
('Default', 360, 1080, 1080),
('0050BM_ProduceOnlineBillImage',8,168,168),
('0112FN_ValidatePoetAccount',24,168,168),
('0315SAM_RetrieveOutageInfo',24,24,24),
('0316SAM_RetrievePremiseOutageHistory',8,168,168),
('0705PA_SyncPerson',8,168,168),
('BP100_AuthorizePayment',360,720,720),
('BP101_RecordPayment',360,720,720),
('BP103_GetWallet',360,720,720),
('BP104_UpdateWalletItem',360,720,720),
('BP105_RemoveWalletItem',360,720,720),
('BP106_GetScheduledPayments',360,720,720),
('BP107_ManageScheduledPayment',360,720,720),
('BP109_RemoveScheduledPayment',360,720,720),
('BP110_GetAutoPayments',360,720,720),
('BP111_ManageAutoPayment',360,720,720),
('BP113_CancelAutoPayment',360,720,720),
('BP116_GetBillingDetails',360,720,720),
('CM004_ValidatePaymentEligibility',360,720,720),
('CM005_ManageServiceAgreement',360,720,720),
('CM008_QueryAccountPremises',360,720,720),
('CM009_QueryPremiseServiceAgreements',360,720,720),
('CM010_QueryAccountDetails',360,720,720),
('FN002b_PublishFixedAssetChanges',360,720,720),
('SM001_ReceiveInvoiceFromEbs',360,720,720),
('SM006_ReceiveInvoiceFromEcm',360,720,720),
('SoRRetry',8,168,168),
('IM007_BulkMeterImport',360,720,720),
('ReceiveIgnoredMessageFromServiceSuite',24,24,24),
('ReceiveOpenWayMeterDataFromItronMdm',24,24,24)
) 
AS SOURCE (Tag,FinalExpiryHours,ErrorExpiryHours,UnknownExpiryHours)
ON Target.Tag = Source.Tag
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET Tag = Source.Tag, FinalExpiryHours = Source.FinalExpiryHours, ErrorExpiryHours = Source.ErrorExpiryHours, UnknownExpiryHours=Source.UnknownExpiryHours
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Tag,FinalExpiryHours,ErrorExpiryHours,UnknownExpiryHours) 
VALUES (Tag,FinalExpiryHours,ErrorExpiryHours,UnknownExpiryHours) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE

OUTPUT $action, inserted.*, deleted.*
;