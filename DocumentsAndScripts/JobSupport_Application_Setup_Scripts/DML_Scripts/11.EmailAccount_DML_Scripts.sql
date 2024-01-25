

-- use onjobsupport47

IF NOT EXISTS (SELECT * FROM [EmailAccount] WHERE [Email]='no-reply@Onjobsupport.in')
   BEGIN
	 INSERT INTO [EmailAccount]([DisplayName],[Email],[Host],[Username],[Password],[Port],[MaxNumberOfEmails],[EnableSsl],[UseDefaultCredentials])
     VALUES ('On Job Suport','no-reply@Onjobsupport.in','smtp-relay.brevo.com','umsateesh@gmail.com','TkUZDCRvhxnF8Era','587',5,0,0)
   END
 