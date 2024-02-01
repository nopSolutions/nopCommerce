
-- use onjobsupport47

-------------------   [CustomerAttribute]  -----------------------------

-- SELECT * FROM [CustomerAttribute]

SET IDENTITY_INSERT [dbo].[CustomerAttribute] ON 

INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (1,'ProfileType', 0, 2,0);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (2,'Current Avalibility', 0, 1,4);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (3,'Relavent Experiance', 0, 1,3);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (4,'Mother Tongue', 0, 1,5);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (5,'Short Description', 0, 10,6);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (6,'Full Description', 0, 10,7);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (7,'Primary Technology', 0, 51,1);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (8,'Secondary Technology', 0, 51,2);

SET IDENTITY_INSERT [dbo].[CustomerAttribute] OFF

-- Make all feilds mandatory except secondary technology which is not used presently
UPDATE [CustomerAttribute] SET IsRequired=1 WHERE Id Not In (8)

-- 
UPDATE [CustomerAttribute] SET ShowOnRegisterPage=1

------------------------  [SpecificationAttribute]  -----------------------------------------

-- Delete  FROM  SpecificationAttribute 
 -- SELECT * FROM SpecificationAttribute

SET IDENTITY_INSERT [dbo].[SpecificationAttribute] ON 

INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (1, N'ProfileType', 0)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (2, N'Current Avalibility', 4)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (3, N'Relavent Experiance', 3)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (4, N'Mother Tongue', 5)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (5, N'Short Description', 6)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (6, N'Full Description', 7)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (7, N'Primary Technology', 1)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (8, N'Secondary Technology', 2)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (9, N'Gender', 8)

SET IDENTITY_INSERT [dbo].[SpecificationAttribute] OFF

--------------------   [SpecificationAttributeOption] ------------------------------------------------------------------------

-- SELECT * FROM [SpecificationAttribute] 
-- SELECT * FROM [SpecificationAttributeOption]

-- Delete  * from  [SpecificationAttributeOption]


SET IDENTITY_INSERT [SpecificationAttributeOption] ON

-- Profile Type
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (1,1,'Give Support',0);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (2,1,'Take Support',1);

-- Current Avalibility
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (3,2,'Available',2);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (4,2,'UnAvailable',3);

-- Relavent Experiance
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (5,3,'0 ',4);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (6,3,'1+',5);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (7,3,'2+ ',6);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (8,3,'3+',7);

INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (9,3,'4+',8);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (10,3,'5+',9);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (11,3,'6+',10);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (12,3,'7+',11);

INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (13,3,'8+',12);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (14,3,'9+',13);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (15,3,'10+',14);

-- Mother Tounge
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(16,'English', 4, 8);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(17,'Bengali', 4, 9);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(18,'Gujarati', 4, 10);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(19,'Hindi', 4, 11);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(20,'Kannada', 4, 12);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(21,'Malayalam', 4,13);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(22,'Marathi', 4, 14);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(23,'Oriya', 4, 15);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(24,'Punjabi', 4, 16);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(25,'Tamil', 4, 17);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(26,'Telugu', 4, 18);

-- Gender
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(27,'Male', 9, 19);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(28,'FeMale', 9, 20);

SET IDENTITY_INSERT [SpecificationAttributeOption] OFF

-- Customer Attribute related DDL scripts for updating tooltip values in register and info pages
UPDATE [CustomerAttribute]
SET HelpText='Select Take Support if you want to take support else select Give Support'
WHERE Id=1

UPDATE [CustomerAttribute]
SET HelpText='Select Avialibilty so that people can contact you if you are only available.'
WHERE Id=2

UPDATE [CustomerAttribute]
SET HelpText='Provide Your relavent experiance with respect to technical skill you mentioned.'
WHERE Id=3

UPDATE [CustomerAttribute]
SET HelpText='Provide Your mother tongue so that others can contact you <br/> if they want to contact same language'
WHERE Id=4

UPDATE [CustomerAttribute]
SET HelpText='Provide short description like  "I have 5 years experaince in xyz technologies" OR "I am looking for some one who can provide support on XYZ technologies"'
WHERE Id=5

UPDATE [CustomerAttribute]
SET HelpText='Provide your technical skill set ,exp etc OR what are you looking for. Please do not share your personally identifiable information such as contact nos,email ids, fb ids, linked in ids. This is strictly against our policy.  '
WHERE Id=6

UPDATE [CustomerAttribute]
SET HelpText='Select TOP 5 skills that you are expert in OR TOP 5 skill that are you looking for in a procpective match'
WHERE Id=7

UPDATE [CustomerAttribute]
SET HelpText='Select TOP 5 optional skills that you have OR TOP 5 optional skills that are you looking for in a procpective match'
WHERE Id=8

UPDATE [CustomerAttribute]
SET HelpText='Provide your linkedin url. We will never show this to anyone . This is to verify your experiance by our internal team and once verified you will get higher ranking in user searchs '
WHERE Id=9

---------------------------------------------------------------------------------------------------