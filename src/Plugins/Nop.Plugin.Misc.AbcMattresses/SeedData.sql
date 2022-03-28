-- Clears all ABC Mattress data from the DB and seeds test data, useful for local development.
-- Once done, make sure to run the 'Update Mattresses' NOP task.

delete from UrlRecord where slug in (select Description from AbcMattressModel)
delete from Product where Sku COLLATE SQL_Latin1_General_CP1_CI_AS in (select Name from AbcMattressModel)
delete from AbcMattressBase
delete from AbcMattressEntry
delete from AbcMattressGift
delete from AbcMattressModel
delete from AbcMattressModelGiftMapping
delete from AbcMattressPackage
delete from AbcMattressProtector
delete from AbcMattressFrame

SET IDENTITY_INSERT [dbo].[AbcMattressModel] ON 
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5255, N'Alverson', 834, N'Alverson', N'Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5261, N'Carrollton', 834, N'Carrollton', N'Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5267, N'Dewitt', 834, N'Dewitt', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5272, N'TrustII-Hybrid', 215, N'TrustII-Hybrid', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5277, N'KelburnII', 215, N'KelburnII', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5281, N'Malloy', 834, N'Malloy', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5287, N'CONTENTO-FIRM', 215, N'Contento-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5293, N'CopperII-Plush', 215, N'CopperII-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5297, N'CopperII-Firm', 215, N'CopperII-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5301, N'Q8-Number-Bed', 654, N'Q8-Number-Bed', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5304, N'Harmony-Lux-XFM', 558, N'Harmony-Lux-Extra-Firm', N'Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5310, N'Uncommon-Firm', 215, N'Uncommon-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5312, N'BILLINGS', 215, N'Billings', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5314, N'Mirabai', 215, N'Mirabai', N'Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5316, N'CONTENTO-PLUSH', 215, N'Contento-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5322, N'Tofte', 652, N'Tofte', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5328, N'Harbor', 652, N'Harbor', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5334, N'Baldwin', 652, N'Baldwin', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5340, N'Rockwell-Firm', 815, N'Rockwell-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5345, N'Hurston', 815, N'Hurston', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5350, N'Rockwell-Plush', 815, N'Rockwell-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5355, N'SLEEP-RITE-II', 4, N'Sleep-Rite-II', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5357, N'DESTINYII', 4, N'DestinyII', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5359, N'Cassatt-Plush', 815, N'Cassatt-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5364, N'X-Class-Hyb-PL', 558, N'X-Class-Hybrid-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5374, N'MACKINACK-PL', 4, N'MACKINACK-Hybrid-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5379, N'MACKINACK-CF', 4, N'MACKINACK-Hybrid-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5384, N'GRAND-HAVEN-CF', 4, N'GRAND-HAVEN-Hybrid-Plush', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5385, N'SILVIRAZ-CF', 4, N'SILVIRAZ-Hybrid-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5387, N'SILVIRAZ-PL', 4, N'SILVIRAZ-Hybrid-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5389, N'BR800-Plush', 558, N'BR800-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5391, N'BRX1000C-Hyb-PL', 558, N'BRX1000C-Hybrid-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5392, N'BRX1000IP-Hyb-M', 558, N'BRX1000IP-Hybrid-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5393, N'BR800-Medium', 558, N'BR800-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5395, N'BR800-CF', 558, N'BR800-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5397, N'SIGNATUREII-PL', 4, N'SIGNATUREII-Hybrid-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5399, N'SIGNATUREII-CF', 4, N'SIGNATUREII-Hybrid-Cushion Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5401, N'ROSE', 789, N'ROSE', N'Ultra Luxury Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5407, N'ST.CLAIR', 789, N'ST.CLAIR', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5409, N'MARIGOLD', 789, N'MARIGOLD', N'Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5411, N'CHESTNUT-STREET', 215, N'CHESTNUT-STREET', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5427, N'Cassatt-Ultra', 821, N'Cassatt-Ultra', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5466, N'Pro-Breeze-Med', 546, N'Pro-Breeze-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5563, N'Pro-Adapt-Soft', 684, N'Pro-Adapt-Soft', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5569, N'Pro-Adapt-Med', 684, N'Pro-Adapt-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5575, N'Pro-Adapt-Firm', 684, N'Pro-Adapt-Firm', N'Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5581, N'Adapt-Medium', 678, N'Adapt-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5587, N'Adapt-Hybrid-Me', 678, N'Adapt-Hybrid-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5593, N'Pro-Adapt-HYB-M', 684, N'Pro-Adapt-Hybrid-Medium', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5609, N'NECTAR', 743, N'NECTAR', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5616, N'Luxe-Adapt-Firm', 678, N'Luxe-Adapt-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5620, N'Luxe-Adapt-PL', 678, N'Luxe-Adapt-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5650, N'Luxe-Breeze-PL', 546, N'Luxe-Breeze-Plush', N'Plush', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5654, N'Luxe-Breeze-CF', 546, N'Luxe-Breeze-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5661, N'Pro-Breeze-HYB', 546, N'Pro-Breeze-Medium-Hybrid', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5721, N'CONFORM', 215, N'CONFORM', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (5825, N'Q8-Flexhead-Num', 654, N'Q8-Flexhead-Number-Bed', N'Cushion-Firm', NULL, NULL)
GO
INSERT [dbo].[AbcMattressModel] ([Id], [Name], [ManufacturerId], [Description], [Comfort], [ProductId], [BrandCategoryId]) VALUES (6158, N'X-Class-Hyb-CF', 558, N'X-Class-Hybrid-Cushion-Firm', N'Cushion-Firm', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[AbcMattressModel] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressEntry] ON 
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (1, 5255, N'Twin', N'37092', CAST(397.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (2, 5255, N'TwinXL', N'37093', CAST(427.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (3, 5255, N'Full', N'37094', CAST(479.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (4, 5255, N'Queen', N'37095', CAST(497.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (5, 5255, N'King', N'37096', CAST(697.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (6, 5255, N'California King', N'37097', CAST(697.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (7, 5261, N'Twin', N'36750', CAST(219.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (8, 5261, N'TwinXL', N'36751', CAST(249.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (9, 5261, N'Full', N'36752', CAST(297.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (10, 5261, N'Queen', N'36753', CAST(327.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (11, 5261, N'King', N'36754', CAST(437.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (12, 5261, N'California King', N'36755', CAST(437.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (13, 5267, N'Twin', N'36844', CAST(175.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (14, 5267, N'TwinXL', N'36845', CAST(199.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (15, 5267, N'Full', N'36846', CAST(267.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (16, 5267, N'Queen', N'36847', CAST(297.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (17, 5267, N'King', N'36848', CAST(397.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (18, 5272, N'Twin', N'36906', CAST(1049.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (19, 5272, N'TwinXL', N'36907', CAST(1074.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (20, 5272, N'Full', N'36908', CAST(1079.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (21, 5272, N'Queen', N'36909', CAST(1099.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (22, 5272, N'King', N'36910', CAST(1599.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (23, 5277, N'TwinXL', N'36913', CAST(1274.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (24, 5277, N'Full', N'36914', CAST(1279.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (25, 5277, N'Queen', N'36915', CAST(1299.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (26, 5277, N'King', N'36916', CAST(1799.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (27, 5281, N'Twin', N'36970', CAST(297.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (28, 5281, N'TwinXL', N'36971', CAST(337.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (29, 5281, N'Full', N'36972', CAST(397.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (30, 5281, N'Queen', N'36973', CAST(437.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (31, 5281, N'King', N'36974', CAST(527.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (32, 5281, N'California King', N'36975', CAST(527.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (33, 5287, N'Twin', N'35019', CAST(257.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (34, 5287, N'TwinXL', N'35020', CAST(297.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (35, 5287, N'Full', N'35021', CAST(377.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (36, 5287, N'Queen', N'35022', CAST(417.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (37, 5287, N'King', N'35023', CAST(597.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (38, 5287, N'California King', N'35024', CAST(597.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (39, 5293, N'TwinXL', N'37201', CAST(1574.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (40, 5293, N'Full', N'37202', CAST(1579.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (41, 5293, N'Queen', N'37203', CAST(1599.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (42, 5293, N'King', N'37204', CAST(2099.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (43, 5297, N'TwinXL', N'37206', CAST(1574.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (44, 5297, N'Full', N'37207', CAST(1579.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (45, 5297, N'Queen', N'37208', CAST(1599.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (46, 5297, N'King', N'37209', CAST(2099.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (47, 5301, N'TwinXL', N'37230', CAST(2297.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (48, 5301, N'Queen', N'37231', CAST(2599.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (49, 5301, N'King', N'37233', CAST(2999.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (50, 5304, N'Twin', N'37252', CAST(999.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (51, 5304, N'TwinXL', N'37253', CAST(1049.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (52, 5304, N'Full', N'37254', CAST(1119.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (53, 5304, N'Queen', N'37255', CAST(1119.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (54, 5304, N'King', N'37256', CAST(1699.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (55, 5304, N'California King', N'37257', CAST(1699.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (56, 5310, N'Queen', N'37263', CAST(549.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (57, 5310, N'King', N'37264', CAST(797.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (58, 5312, N'Queen', N'37269', CAST(647.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (59, 5312, N'King', N'37270', CAST(857.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (60, 5314, N'Queen', N'37275', CAST(697.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (61, 5314, N'King', N'37276', CAST(897.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (62, 5316, N'Twin', N'35300', CAST(427.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (63, 5316, N'TwinXL', N'35301', CAST(487.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (64, 5316, N'Full', N'35302', CAST(537.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (65, 5316, N'Queen', N'35303', CAST(557.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (66, 5316, N'King', N'35304', CAST(847.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (67, 5316, N'California King', N'35305', CAST(847.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (68, 5322, N'Twin', N'37312', CAST(1397.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (69, 5322, N'TwinXL', N'37313', CAST(1497.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (70, 5322, N'Full', N'37314', CAST(1497.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (71, 5322, N'Queen', N'37315', CAST(1597.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (72, 5322, N'King', N'37316', CAST(2277.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (73, 5322, N'California King', N'37317', CAST(2277.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (74, 5328, N'Twin', N'37318', CAST(1737.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (75, 5328, N'TwinXL', N'37319', CAST(1937.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (76, 5328, N'Full', N'37320', CAST(1937.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (77, 5328, N'Queen', N'37321', CAST(1997.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (78, 5328, N'King', N'37322', CAST(2577.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (79, 5328, N'California King', N'37324', CAST(2577.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (80, 5334, N'Twin', N'37325', CAST(2477.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (81, 5334, N'TwinXL', N'37326', CAST(2687.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (82, 5334, N'Full', N'37327', CAST(2687.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (83, 5334, N'Queen', N'37328', CAST(2787.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (84, 5334, N'King', N'37329', CAST(3497.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (85, 5334, N'California King', N'37330', CAST(3497.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (86, 5340, N'TwinXL', N'32335', CAST(1899.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (87, 5340, N'Full', N'32336', CAST(1949.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (88, 5340, N'Queen', N'32337', CAST(1999.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (89, 5340, N'King', N'32338', CAST(2599.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (90, 5340, N'California King', N'32339', CAST(2599.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (91, 5345, N'TwinXL', N'32340', CAST(1399.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (92, 5345, N'Full', N'32341', CAST(1449.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (93, 5345, N'Queen', N'32342', CAST(1499.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (94, 5345, N'King', N'32343', CAST(2099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (95, 5345, N'California King', N'32344', CAST(2099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (96, 5350, N'TwinXL', N'32345', CAST(1699.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (97, 5350, N'Full', N'32346', CAST(1749.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (98, 5350, N'Queen', N'32347', CAST(1799.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (99, 5350, N'King', N'32348', CAST(2399.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (100, 5350, N'California King', N'32349', CAST(2399.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (101, 5355, N'Queen', N'37363', CAST(229.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (102, 5355, N'King', N'37364', CAST(289.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (103, 5357, N'Queen', N'37368', CAST(359.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (104, 5357, N'King', N'37369', CAST(499.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (105, 5359, N'TwinXL', N'32370', CAST(2399.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (106, 5359, N'Full', N'32371', CAST(2449.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (107, 5359, N'Queen', N'32372', CAST(2499.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (108, 5359, N'King', N'32373', CAST(3099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (109, 5359, N'California King', N'32374', CAST(3099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (110, 5364, N'TwinXL', N'35455', CAST(1799.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (111, 5364, N'Full', N'35456', CAST(2079.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (112, 5364, N'Queen', N'35457', CAST(2199.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (113, 5364, N'King', N'35458', CAST(2699.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (114, 5364, N'California King', N'35459', CAST(2699.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (115, 6158, N'TwinXL', N'35474', CAST(1599.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (116, 6158, N'Full', N'35475', CAST(1879.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (117, 6158, N'Queen', N'35476', CAST(1999.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (118, 6158, N'King', N'35477', CAST(2499.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (119, 6158, N'California King', N'35478', CAST(2499.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (120, 5374, N'Twin', N'37646', CAST(599.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (121, 5374, N'Full', N'37647', CAST(797.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (122, 5374, N'Queen', N'37648', CAST(859.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (123, 5374, N'King', N'37649', CAST(997.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (124, 5374, N'California King', N'37650', CAST(997.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (125, 5379, N'Twin', N'37651', CAST(599.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (126, 5379, N'Full', N'37652', CAST(797.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (127, 5379, N'Queen', N'37653', CAST(859.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (128, 5379, N'King', N'37654', CAST(997.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (129, 5379, N'California King', N'37655', CAST(979.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (130, 5384, N'King', N'37687', CAST(1397.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (131, 5385, N'Twin', N'37723', CAST(433.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (132, 5385, N'Queen', N'37726', CAST(697.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (133, 5387, N'Twin', N'37729', CAST(433.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (134, 5387, N'Queen', N'37732', CAST(697.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (135, 5389, N'Queen', N'37743', CAST(697.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (136, 5389, N'King', N'37744', CAST(997.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (137, 5391, N'Queen', N'37753', CAST(999.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (138, 5392, N'Queen', N'37759', CAST(1299.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (139, 5393, N'Queen', N'37765', CAST(477.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (140, 5393, N'King', N'37766', CAST(597.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (141, 5395, N'Queen', N'37771', CAST(537.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (142, 5395, N'King', N'37772', CAST(757.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (143, 5397, N'Queen', N'37809', CAST(555.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (144, 5397, N'King', N'37810', CAST(727.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (145, 5399, N'Queen', N'37815', CAST(555.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (146, 5399, N'King', N'37816', CAST(727.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (147, 5401, N'Twin', N'37930', CAST(797.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (148, 5401, N'TwinXL', N'37931', CAST(847.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (149, 5401, N'Full', N'37932', CAST(847.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (150, 5401, N'Queen', N'37933', CAST(897.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (151, 5401, N'King', N'37934', CAST(1099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (152, 5401, N'California King', N'37935', CAST(1099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (153, 5407, N'Queen', N'37939', CAST(897.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (154, 5407, N'King', N'37940', CAST(1099.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (155, 5409, N'Queen', N'37945', CAST(797.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (156, 5409, N'King', N'37946', CAST(997.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (157, 5411, N'Queen', N'37993', CAST(657.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (158, 5411, N'King', N'37994', CAST(997.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (159, 5427, N'Full', N'36370', CAST(2249.00000 AS Decimal(19, 5)), N'Inner Spring')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (160, 5272, N'California King', N'36448', CAST(120.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (161, 5466, N'King', N'37705', CAST(4699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (162, 5563, N'Twin', N'37533', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (163, 5563, N'TwinXL', N'37534', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (164, 5563, N'Full', N'37535', CAST(2849.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (165, 5563, N'Queen', N'37536', CAST(2999.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (166, 5563, N'King', N'37537', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (167, 5563, N'California King', N'37538', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (168, 5569, N'Twin', N'37539', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (169, 5569, N'TwinXL', N'37540', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (170, 5569, N'Full', N'37541', CAST(2849.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (171, 5569, N'Queen', N'37542', CAST(2999.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (172, 5569, N'King', N'37543', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (173, 5569, N'California King', N'37544', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (174, 5575, N'Twin', N'37545', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (175, 5575, N'TwinXL', N'37546', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (176, 5575, N'Full', N'37547', CAST(2849.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (177, 5575, N'Queen', N'37548', CAST(2999.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (178, 5575, N'King', N'37549', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (179, 5575, N'California King', N'37551', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (180, 5581, N'Twin', N'37600', CAST(1699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (181, 5581, N'TwinXL', N'37601', CAST(1699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (182, 5581, N'Full', N'37602', CAST(2049.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (183, 5581, N'Queen', N'37603', CAST(2199.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (184, 5581, N'King', N'37604', CAST(2899.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (185, 5581, N'California King', N'37605', CAST(2899.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (186, 5587, N'Twin', N'37606', CAST(1699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (187, 5587, N'TwinXL', N'37607', CAST(1699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (188, 5587, N'Full', N'37608', CAST(2049.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (189, 5587, N'Queen', N'37609', CAST(2199.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (190, 5587, N'King', N'37610', CAST(2899.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (191, 5587, N'California King', N'37611', CAST(2899.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (192, 5593, N'Twin', N'37612', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (193, 5593, N'TwinXL', N'37613', CAST(2499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (194, 5593, N'Full', N'37614', CAST(2849.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (195, 5593, N'Queen', N'37615', CAST(2999.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (196, 5593, N'King', N'37616', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (197, 5593, N'California King', N'37617', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (198, 5609, N'Twin', N'37660', CAST(399.00000 AS Decimal(19, 5)), N'Memory Foam')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (199, 5609, N'TwinXL', N'37661', CAST(469.00000 AS Decimal(19, 5)), N'Memory Foam')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (200, 5609, N'Full', N'37662', CAST(599.00000 AS Decimal(19, 5)), N'Memory Foam')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (201, 5609, N'Queen', N'37663', CAST(699.00000 AS Decimal(19, 5)), N'Memory Foam')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (202, 5609, N'King', N'37664', CAST(899.00000 AS Decimal(19, 5)), N'Memory Foam')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (203, 5609, N'California King', N'37665', CAST(899.00000 AS Decimal(19, 5)), N'Memory Foam')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (204, 5616, N'TwinXL', N'37700', CAST(3199.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (205, 5616, N'Queen', N'37701', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (206, 5616, N'King', N'37702', CAST(4399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (207, 5616, N'California King', N'37703', CAST(4399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (208, 5620, N'TwinXL', N'37711', CAST(3199.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (209, 5620, N'Queen', N'37712', CAST(3699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (210, 5620, N'King', N'37713', CAST(4399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (211, 5620, N'California King', N'37714', CAST(4399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (212, 5650, N'TwinXL', N'37970', CAST(4199.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (213, 5650, N'Queen', N'37971', CAST(4699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (214, 5650, N'King', N'37972', CAST(5399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (215, 5650, N'California King', N'37973', CAST(5399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (216, 5654, N'TwinXL', N'37975', CAST(4199.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (217, 5654, N'Queen', N'37976', CAST(4699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (218, 5654, N'King', N'37977', CAST(5399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (219, 5654, N'California King', N'37978', CAST(5399.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (220, 5466, N'TwinXL', N'37980', CAST(3499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (221, 5466, N'Queen', N'37981', CAST(3999.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (222, 5466, N'California King', N'37982', CAST(4699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (223, 5661, N'TwinXL', N'37983', CAST(3499.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (224, 5661, N'Queen', N'37984', CAST(3999.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (225, 5661, N'King', N'37985', CAST(4699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (226, 5661, N'California King', N'37986', CAST(4699.00000 AS Decimal(19, 5)), N'Tempurpedic Material')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (227, 5293, N'California King', N'37205', CAST(2099.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (228, 5297, N'California King', N'37210', CAST(2099.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (229, 5272, N'California King', N'36912', CAST(1599.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (230, 5277, N'California King', N'36917', CAST(1799.00000 AS Decimal(19, 5)), N'Hybrid')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (231, 5825, N'Queen', N'37232', CAST(2999.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (232, 5825, N'King', N'37234', CAST(3499.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (233, 5301, N'California King', N'37235', CAST(3099.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (234, 5825, N'California King', N'37236', CAST(3499.00000 AS Decimal(19, 5)), N'Air')
GO
INSERT [dbo].[AbcMattressEntry] ([Id], [AbcMattressModelId], [Size], [ItemNo], [Price], [Type]) VALUES (235, 5301, N'Queen', N'37354', CAST(1498.00000 AS Decimal(19, 5)), N'Air')
GO
SET IDENTITY_INSERT [dbo].[AbcMattressEntry] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressGift] ON 
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7913, N'80538', N'FREE-BOSE-SLEEPBUDS', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7914, N'80570', N'FREE-43"-4K-SMART-TV', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7915, N'80535', N'FREE-55"-4K-SMART-TV', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7916, N'80536', N'FREE (X2)PILLOWS & MATT PROT.', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7917, N'80533', N'FREE-HB/FB/RAILS', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7918, N'80543', N'CHARBROIL GRILL $1297/BEDDING', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7919, N'80539', N'$50-NECTAR-GIFT-W/PURCHASE', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7920, N'80567', N'FREE 32" $1299TO $1699 BEDDING', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressGift] ([Id], [ItemNo], [Description], [Amount], [Qty]) VALUES (7921, N'80568', N'FREE 40" $1799+ UP BEDDING', CAST(0.00000 AS Decimal(19, 5)), 1)
GO
SET IDENTITY_INSERT [dbo].[AbcMattressGift] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressModelGiftMapping] ON 
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7778, 5272, 7913)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7779, 5277, 7913)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7780, 5293, 7913)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7781, 5297, 7913)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7782, 5301, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7783, 5322, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7784, 5328, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7785, 5334, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7786, 5340, 7914)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7787, 5345, 7914)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7788, 5350, 7914)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7789, 5359, 7914)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7790, 5364, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7791, 6158, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7792, 5391, 7913)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7793, 5392, 7913)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7794, 5427, 7914)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7795, 5466, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7796, 5304, 7917)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7797, 5563, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7798, 5569, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7799, 5575, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7800, 5581, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7801, 5587, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7802, 5593, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7803, 5609, 7919)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7804, 5616, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7805, 5620, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7806, 5650, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7807, 5654, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7808, 5661, 7915)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7809, 5721, 7917)
GO
INSERT [dbo].[AbcMattressModelGiftMapping] ([Id], [AbcMattressModelId], [AbcMattressGiftId]) VALUES (7810, 5825, 7915)
GO
SET IDENTITY_INSERT [dbo].[AbcMattressModelGiftMapping] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressBase] ON 
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6256, N'37337', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6257, N'37338', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6258, N'37339', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6259, N'37341', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6260, N'37343', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6261, N'36657', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6262, N'36658', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6263, N'36659', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6264, N'36665', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6265, N'37795', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6266, N'37796', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6267, N'37797', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6268, N'37798', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6269, N'37799', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6270, N'35854', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6271, N'35855', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6272, N'35856', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6273, N'35857', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6274, N'35859', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6275, N'37306', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6276, N'37307', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6277, N'37308', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6278, N'37309', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6279, N'', N'', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6280, N'32330', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6281, N'32331', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6282, N'32332', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6283, N'32334', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6284, N'37382', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6285, N'37380', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6286, N'35521', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6287, N'35522', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6288, N'35523', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6289, N'35525', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6290, N'37640', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6291, N'37641', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6292, N'37642', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6293, N'37643', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6294, N'37644', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6295, N'37890', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6296, N'37891', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6297, N'37892', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6298, N'37893', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6299, N'37894', N'Low Profile', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6300, N'35564', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6301, N'35567', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6302, N'38583', N'Ergo 2', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6303, N'38587', N'Ergo 2', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6304, N'37618', N'Ergo Extended', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6305, N'37620', N'Ergo Extended', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6306, N'37331', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6307, N'37332', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6308, N'37333', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6309, N'37334', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6310, N'37336', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6311, N'36633', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6312, N'36634', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6313, N'36652', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6314, N'36655', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6315, N'37553', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6316, N'37790', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6317, N'37791', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6318, N'37792', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6319, N'37793', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6320, N'37794', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6321, N'35848', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6322, N'35849', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6323, N'35850', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6324, N'35851', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6325, N'35853', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6326, N'37300', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6327, N'37301', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6328, N'37302', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6329, N'37303', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6330, N'37304', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6331, N'32325', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6332, N'32326', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6333, N'32327', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6334, N'32329', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6335, N'37378', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6336, N'37376', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6337, N'35513', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6338, N'35482', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6339, N'35515', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6340, N'35517', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6341, N'35514', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6342, N'37552', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6343, N'37554', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6344, N'37555', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6345, N'37557', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6346, N'37635', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6347, N'37636', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6348, N'37637', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6349, N'37634', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6350, N'37639', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6351, N'37870', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6352, N'37871', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6353, N'37872', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6354, N'37873', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6355, N'37874', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6356, N'32328', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6357, N'35563', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6358, N'35565', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6359, N'35566', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6360, N'35568', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6361, N'35569', N'Ease 3.0', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6362, N'35860', N'Simplicity', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6363, N'38584', N'Ergo 2', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6364, N'38585', N'Ergo 2', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6365, N'35588', N'Ergo 2', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6366, N'35589', N'Ergo 2', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6367, N'37528', N'Simplicity', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6368, N'37530', N'Simplicity', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6369, N'35742', N'Simple Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6370, N'37531', N'Simplicity', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6371, N'35746', N'Simple Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6372, N'35861', N'Simplicity', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6373, N'35858', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6374, N'37529', N'Simplicity', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6375, N'37619', N'Ergo Extended', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6376, N'37622', N'Ergo Extended', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6377, N'37624', N'Ergo Extended', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6378, N'35862', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6379, N'35863', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6380, N'38863', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6381, N'58890', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6382, N'58892', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6383, N'58893', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6384, N'35353', N'Simple Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6385, N'35351', N'Simple Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6386, N'35481', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6387, N'35483', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6388, N'35484', N'Simple Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6389, N'35485', N'Advanced Adj Base', 1)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6390, N'37335', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6391, N'35852', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6392, N'36447', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6393, N'37800', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6394, N'37305', N'Regular', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6395, N'37383', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6396, N'35516', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6397, N'37689', N'Split', 0)
GO
INSERT [dbo].[AbcMattressBase] ([Id], [ItemNo], [Name], [IsAdjustable]) VALUES (6398, N'37875', N'Split', 0)
GO
SET IDENTITY_INSERT [dbo].[AbcMattressBase] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressPackage] ON 
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6520, 1, 6256, N'66092', CAST(547.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6521, 2, 6257, N'66093', CAST(577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6522, 3, 6258, N'66094', CAST(657.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6523, 4, 6259, N'66095', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6524, 5, 6257, N'66096', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6525, 6, 6260, N'66097', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6526, 7, 6256, N'66750', CAST(349.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6527, 8, 6257, N'66751', CAST(379.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6528, 9, 6258, N'66752', CAST(459.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6529, 10, 6259, N'66753', CAST(499.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6530, 11, 6257, N'66754', CAST(699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6531, 12, 6260, N'66755', CAST(699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6532, 13, 6256, N'66844', CAST(249.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6533, 14, 6257, N'66845', CAST(279.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6534, 15, 6258, N'66846', CAST(359.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6535, 16, 6259, N'66847', CAST(399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6536, 17, 6257, N'66848', CAST(599.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6537, 18, 6261, N'66906', CAST(1149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6539, 20, 6263, N'66908', CAST(1229.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6540, 21, 6264, N'66909', CAST(1249.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6541, 22, 6262, N'66910', CAST(1849.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6542, 23, 6262, N'66913', CAST(1399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6543, 24, 6263, N'66914', CAST(1429.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6544, 25, 6264, N'66915', CAST(1449.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6545, 26, 6262, N'66916', CAST(2049.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6546, 27, 6256, N'66970', CAST(449.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6547, 28, 6257, N'66971', CAST(479.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6548, 29, 6258, N'66972', CAST(559.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6550, 31, 6257, N'66974', CAST(799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6551, 32, 6260, N'66975', CAST(799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6552, 33, 6265, N'67019', CAST(355.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6554, 35, 6267, N'67021', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6555, 36, 6268, N'67022', CAST(555.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6556, 37, 6266, N'67023', CAST(833.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6557, 38, 6269, N'67024', CAST(833.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6559, 40, 6263, N'67202', CAST(1729.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6560, 41, 6264, N'67203', CAST(1749.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6561, 42, 6262, N'67204', CAST(2349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6563, 44, 6263, N'67207', CAST(1729.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6564, 45, 6264, N'67208', CAST(1749.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6565, 46, 6262, N'67209', CAST(2349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6567, 48, 6264, N'67231', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6568, 49, 6262, N'67233', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6569, 50, 6270, N'67252', CAST(1119.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6570, 51, 6271, N'67253', CAST(1179.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6571, 52, 6272, N'67254', CAST(1259.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6572, 53, 6273, N'67255', CAST(1349.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6573, 54, 6271, N'67256', CAST(1959.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6574, 55, 6274, N'67257', CAST(1959.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6576, 57, 6266, N'67264', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6577, 58, 6268, N'67269', CAST(747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6578, 59, 6266, N'67270', CAST(1077.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6579, 60, 6268, N'67275', CAST(877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6580, 61, 6266, N'67276', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6581, 62, 6265, N'67300', CAST(488.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6582, 63, 6266, N'67301', CAST(525.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6583, 64, 6267, N'67302', CAST(597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6584, 65, 6268, N'67303', CAST(633.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6585, 66, 6266, N'67304', CAST(933.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6586, 67, 6269, N'67305', CAST(933.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6587, 68, 6275, N'67312', CAST(1497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6588, 69, 6276, N'67313', CAST(1777.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6589, 70, 6277, N'67314', CAST(1888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6590, 71, 6278, N'67315', CAST(1997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6591, 72, 6276, N'67316', CAST(2777.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6592, 73, 6279, N'67317', CAST(2777.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6593, 74, 6275, N'67318', CAST(1888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6594, 75, 6276, N'67319', CAST(2148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6595, 76, 6277, N'67320', CAST(2288.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6596, 77, 6278, N'67321', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6597, 78, 6276, N'67322', CAST(3177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6598, 79, 6279, N'67324', CAST(3177.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6599, 80, 6275, N'67325', CAST(2688.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6600, 81, 6276, N'67326', CAST(2797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6601, 82, 6277, N'67327', CAST(2888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6602, 83, 6278, N'67328', CAST(2988.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6603, 84, 6276, N'67329', CAST(3888.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6604, 85, 6279, N'67330', CAST(3888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6605, 86, 6280, N'67335', CAST(2049.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6606, 87, 6281, N'67336', CAST(2149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6608, 89, 6280, N'67338', CAST(2899.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6609, 90, 6283, N'67339', CAST(2899.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6610, 91, 6280, N'67340', CAST(1549.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6611, 92, 6281, N'67341', CAST(1649.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6613, 94, 6280, N'67343', CAST(2399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6614, 95, 6283, N'67344', CAST(2399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6615, 96, 6280, N'67345', CAST(1849.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6616, 97, 6281, N'67346', CAST(1949.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6618, 99, 6280, N'67348', CAST(2699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6619, 100, 6283, N'67349', CAST(2699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6620, 101, 6284, N'67363', CAST(277.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6621, 102, 6285, N'67364', CAST(399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6622, 103, 6284, N'67368', CAST(333.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6623, 104, 6285, N'67369', CAST(559.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6624, 105, 6280, N'67370', CAST(2549.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6625, 106, 6281, N'67371', CAST(2649.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6626, 107, 6282, N'67372', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6627, 108, 6280, N'67373', CAST(3399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6628, 109, 6283, N'67374', CAST(3399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6629, 110, 6286, N'67455', CAST(2099.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6630, 111, 6287, N'67456', CAST(2409.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6631, 112, 6288, N'67457', CAST(2599.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6632, 113, 6286, N'67458', CAST(3199.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6633, 114, 6289, N'67459', CAST(3199.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6634, 115, 6286, N'67474', CAST(1899.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6635, 116, 6287, N'67475', CAST(2209.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6637, 118, 6286, N'67477', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6638, 119, 6289, N'67478', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6639, 120, 6290, N'67646', CAST(537.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6640, 121, 6291, N'67647', CAST(747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6641, 122, 6292, N'67648', CAST(877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6642, 123, 6293, N'67649', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6643, 124, 6294, N'67650', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6644, 125, 6290, N'67651', CAST(537.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6645, 126, 6291, N'67652', CAST(747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6646, 127, 6292, N'67653', CAST(877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6647, 128, 6293, N'67654', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6648, 129, 6294, N'67655', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6649, 130, 6293, N'67687', CAST(1597.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6650, 131, 6290, N'67723', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6651, 132, 6292, N'67726', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6652, 133, 6290, N'67729', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6653, 134, 6292, N'67732', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6654, 135, 6273, N'67743', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6655, 136, 6271, N'67744', CAST(1199.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6658, 139, 6273, N'67765', CAST(555.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6659, 140, 6271, N'67766', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6660, 141, 6273, N'67771', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6661, 142, 6271, N'67772', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6662, 143, 6292, N'67809', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6663, 144, 6293, N'67810', CAST(988.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6664, 145, 6292, N'67815', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6665, 146, 6293, N'67816', CAST(988.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6667, 148, 6296, N'67931', CAST(897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6668, 149, 6297, N'67932', CAST(967.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6670, 151, 6296, N'67934', CAST(1397.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6671, 152, 6299, N'67935', CAST(1397.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6672, 153, 6298, N'67939', CAST(897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6673, 154, 6296, N'67940', CAST(1297.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6674, 155, 6298, N'67945', CAST(779.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6675, 156, 6296, N'67946', CAST(1197.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6676, 157, 6264, N'67993', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6677, 158, 6262, N'67994', CAST(1197.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6678, 89, 6300, N'71338', CAST(4097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6679, 90, 6301, N'71339', CAST(4097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6680, 94, 6300, N'71343', CAST(3597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6681, 95, 6301, N'71344', CAST(3597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6682, 99, 6300, N'71348', CAST(3897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6683, 100, 6301, N'71349', CAST(3897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6684, 108, 6300, N'71373', CAST(4597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6685, 109, 6301, N'71374', CAST(4597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6686, 89, 6302, N'72338', CAST(4997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6687, 90, 6303, N'72339', CAST(4997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6688, 94, 6302, N'72343', CAST(4497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6689, 95, 6303, N'72344', CAST(4497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6690, 99, 6302, N'72348', CAST(4797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6691, 100, 6303, N'72349', CAST(4797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6692, 159, 6281, N'72370', CAST(2649.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6693, 108, 6302, N'72373', CAST(5497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6694, 109, 6303, N'72374', CAST(5497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6695, 89, 6304, N'73338', CAST(6597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6696, 90, 6305, N'73339', CAST(6597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6697, 94, 6304, N'73343', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6698, 95, 6305, N'73344', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6699, 99, 6304, N'73348', CAST(6397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6700, 100, 6305, N'73349', CAST(6397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6701, 108, 6304, N'73373', CAST(7097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6702, 109, 6305, N'73374', CAST(7097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6703, 7, 6306, N'76750', CAST(349.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6704, 8, 6307, N'76751', CAST(379.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6705, 9, 6308, N'76752', CAST(459.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6706, 10, 6309, N'76753', CAST(499.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6707, 11, 6307, N'76754', CAST(699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6708, 12, 6310, N'76755', CAST(699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6709, 13, 6306, N'76844', CAST(249.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6710, 14, 6307, N'76845', CAST(279.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6711, 15, 6308, N'76846', CAST(359.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6712, 16, 6309, N'76847', CAST(399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6713, 17, 6307, N'76848', CAST(599.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6714, 18, 6311, N'76906', CAST(1149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6716, 20, 6313, N'76908', CAST(1229.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6717, 21, 6314, N'76909', CAST(1249.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6718, 22, 6312, N'76910', CAST(1849.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6719, 160, 6279, N'76912', CAST(1849.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6720, 23, 6312, N'76913', CAST(1399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6721, 24, 6313, N'76914', CAST(1429.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6722, 25, 6314, N'76915', CAST(1449.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6723, 26, 6312, N'76916', CAST(2049.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6724, 160, 6279, N'76917', CAST(2049.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6725, 27, 6306, N'76970', CAST(449.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6726, 28, 6307, N'76971', CAST(479.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6727, 29, 6308, N'76972', CAST(559.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6729, 31, 6307, N'76974', CAST(799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6730, 32, 6310, N'76975', CAST(799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6731, 161, 6315, N'77005', CAST(5099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6732, 33, 6316, N'77019', CAST(355.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6734, 35, 6318, N'77021', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6735, 36, 6319, N'77022', CAST(555.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6736, 37, 6317, N'77023', CAST(833.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6737, 38, 6320, N'77024', CAST(833.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6738, 1, 6306, N'77092', CAST(547.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6739, 2, 6307, N'77093', CAST(577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6740, 3, 6308, N'77094', CAST(657.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6741, 4, 6309, N'77095', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6742, 5, 6307, N'77096', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6743, 6, 6310, N'77097', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6745, 40, 6313, N'77202', CAST(1729.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6746, 41, 6314, N'77203', CAST(1749.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6747, 42, 6312, N'77204', CAST(2349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6748, 160, 6279, N'77205', CAST(2349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6750, 44, 6313, N'77207', CAST(1729.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6751, 45, 6314, N'77208', CAST(1749.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6752, 46, 6312, N'77209', CAST(2349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6753, 160, 6279, N'77210', CAST(2349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6755, 48, 6314, N'77231', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6756, 49, 6312, N'77233', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6757, 160, 6279, N'77235', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6758, 52, 6321, N'77252', CAST(1119.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6759, 51, 6322, N'77253', CAST(1179.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6760, 52, 6323, N'77254', CAST(1259.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6761, 53, 6324, N'77255', CAST(1349.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6762, 54, 6322, N'77256', CAST(1959.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6763, 55, 6325, N'77257', CAST(1959.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6765, 57, 6317, N'77264', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6766, 58, 6319, N'77269', CAST(747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6767, 59, 6317, N'77270', CAST(1077.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6768, 60, 6319, N'77275', CAST(877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6769, 61, 6317, N'77276', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6770, 62, 6316, N'77300', CAST(488.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6771, 63, 6317, N'77301', CAST(525.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6772, 64, 6318, N'77302', CAST(597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6773, 65, 6319, N'77303', CAST(633.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6774, 66, 6317, N'77304', CAST(933.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6775, 67, 6320, N'77305', CAST(933.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6776, 68, 6326, N'77312', CAST(1497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6777, 69, 6327, N'77313', CAST(1777.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6778, 70, 6328, N'77314', CAST(1888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6779, 71, 6329, N'77315', CAST(1997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6780, 72, 6327, N'77316', CAST(2777.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6781, 73, 6330, N'77317', CAST(2777.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6782, 74, 6326, N'77318', CAST(1888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6783, 75, 6327, N'77319', CAST(2148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6784, 76, 6328, N'77320', CAST(2288.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6785, 77, 6329, N'77321', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6786, 78, 6327, N'77322', CAST(3177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6787, 79, 6330, N'77324', CAST(3177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6788, 80, 6326, N'77325', CAST(2688.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6789, 81, 6327, N'77326', CAST(2797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6790, 82, 6328, N'77327', CAST(2888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6791, 83, 6329, N'77328', CAST(2988.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6792, 84, 6327, N'77329', CAST(3888.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6793, 85, 6330, N'77330', CAST(3888.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6794, 86, 6331, N'77335', CAST(2049.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6795, 87, 6332, N'77336', CAST(2149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6797, 89, 6331, N'77338', CAST(2899.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6798, 90, 6334, N'77339', CAST(2899.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6799, 91, 6331, N'77340', CAST(1549.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6800, 92, 6332, N'77341', CAST(1649.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6802, 94, 6331, N'77343', CAST(2399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6803, 95, 6334, N'77344', CAST(2399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6804, 96, 6331, N'77345', CAST(1849.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6805, 97, 6332, N'77346', CAST(1949.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6807, 99, 6331, N'77348', CAST(2699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6808, 100, 6334, N'77349', CAST(2699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6809, 101, 6335, N'77363', CAST(277.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6810, 102, 6336, N'77364', CAST(399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6811, 103, 6335, N'77368', CAST(333.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6812, 104, 6336, N'77369', CAST(559.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6813, 105, 6331, N'77370', CAST(2549.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6814, 106, 6332, N'77371', CAST(2649.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6815, 107, 6333, N'77372', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6816, 108, 6331, N'77373', CAST(3399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6817, 109, 6334, N'77374', CAST(3399.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6818, 110, 6337, N'77455', CAST(2099.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6819, 111, 6338, N'77456', CAST(2409.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6820, 112, 6339, N'77457', CAST(2599.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6821, 113, 6337, N'77458', CAST(3199.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6822, 114, 6340, N'77459', CAST(3199.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6823, 115, 6337, N'77474', CAST(1899.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6824, 116, 6341, N'77475', CAST(2209.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6826, 118, 6337, N'77477', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6827, 119, 6340, N'77478', CAST(2999.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6828, 162, 6342, N'77533', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6829, 163, 6315, N'77534', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6830, 164, 6343, N'77535', CAST(3149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6831, 165, 6344, N'77536', CAST(3299.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6832, 166, 6315, N'77537', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6833, 167, 6345, N'77538', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6834, 168, 6342, N'77539', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6835, 169, 6315, N'77540', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6836, 170, 6343, N'77541', CAST(3149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6837, 171, 6344, N'77542', CAST(3299.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6838, 172, 6315, N'77543', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6839, 173, 6345, N'77544', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6840, 174, 6342, N'77545', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6841, 175, 6315, N'77546', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6842, 176, 6343, N'77547', CAST(3149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6843, 177, 6344, N'77548', CAST(3299.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6844, 178, 6315, N'77549', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6845, 179, 6345, N'77551', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6846, 180, 6342, N'77600', CAST(1899.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6847, 181, 6315, N'77601', CAST(1899.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6848, 182, 6343, N'77602', CAST(2349.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6849, 183, 6344, N'77603', CAST(2499.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6850, 184, 6315, N'77604', CAST(3299.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6851, 185, 6345, N'77605', CAST(3299.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6852, 186, 6342, N'77606', CAST(1899.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6853, 187, 6315, N'77607', CAST(1899.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6854, 188, 6343, N'77608', CAST(2349.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6855, 189, 6344, N'77609', CAST(2499.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6856, 190, 6315, N'77610', CAST(3299.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6857, 191, 6345, N'77611', CAST(3299.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6858, 192, 6342, N'77612', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6859, 193, 6315, N'77613', CAST(2699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6860, 194, 6343, N'77614', CAST(3149.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6861, 195, 6344, N'77615', CAST(3299.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6862, 196, 6315, N'77616', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6863, 197, 6345, N'77617', CAST(4099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6864, 120, 6346, N'77646', CAST(537.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6865, 121, 6347, N'77647', CAST(747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6866, 122, 6348, N'77648', CAST(877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6867, 123, 6349, N'77649', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6868, 124, 6350, N'77650', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6869, 125, 6346, N'77651', CAST(537.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6870, 126, 6347, N'77652', CAST(747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6871, 127, 6348, N'77653', CAST(877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6872, 128, 6349, N'77654', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6873, 129, 6350, N'77655', CAST(1177.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6874, 198, 6279, N'77660', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6875, 199, 6279, N'77661', CAST(667.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6876, 200, 6279, N'77662', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6877, 201, 6279, N'77663', CAST(934.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6878, 202, 6279, N'77664', CAST(1077.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6879, 203, 6279, N'77665', CAST(1077.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6880, 130, 6349, N'77687', CAST(1597.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6881, 204, 6315, N'77700', CAST(3399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6882, 205, 6344, N'77701', CAST(3999.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6883, 206, 6315, N'77702', CAST(4799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6884, 207, 6345, N'77703', CAST(4799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6885, 208, 6315, N'77711', CAST(3399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6886, 209, 6344, N'77712', CAST(3999.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6887, 210, 6315, N'77713', CAST(4799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6888, 211, 6345, N'77714', CAST(4799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6889, 131, 6346, N'77723', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6890, 132, 6348, N'77726', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6891, 133, 6346, N'77729', CAST(497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6892, 134, 6348, N'77732', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6893, 135, 6324, N'77743', CAST(797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6894, 136, 6322, N'77744', CAST(1199.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6896, 138, 6324, N'77759', CAST(1399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6897, 139, 6324, N'77765', CAST(555.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6898, 140, 6322, N'77766', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6899, 141, 6324, N'77771', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6900, 142, 6322, N'77772', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6901, 143, 6348, N'77809', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6902, 144, 6349, N'77810', CAST(988.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6903, 145, 6348, N'77815', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6904, 146, 6349, N'77816', CAST(988.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6906, 148, 6352, N'77931', CAST(897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6907, 149, 6353, N'77932', CAST(967.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6909, 151, 6352, N'77934', CAST(1397.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6910, 152, 6355, N'77935', CAST(1397.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6911, 153, 6354, N'77939', CAST(897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6912, 154, 6352, N'77940', CAST(1297.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6913, 155, 6354, N'77945', CAST(779.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6914, 156, 6352, N'77946', CAST(1197.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6915, 212, 6315, N'77970', CAST(4399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6916, 213, 6344, N'77971', CAST(4999.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6917, 214, 6315, N'77972', CAST(5799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6918, 215, 6345, N'77973', CAST(5799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6919, 216, 6315, N'77975', CAST(4399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6920, 217, 6344, N'77976', CAST(4999.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6921, 218, 6315, N'77977', CAST(5799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6922, 219, 6345, N'77978', CAST(5799.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6923, 220, 6315, N'77980', CAST(3699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6924, 221, 6344, N'77981', CAST(4299.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6925, 222, 6345, N'77982', CAST(5099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6926, 223, 6315, N'77983', CAST(3699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6927, 224, 6344, N'77984', CAST(4299.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6928, 225, 6315, N'77985', CAST(5099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6929, 226, 6345, N'77986', CAST(5099.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6930, 157, 6314, N'77993', CAST(697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6931, 158, 6312, N'77994', CAST(1197.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6932, 160, 6279, N'77995', CAST(1197.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6933, 161, 6300, N'78005', CAST(6197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6934, 88, 6356, N'78337', CAST(2269.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6935, 93, 6356, N'78342', CAST(1769.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6936, 98, 6356, N'78347', CAST(2069.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6937, 107, 6356, N'78372', CAST(2769.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6938, 162, 6357, N'78533', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6939, 163, 6300, N'78534', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6940, 164, 6358, N'78535', CAST(3648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6941, 165, 6359, N'78536', CAST(3798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6942, 166, 6300, N'78537', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6943, 167, 6301, N'78538', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6944, 168, 6357, N'78539', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6945, 169, 6300, N'78540', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6946, 170, 6358, N'78541', CAST(3648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6947, 171, 6359, N'78542', CAST(3798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6948, 172, 6300, N'78543', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6949, 173, 6301, N'78544', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6950, 174, 6357, N'78545', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6951, 175, 6300, N'78546', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6952, 176, 6358, N'78547', CAST(3648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6953, 177, 6359, N'78548', CAST(3798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6954, 178, 6300, N'78549', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6955, 179, 6301, N'78551', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6956, 180, 6357, N'78600', CAST(2398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6957, 181, 6300, N'78601', CAST(2398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6958, 182, 6358, N'78602', CAST(2848.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6959, 183, 6359, N'78603', CAST(2998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6960, 184, 6300, N'78604', CAST(4297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6961, 185, 6301, N'78605', CAST(4297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6962, 186, 6357, N'78606', CAST(2398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6963, 187, 6300, N'78607', CAST(2398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6964, 188, 6358, N'78608', CAST(2848.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6965, 189, 6359, N'78609', CAST(2998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6966, 190, 6300, N'78610', CAST(4297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6967, 191, 6301, N'78611', CAST(4297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6968, 192, 6357, N'78612', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6969, 193, 6300, N'78613', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6970, 194, 6358, N'78614', CAST(3648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6971, 195, 6359, N'78615', CAST(3798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6972, 196, 6300, N'78616', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6973, 197, 6301, N'78617', CAST(5097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6974, 204, 6300, N'78700', CAST(3948.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6975, 205, 6359, N'78701', CAST(4548.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6976, 206, 6300, N'78702', CAST(5897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6977, 207, 6301, N'78703', CAST(5897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6978, 206, 6360, N'78704', CAST(5748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6979, 207, 6361, N'78705', CAST(5748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6980, 208, 6300, N'78711', CAST(3948.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6981, 209, 6359, N'78712', CAST(4548.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6982, 210, 6300, N'78713', CAST(5897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6983, 211, 6301, N'78714', CAST(5897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6984, 210, 6360, N'78715', CAST(5748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6985, 211, 6361, N'78716', CAST(5748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6986, 160, 6279, N'78932', CAST(1197.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6987, 212, 6300, N'78970', CAST(4948.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6988, 213, 6359, N'78971', CAST(5548.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6989, 214, 6300, N'78972', CAST(6897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6990, 215, 6301, N'78973', CAST(6897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6991, 216, 6300, N'78975', CAST(4948.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6992, 217, 6359, N'78976', CAST(5548.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6993, 218, 6300, N'78977', CAST(6897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6994, 219, 6301, N'78978', CAST(6897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6995, 220, 6300, N'78980', CAST(4248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6996, 221, 6359, N'78981', CAST(4848.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6997, 222, 6301, N'78982', CAST(6197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6998, 223, 6300, N'78983', CAST(4248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (6999, 224, 6359, N'78984', CAST(4848.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7000, 225, 6300, N'78985', CAST(6197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7001, 226, 6301, N'78986', CAST(6197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7002, 161, 6360, N'79005', CAST(6048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7003, 86, 6300, N'79335', CAST(2648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7004, 87, 6358, N'79336', CAST(2798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7005, 88, 6359, N'79337', CAST(2848.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7006, 89, 6360, N'79338', CAST(3948.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7007, 90, 6361, N'79339', CAST(3948.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7008, 91, 6300, N'79340', CAST(2148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7009, 92, 6358, N'79341', CAST(2298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7010, 93, 6359, N'79342', CAST(2348.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7011, 94, 6360, N'79343', CAST(3448.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7012, 95, 6361, N'79344', CAST(3448.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7013, 96, 6300, N'79345', CAST(2448.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7014, 97, 6358, N'79346', CAST(2598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7015, 98, 6359, N'79347', CAST(2648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7016, 99, 6360, N'79348', CAST(3748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7017, 100, 6361, N'79349', CAST(3748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7018, 105, 6300, N'79370', CAST(3148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7019, 106, 6358, N'79371', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7020, 107, 6359, N'79372', CAST(3348.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7021, 108, 6360, N'79373', CAST(4448.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7022, 109, 6361, N'79374', CAST(4448.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7023, 166, 6360, N'79537', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7024, 167, 6361, N'79538', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7025, 172, 6360, N'79543', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7026, 173, 6361, N'79544', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7027, 178, 6360, N'79549', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7028, 179, 6361, N'79551', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7029, 184, 6360, N'79604', CAST(4248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7030, 185, 6361, N'79605', CAST(4248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7031, 190, 6360, N'79610', CAST(4248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7032, 191, 6361, N'79611', CAST(4248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7033, 196, 6360, N'79616', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7034, 197, 6361, N'79617', CAST(5048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7035, 214, 6360, N'79972', CAST(6748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7036, 215, 6361, N'79973', CAST(6748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7037, 218, 6360, N'79977', CAST(6748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7038, 219, 6361, N'79978', CAST(6748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7039, 222, 6361, N'79982', CAST(6048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7040, 225, 6360, N'79985', CAST(6048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7041, 226, 6361, N'79986', CAST(6048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7042, 39, 6300, N'80201', CAST(2277.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7043, 40, 6358, N'80202', CAST(2287.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7044, 41, 6359, N'80203', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7045, 42, 6300, N'80204', CAST(3497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7046, 227, 6301, N'80205', CAST(3497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7047, 43, 6300, N'80206', CAST(2277.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7048, 44, 6358, N'80207', CAST(2287.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7049, 45, 6359, N'80208', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7050, 46, 6300, N'80209', CAST(3497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7051, 228, 6301, N'80210', CAST(3497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7052, 54, 6362, N'80256', CAST(2597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7053, 58, 6359, N'80269', CAST(1477.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7054, 59, 6360, N'80270', CAST(2077.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7055, 60, 6359, N'80275', CAST(1577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7056, 61, 6360, N'80276', CAST(2177.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7057, 86, 6302, N'80335', CAST(3098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7058, 87, 6363, N'80336', CAST(3248.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7059, 88, 6364, N'80337', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7060, 89, 6365, N'80338', CAST(4598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7061, 90, 6366, N'80339', CAST(4598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7062, 91, 6302, N'80340', CAST(2598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7063, 92, 6363, N'80341', CAST(2748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7064, 93, 6364, N'80342', CAST(2798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7065, 94, 6365, N'80343', CAST(4098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7066, 95, 6366, N'80344', CAST(4098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7067, 96, 6302, N'80345', CAST(2898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7068, 97, 6363, N'80346', CAST(3048.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7069, 98, 6364, N'80347', CAST(3098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7070, 99, 6365, N'80348', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7071, 100, 6366, N'80349', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7072, 105, 6302, N'80370', CAST(3598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7073, 106, 6363, N'80371', CAST(3748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7074, 107, 6364, N'80372', CAST(3798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7075, 108, 6365, N'80373', CAST(5098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7076, 109, 6366, N'80374', CAST(5098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7077, 19, 6300, N'80907', CAST(1777.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7078, 20, 6358, N'80908', CAST(1797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7079, 21, 6359, N'80909', CAST(1897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7080, 22, 6300, N'80910', CAST(2997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7081, 229, 6301, N'80912', CAST(2997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7082, 23, 6300, N'80913', CAST(1977.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7083, 24, 6358, N'80914', CAST(1987.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7084, 25, 6359, N'80915', CAST(2097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7085, 26, 6300, N'80916', CAST(3197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7086, 230, 6301, N'80917', CAST(3197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7087, 161, 6365, N'81005', CAST(6698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7088, 47, 6367, N'81230', CAST(2877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7089, 48, 6368, N'81231', CAST(3197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7090, 231, 6369, N'81232', CAST(3988.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7091, 49, 6370, N'81233', CAST(3688.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7092, 232, 6367, N'81234', CAST(4297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7093, 233, 6361, N'81235', CAST(3688.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7094, 234, 6371, N'81236', CAST(4297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7095, 51, 6362, N'81253', CAST(1498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7096, 53, 6372, N'81255', CAST(1698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7097, 54, 6373, N'81256', CAST(2298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7098, 62, 6357, N'81300', CAST(1097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7099, 63, 6300, N'81301', CAST(1147.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7100, 64, 6358, N'81302', CAST(1250.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7101, 65, 6359, N'81303', CAST(1350.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7102, 66, 6360, N'81304', CAST(1997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7103, 67, 6361, N'81305', CAST(1997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7104, 68, 6357, N'81312', CAST(1997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7105, 69, 6367, N'81313', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7106, 70, 6374, N'81314', CAST(2497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7107, 71, 6368, N'81315', CAST(2788.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7108, 72, 6367, N'81316', CAST(3677.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7109, 73, 6371, N'81317', CAST(3677.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7110, 74, 6357, N'81318', CAST(2437.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7111, 75, 6367, N'81319', CAST(2777.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7112, 76, 6374, N'81320', CAST(2837.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7113, 77, 6368, N'81321', CAST(2797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7114, 78, 6367, N'81322', CAST(3977.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7115, 79, 6371, N'81324', CAST(3977.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7116, 80, 6357, N'81325', CAST(2977.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7117, 81, 6367, N'81326', CAST(3286.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7118, 82, 6374, N'81327', CAST(3380.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7119, 83, 6368, N'81328', CAST(3587.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7120, 84, 6367, N'81329', CAST(4797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7121, 85, 6371, N'81330', CAST(4797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7122, 86, 6304, N'81335', CAST(3898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7123, 88, 6375, N'81337', CAST(4198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7124, 89, 6376, N'81338', CAST(5898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7125, 90, 6377, N'81339', CAST(5898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7126, 91, 6304, N'81340', CAST(3398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7127, 93, 6375, N'81342', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7128, 94, 6376, N'81343', CAST(5398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7129, 95, 6377, N'81344', CAST(5398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7130, 96, 6304, N'81345', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7131, 98, 6375, N'81347', CAST(3998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7132, 99, 6376, N'81348', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7133, 100, 6377, N'81349', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7134, 235, 6368, N'81354', CAST(2497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7135, 105, 6304, N'81370', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7136, 107, 6375, N'81372', CAST(4698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7137, 108, 6376, N'81373', CAST(6398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7138, 109, 6377, N'81374', CAST(6398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7139, 110, 6362, N'81455', CAST(2398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7140, 112, 6372, N'81457', CAST(2898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7141, 113, 6373, N'81458', CAST(3498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7142, 113, 6362, N'81459', CAST(3797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7143, 115, 6362, N'81474', CAST(2198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7144, 117, 6372, N'81476', CAST(2698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7145, 118, 6373, N'81477', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7146, 118, 6362, N'81478', CAST(3597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7147, 199, 6362, N'81661', CAST(947.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7148, 201, 6372, N'81663', CAST(1197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7149, 202, 6373, N'81664', CAST(1657.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7150, 135, 6372, N'81743', CAST(1257.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7151, 136, 6373, N'81744', CAST(1787.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7152, 137, 6372, N'81753', CAST(1597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7153, 138, 6372, N'81759', CAST(1877.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7154, 139, 6372, N'81765', CAST(1037.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7155, 140, 6373, N'81766', CAST(1337.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7156, 141, 6372, N'81771', CAST(1097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7157, 142, 6373, N'81772', CAST(1527.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7158, 18, 6357, N'81906', CAST(1747.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7159, 222, 6366, N'81982', CAST(6698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7160, 157, 6359, N'81993', CAST(1427.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7161, 158, 6360, N'81994', CAST(2477.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7162, 51, 6378, N'82253', CAST(1798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7163, 53, 6379, N'82255', CAST(1998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7164, 54, 6380, N'82256', CAST(2798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7165, 69, 6381, N'82313', CAST(3197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7166, 71, 6382, N'82315', CAST(3497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7167, 72, 6381, N'82316', CAST(5577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7168, 73, 6383, N'82317', CAST(5577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7169, 75, 6381, N'82319', CAST(3537.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7170, 77, 6382, N'82321', CAST(3597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7171, 78, 6381, N'82322', CAST(5888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7172, 79, 6383, N'82324', CAST(5888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7173, 81, 6381, N'82326', CAST(4166.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7174, 83, 6382, N'82328', CAST(4488.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7175, 84, 6381, N'82329', CAST(6577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7176, 85, 6383, N'82330', CAST(6577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7177, 110, 6378, N'82455', CAST(2698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7178, 112, 6379, N'82457', CAST(3198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7179, 113, 6380, N'82458', CAST(3998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7180, 113, 6378, N'82459', CAST(4397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7181, 115, 6378, N'82474', CAST(2498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7182, 117, 6379, N'82476', CAST(2998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7183, 118, 6380, N'82477', CAST(3798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7184, 118, 6378, N'82478', CAST(4197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7185, 166, 6365, N'82537', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7186, 167, 6366, N'82538', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7187, 172, 6365, N'82543', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7188, 173, 6366, N'82544', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7189, 178, 6365, N'82549', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7190, 179, 6366, N'82551', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7191, 184, 6365, N'82604', CAST(4898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7192, 185, 6366, N'82605', CAST(4898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7193, 190, 6365, N'82610', CAST(4898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7194, 191, 6366, N'82611', CAST(4898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7195, 196, 6365, N'82616', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7196, 197, 6366, N'82617', CAST(5698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7197, 135, 6379, N'82743', CAST(1577.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7198, 136, 6378, N'82744', CAST(2697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7199, 137, 6379, N'82753', CAST(1897.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7200, 138, 6379, N'82759', CAST(2197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7201, 139, 6379, N'82765', CAST(1377.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7202, 140, 6378, N'82766', CAST(2267.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7203, 141, 6379, N'82771', CAST(1427.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7204, 142, 6378, N'82772', CAST(2427.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7205, 148, 6362, N'82931', CAST(1355.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7206, 150, 6372, N'82933', CAST(1477.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7207, 151, 6373, N'82934', CAST(2197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7208, 152, 6371, N'82935', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7209, 153, 6372, N'82939', CAST(1388.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7210, 154, 6373, N'82940', CAST(1998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7211, 155, 6372, N'82945', CAST(1397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7212, 156, 6373, N'82946', CAST(1797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7213, 214, 6365, N'82972', CAST(7398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7214, 215, 6366, N'82973', CAST(7398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7215, 218, 6365, N'82977', CAST(7398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7216, 219, 6366, N'82978', CAST(7398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7217, 225, 6365, N'82985', CAST(6698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7218, 226, 6366, N'82986', CAST(6698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7219, 157, 6384, N'82993', CAST(1677.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7220, 158, 6385, N'82994', CAST(2857.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7221, 161, 6302, N'83005', CAST(7097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7222, 39, 6302, N'83201', CAST(2627.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7223, 40, 6363, N'83202', CAST(2697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7224, 41, 6364, N'83203', CAST(2737.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7225, 42, 6302, N'83204', CAST(4287.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7226, 227, 6303, N'83205', CAST(4287.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7227, 43, 6302, N'83206', CAST(2627.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7228, 44, 6363, N'83207', CAST(2697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7229, 45, 6364, N'83208', CAST(2737.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7230, 46, 6302, N'83209', CAST(4287.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7231, 228, 6303, N'83210', CAST(4287.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7232, 51, 6386, N'83253', CAST(2748.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7233, 54, 6378, N'83256', CAST(3197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7234, 110, 6386, N'83455', CAST(3648.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7235, 111, 6338, N'83456', CAST(4028.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7236, 112, 6387, N'83457', CAST(4198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7237, 113, 6388, N'83458', CAST(5298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7238, 113, 6386, N'83459', CAST(6297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7239, 114, 6389, N'83460', CAST(6297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7240, 115, 6386, N'83474', CAST(3448.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7241, 116, 6338, N'83475', CAST(3828.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7242, 117, 6387, N'83476', CAST(3998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7243, 118, 6388, N'83477', CAST(5098.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7244, 118, 6386, N'83478', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7245, 119, 6389, N'83479', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7246, 163, 6302, N'83534', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7247, 164, 6363, N'83535', CAST(4148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7248, 165, 6364, N'83536', CAST(4298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7249, 166, 6302, N'83537', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7250, 167, 6303, N'83538', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7251, 169, 6302, N'83540', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7252, 170, 6363, N'83541', CAST(4148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7253, 171, 6364, N'83542', CAST(4298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7254, 172, 6302, N'83543', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7255, 173, 6303, N'83544', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7256, 175, 6302, N'83546', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7257, 176, 6363, N'83547', CAST(4148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7258, 177, 6364, N'83548', CAST(4298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7259, 178, 6302, N'83549', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7260, 179, 6303, N'83551', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7261, 181, 6302, N'83601', CAST(2898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7262, 182, 6363, N'83602', CAST(3348.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7263, 183, 6364, N'83603', CAST(3498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7264, 184, 6302, N'83604', CAST(5297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7265, 185, 6303, N'83605', CAST(5297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7266, 187, 6302, N'83607', CAST(2898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7267, 188, 6363, N'83608', CAST(3348.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7268, 189, 6364, N'83609', CAST(3498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7269, 190, 6302, N'83610', CAST(5297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7270, 191, 6303, N'83611', CAST(5297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7271, 193, 6302, N'83613', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7272, 194, 6363, N'83614', CAST(4148.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7273, 195, 6364, N'83615', CAST(4298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7274, 196, 6302, N'83616', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7275, 197, 6303, N'83617', CAST(6097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7276, 204, 6302, N'83700', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7277, 205, 6364, N'83701', CAST(4998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7278, 206, 6365, N'83702', CAST(6398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7279, 207, 6366, N'83703', CAST(6398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7280, 206, 6302, N'83704', CAST(6797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7281, 207, 6303, N'83705', CAST(6797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7282, 208, 6302, N'83711', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7283, 209, 6364, N'83712', CAST(4998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7284, 210, 6365, N'83713', CAST(6398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7285, 211, 6366, N'83714', CAST(6398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7286, 210, 6302, N'83715', CAST(6797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7287, 211, 6303, N'83716', CAST(6797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7288, 19, 6302, N'83907', CAST(2270.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7289, 20, 6363, N'83908', CAST(2297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7290, 21, 6364, N'83909', CAST(2397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7291, 22, 6302, N'83910', CAST(3997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7292, 229, 6303, N'83912', CAST(3997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7293, 23, 6302, N'83913', CAST(2472.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7294, 24, 6363, N'83914', CAST(2557.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7295, 25, 6364, N'83915', CAST(2597.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7296, 26, 6302, N'83916', CAST(4197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7297, 230, 6303, N'83917', CAST(4197.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7298, 212, 6302, N'83970', CAST(5398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7299, 213, 6364, N'83971', CAST(5998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7300, 214, 6302, N'83972', CAST(7797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7301, 215, 6303, N'83973', CAST(7797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7302, 216, 6302, N'83975', CAST(5398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7303, 217, 6364, N'83976', CAST(5998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7304, 218, 6302, N'83977', CAST(7797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7305, 219, 6303, N'83978', CAST(7797.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7306, 220, 6302, N'83980', CAST(4698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7307, 221, 6364, N'83981', CAST(5298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7308, 222, 6303, N'83982', CAST(7097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7309, 223, 6302, N'83983', CAST(4698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7310, 224, 6364, N'83984', CAST(5298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7311, 225, 6302, N'83985', CAST(7097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7312, 226, 6303, N'83986', CAST(7097.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7313, 161, 6304, N'84005', CAST(8697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7314, 42, 6360, N'84204', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7315, 227, 6361, N'84205', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7316, 46, 6360, N'84209', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7317, 228, 6361, N'84210', CAST(3298.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7318, 163, 6304, N'84534', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7319, 165, 6375, N'84536', CAST(4998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7320, 166, 6304, N'84537', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7321, 167, 6305, N'84538', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7322, 169, 6304, N'84540', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7323, 171, 6375, N'84542', CAST(4998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7324, 172, 6304, N'84543', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7325, 173, 6305, N'84544', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7326, 175, 6304, N'84546', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7327, 177, 6375, N'84548', CAST(4998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7328, 178, 6304, N'84549', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7329, 179, 6305, N'84551', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7330, 181, 6304, N'84601', CAST(3598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7331, 183, 6375, N'84603', CAST(4198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7332, 184, 6304, N'84604', CAST(6697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7333, 185, 6305, N'84605', CAST(6697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7334, 187, 6304, N'84607', CAST(3598.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7335, 189, 6375, N'84609', CAST(4198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7336, 190, 6304, N'84610', CAST(6697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7337, 191, 6305, N'84611', CAST(6697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7338, 193, 6304, N'84613', CAST(4398.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7339, 195, 6375, N'84615', CAST(4998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7340, 196, 6304, N'84616', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7341, 197, 6305, N'84617', CAST(7497.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7342, 204, 6304, N'84700', CAST(5198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7343, 205, 6375, N'84701', CAST(5898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7344, 206, 6376, N'84702', CAST(7698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7345, 207, 6377, N'84703', CAST(7698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7346, 206, 6304, N'84704', CAST(8397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7347, 207, 6305, N'84705', CAST(8397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7348, 208, 6304, N'84711', CAST(5198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7349, 209, 6375, N'84712', CAST(5898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7350, 210, 6376, N'84713', CAST(7698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7351, 211, 6377, N'84714', CAST(7698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7352, 210, 6304, N'84715', CAST(8397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7353, 211, 6305, N'84716', CAST(8397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7354, 22, 6360, N'84910', CAST(2798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7355, 229, 6361, N'84912', CAST(2798.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7356, 26, 6360, N'84916', CAST(2998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7357, 230, 6361, N'84917', CAST(2998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7358, 212, 6304, N'84970', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7359, 213, 6375, N'84971', CAST(6898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7360, 214, 6304, N'84972', CAST(9397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7361, 215, 6305, N'84973', CAST(9397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7362, 216, 6304, N'84975', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7363, 217, 6375, N'84976', CAST(6898.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7364, 218, 6304, N'84977', CAST(9397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7365, 219, 6305, N'84978', CAST(9397.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7366, 220, 6304, N'84980', CAST(5498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7367, 221, 6375, N'84981', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7368, 222, 6305, N'84982', CAST(8697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7369, 223, 6304, N'84983', CAST(5498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7370, 224, 6375, N'84984', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7371, 225, 6304, N'84985', CAST(8697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7372, 226, 6305, N'84986', CAST(8697.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7373, 42, 6365, N'85204', CAST(3782.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7374, 227, 6366, N'85205', CAST(3782.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7375, 46, 6365, N'85209', CAST(3782.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7376, 228, 6366, N'85210', CAST(3782.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7377, 56, 6359, N'85263', CAST(1297.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7378, 57, 6360, N'85264', CAST(1888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7379, 22, 6365, N'85910', CAST(3498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7380, 229, 6366, N'85912', CAST(3498.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7381, 26, 6365, N'85916', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7382, 230, 6366, N'85917', CAST(3698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7383, 161, 6376, N'86005', CAST(7998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7384, 4, 6390, N'86095', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7385, 166, 6376, N'86537', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7386, 167, 6377, N'86538', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7387, 172, 6376, N'86543', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7388, 173, 6377, N'86544', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7389, 178, 6376, N'86549', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7390, 179, 6377, N'86551', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7391, 184, 6376, N'86604', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7392, 185, 6377, N'86605', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7393, 190, 6376, N'86610', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7394, 191, 6377, N'86611', CAST(6198.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7395, 196, 6376, N'86616', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7396, 197, 6377, N'86617', CAST(6998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7397, 137, 6391, N'86753', CAST(1299.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7398, 141, 6391, N'86771', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7399, 16, 6390, N'86847', CAST(499.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7400, 21, 6392, N'86909', CAST(1319.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7401, 25, 6392, N'86915', CAST(1519.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7402, 214, 6376, N'86972', CAST(8698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7403, 215, 6377, N'86973', CAST(8698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7404, 218, 6376, N'86977', CAST(8698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7405, 219, 6377, N'86978', CAST(8698.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7406, 222, 6377, N'86982', CAST(7998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7407, 225, 6376, N'86985', CAST(7998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7408, 226, 6377, N'86986', CAST(7998.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7409, 36, 6393, N'87022', CAST(655.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7410, 41, 6392, N'87203', CAST(1819.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7411, 45, 6392, N'87208', CAST(1819.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7412, 53, 6391, N'87255', CAST(1349.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7413, 56, 6393, N'87263', CAST(777.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7414, 58, 6393, N'87269', CAST(847.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7415, 60, 6393, N'87275', CAST(977.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7416, 65, 6393, N'87303', CAST(733.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7417, 71, 6394, N'87315', CAST(2097.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7418, 77, 6394, N'87321', CAST(2497.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7419, 83, 6394, N'87328', CAST(3088.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7420, 101, 6395, N'87363', CAST(377.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7421, 103, 6395, N'87368', CAST(433.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7422, 112, 6396, N'87457', CAST(2699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7423, 117, 6396, N'87476', CAST(2499.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7424, 122, 6397, N'87648', CAST(897.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7425, 135, 6391, N'87743', CAST(897.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7426, 10, 6390, N'87753', CAST(599.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7427, 138, 6391, N'87759', CAST(1599.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7428, 139, 6391, N'87765', CAST(655.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7429, 143, 6397, N'87809', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7430, 145, 6397, N'87815', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7431, 150, 6398, N'87933', CAST(1097.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7432, 153, 6398, N'87939', CAST(997.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7433, 155, 6398, N'87945', CAST(879.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7434, 30, 6390, N'87973', CAST(699.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7435, 157, 6392, N'87993', CAST(797.00000 AS Decimal(19, 5)), 2)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7436, 19, 6262, N'66907', CAST(1199.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7437, 30, 6259, N'66973', CAST(599.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7438, 34, 6266, N'67020', CAST(477.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7439, 39, 6262, N'67201', CAST(1699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7440, 43, 6262, N'67206', CAST(1699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7441, 47, 6262, N'67230', CAST(2399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7442, 56, 6268, N'67263', CAST(677.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7443, 88, 6282, N'67337', CAST(2199.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7444, 93, 6282, N'67342', CAST(1699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7445, 98, 6282, N'67347', CAST(1999.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7446, 117, 6288, N'67476', CAST(2399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7447, 137, 6273, N'67753', CAST(1199.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7448, 138, 6273, N'67759', CAST(1499.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7449, 147, 6295, N'67930', CAST(888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7450, 150, 6298, N'67933', CAST(997.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7451, 19, 6312, N'76907', CAST(1199.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7452, 30, 6309, N'76973', CAST(599.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7453, 34, 6317, N'77020', CAST(477.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7454, 39, 6312, N'77201', CAST(1699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7455, 43, 6312, N'77206', CAST(1699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7456, 47, 6312, N'77230', CAST(2399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7457, 56, 6319, N'77263', CAST(677.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7458, 88, 6333, N'77337', CAST(2199.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7459, 93, 6333, N'77342', CAST(1699.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7460, 98, 6333, N'77347', CAST(1999.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7461, 117, 6339, N'77476', CAST(2399.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7462, 137, 6324, N'77753', CAST(1199.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7463, 147, 6351, N'77930', CAST(888.00000 AS Decimal(19, 5)), 1)
GO
INSERT [dbo].[AbcMattressPackage] ([Id], [AbcMattressEntryId], [AbcMattressBaseId], [ItemNo], [Price], [BaseQuantity]) VALUES (7464, 150, 6354, N'77933', CAST(997.00000 AS Decimal(19, 5)), 1)
GO
SET IDENTITY_INSERT [dbo].[AbcMattressPackage] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressFrame] ON 
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (36, N'ESSENTIAL TWIN', N'39064', CAST(29.99000 AS Decimal(19, 5)), N'Twin')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (37, N'PREMIUM TWIN', N'39067', CAST(55.99000 AS Decimal(19, 5)), N'Twin')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (38, N'ESSENTIAL FULL', N'39065', CAST(59.00000 AS Decimal(19, 5)), N'Full')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (39, N'PREMIUM FULL', N'39068', CAST(59.99000 AS Decimal(19, 5)), N'Full')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (40, N'PREMIUM QUEEN', N'39069', CAST(79.99000 AS Decimal(19, 5)), N'Queen')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (41, N'ESSENTIAL KING', N'39066', CAST(69.99000 AS Decimal(19, 5)), N'King')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (42, N'PREMIUM KING', N'39070', CAST(109.99000 AS Decimal(19, 5)), N'King')
GO
INSERT [dbo].[AbcMattressFrame] ([Id], [Name], [ItemNo], [Price], [Size]) VALUES (43, N'PREMIUM CALKING', N'39071', CAST(99.99000 AS Decimal(19, 5)), N'California King')
GO
SET IDENTITY_INSERT [dbo].[AbcMattressFrame] OFF
GO
SET IDENTITY_INSERT [dbo].[AbcMattressProtector] ON 
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (522, N'Twin', N'ULS TWIN PROTECTOR', N'35000', CAST(39.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (523, N'Twin', N'L&P TWIN PAD', N'35292', CAST(28.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (524, N'Twin', N'SEALY TWIN PAD', N'35440', CAST(35.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (525, N'Twin', N'UVM TWIN COOLTOUCH', N'36000', CAST(39.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (526, N'Twin', N'TEMPUR TWIN PAD', N'36020', CAST(35.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (527, N'Twin', N'TEMPUR TWIN PAD', N'37020', CAST(109.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (528, N'Twin', N'L&P TWIN PAD', N'37292', CAST(39.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (529, N'Twin', N'GBC TWIN PROTECTOR', N'39072', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (530, N'TwinXL', N'ULS MATT PROTECTOR', N'35006', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (531, N'TwinXL', N'SEALY TWINXL PAD', N'35441', CAST(29.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (532, N'TwinXL', N'UVM TWINXL COOLTOUCH', N'36001', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (533, N'TwinXL', N'TEMPURPEDIC TWINXL', N'36021', CAST(39.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (534, N'TwinXL', N'TEMPUR TWINXL', N'37021', CAST(109.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (535, N'TwinXL', N'L&P TWINXL PAD', N'37293', CAST(39.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (536, N'TwinXL', N'ULS MATT PROTECTOR', N'38001', CAST(79.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (537, N'TwinXL', N'GBC MATT PROTECTOR', N'39073', CAST(59.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (538, N'Full', N'ULS MATT PROTECTOR', N'35002', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (539, N'Full', N'L&P FULL PAD', N'35287', CAST(25.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (540, N'Full', N'SEALY FULL PAD', N'35444', CAST(39.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (541, N'Full', N'UVM FULL COOLTOUCH', N'36002', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (542, N'Full', N'TEMPURPEDIC FULL PAD', N'36022', CAST(44.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (543, N'Full', N'TEMPUR FULL PAD', N'37022', CAST(129.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (544, N'Full', N'L&P FULL PAD', N'37287', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (545, N'Full', N'GBC MATT PROTECTOR', N'39074', CAST(59.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (546, N'Queen', N'ULS MATT PROTECTOR', N'35003', CAST(59.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (547, N'Queen', N'SEALY QUEEN PAD', N'35445', CAST(69.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (548, N'Queen', N'UVM QUEEN COOLTOUCH', N'36003', CAST(55.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (549, N'Queen', N'TEMPUR QUEEN', N'37023', CAST(149.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (550, N'Queen', N'L&P QUEEN PAD', N'37288', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (551, N'Queen', N'ULS ULTIMATE PROT', N'38003', CAST(99.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (552, N'Queen', N'GBC ENCASED PROT', N'39075', CAST(69.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (553, N'King', N'ULS KING MATT PROT', N'35004', CAST(69.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (554, N'King', N'L&P KING PAD', N'35293', CAST(25.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (555, N'King', N'SEALY KING PAD', N'35446', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (556, N'King', N'UVM KING COOLTOUCH', N'36004', CAST(77.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (557, N'King', N'TEMPURPEDIC KING PAD', N'36026', CAST(99.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (558, N'King', N'TEMPUR KING PAD', N'37025', CAST(169.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (559, N'King', N'L&P KING PAD', N'37295', CAST(59.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (560, N'King', N'ULS ULTIMATE PROT', N'38004', CAST(129.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (561, N'King', N'KING ENCASE MATT PRO', N'39076', CAST(79.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (562, N'California King', N'CALKING MATT PROT', N'35005', CAST(69.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (563, N'California King', N'SEALY CALKING PAD', N'35450', CAST(49.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (564, N'California King', N'L&P CALKING PAD', N'35765', CAST(35.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (565, N'California King', N'UVM CALKING COOLTOUC', N'36005', CAST(77.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (566, N'California King', N'TEMPUR CALKING PAD', N'37026', CAST(169.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (567, N'California King', N'L&P CALKING PAD', N'37296', CAST(59.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (568, N'California King', N'ULTIMATE MAT PROT', N'38005', CAST(129.00000 AS Decimal(19, 5)))
GO
INSERT [dbo].[AbcMattressProtector] ([Id], [Size], [Name], [ItemNo], [Price]) VALUES (569, N'California King', N'CALK ENCASED PROTECT', N'39077', CAST(79.00000 AS Decimal(19, 5)))
GO
SET IDENTITY_INSERT [dbo].[AbcMattressProtector] OFF
GO
