-- 7Spikes upgrade scripts from nopCommerce 4.10 to 4.20

-- Upgrade script for Anywhere Sliders
IF(EXISTS (SELECT NULL FROM sys.objects WHERE object_id = OBJECT_ID(N'[SS_AS_SliderImage]') AND type in (N'U')))
BEGIN
	IF( NOT EXISTS (SELECT NULL FROM sys.columns WHERE object_id = object_id(N'[SS_AS_SliderImage]') and NAME='MobilePictureId'))
	BEGIN
		ALTER TABLE [SS_AS_SliderImage] ADD [MobilePictureId] [int] NOT NULL DEFAULT 0;
	END
END