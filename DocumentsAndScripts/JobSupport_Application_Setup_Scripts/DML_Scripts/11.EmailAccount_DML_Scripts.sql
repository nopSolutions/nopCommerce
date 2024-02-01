

-- use onjobsupport47

IF NOT EXISTS (SELECT * FROM [EmailAccount] WHERE [Email]='no-reply@Onjobsupport.in')
   BEGIN
	 INSERT INTO [EmailAccount]([DisplayName],[Email],[Host],[Username],[Password],[Port],[MaxNumberOfEmails],[EnableSsl],[UseDefaultCredentials])
     VALUES ('On Job Suport','no-reply@Onjobsupport.in','smtp-relay.brevo.com','umsateesh@gmail.com','TkUZDCRvhxnF8Era','587',5,0,0)
   END
 

 -- select * from [dbo].[MessageTemplate]

 -- update bcc email id so that for every customer email , this email id will be bcced so that
 -- I can check content of different templates and their usage
 UPDATE [dbo].[MessageTemplate]
 SET BccEmailAddresses='umsateesh@gmail.com'

 --replace product with profile in some columns
 UPDATE [dbo].[MessageTemplate]
 SET Subject=REPLACE(Subject,'product','profile'),
    Body=REPLACE(Body,' product ',' profile ')
 WHERE Name in ('Product.ProductReview','ProductReview.Reply.CustomerNotification')