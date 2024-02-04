
-- use [onjobsupport47]

-- Topics Settings
UPDATE [dbo].[Topic] SET [Body] = '<p>Please contact us for any queries you have. We will be happy to assist you.</p>' WHERE Id=4 -- Contact us page
UPDATE [dbo].[Topic] SET [Body] = '<p></p>', Title='' WHERE Id=7 -- Login Page Info
UPDATE LocaleStringResource
SET ResourceValue='<p><strong>Welcome to on job support!</strong><br />Register with us for future convenience:</p><p style="text-align: left;">1.Resgistration is mandatory as we need to show relavent profiles to provide support and take support</p><p style="text-align: left;">2. You can directly contact with people who can provide support thus eliminating middle man</p><p style="text-align: left;">3. Please visit <a title="This Link" href="https://onjobsupport.in" target="_blank" rel="noopener">This Link</a> for further information</p>'
Where ResourceName='Account.Login.NewCustomerText'


-- hide footer Shipping & returns item
UPDATE [dbo].[Topic] SET IncludeInFooterColumn1=0 WHERE SystemName='ShippingInfo'


-- LoginPageContent Topic
INSERT INTO [dbo].[Topic]
           ([SystemName],[IncludeInSitemap],[IncludeInTopMenu],[IncludeInFooterColumn1]
		   ,[IncludeInFooterColumn2],[IncludeInFooterColumn3],[DisplayOrder],[AccessibleWhenStoreClosed]
           ,[IsPasswordProtected],[Title],[Body],[Published],[TopicTemplateId],[SubjectToAcl],[LimitedToStores])
     VALUES('LoginPageContent',0,0,0,0,0,1,0,0,'',
			'<div class="information-boxes-wrapper"> <div class="information-boxes-block"> <div class="information-box" href="abv.bg"> <div class="image-wrapper"> <div class="image-holder"> <img alt="" src="https://venture2.nop-templates.com/images/thumbs/0000442.png"> </div></div><div class="information-wrapper"> <div class="title">Why Choose Us</div></div><div class="information-wrapper"> <div class="description"> Lorem ipsum dolor sit amet, et mel quis habeo patrioque,eu eripuit menandri. Lorem ipsum dolor sit amet, et mel quis habeo patrioque,eu eripuit menandri. </div></div></div><div class="information-box"> <div class="image-wrapper"> <div class="image-holder"> <img alt="" src="https://venture2.nop-templates.com/images/thumbs/0000443.png"> </div></div><div class="information-wrapper"> <div class="title">100% Satisfaction Guaranteed</div></div><div class="information-wrapper"> <div class="description">Lorem ipsum dolor sit amet, et mel quis habeo patrioque.</div></div></div><div class="information-box"> <div class="image-wrapper"> <div class="image-holder"> <img alt="" src="https://venture2.nop-templates.com/images/thumbs/0000444.png"> </div></div><div class="information-wrapper"> <div class="title">100% Satisfaction Guaranteed</div></div><div class="information-wrapper"> <div class="description">Lorem ipsum dolor sit amet, et mel quis habeo patrioque.</div></div></div></div></div>'
			,1,1,0,0)

-- Crete Login page new customer text
IF NOT EXISTS (SELECT * FROM [Topic] WHERE [SystemName]='LoginPageNewCustomerText')
   BEGIN
		INSERT [dbo].[Topic] ([Id], [SystemName], [IncludeInSitemap], [IncludeInTopMenu], [IncludeInFooterColumn1], [IncludeInFooterColumn2], [IncludeInFooterColumn3], [DisplayOrder], [AccessibleWhenStoreClosed], [IsPasswordProtected], [Password], [Title], [Body], [Published], [TopicTemplateId], [MetaKeywords], [MetaDescription], [MetaTitle], [SubjectToAcl], [LimitedToStores]) VALUES (14, N'LoginPageNewCustomerText', 0, 0, 0, 0, 0, 1, 0, 0, NULL, NULL, N'<div><strong>Welcome to on job support!</strong></div>
            <div>&nbsp;</div>
            <div><span style="color: #e03e2d;">Note: We are not mediators.&nbsp;</span></div>
            <div><span style="color: #e03e2d;">We seek experianced professionals for direct support to our consultants.</span></div>
            <div><span style="color: #e03e2d;">Your details are shared with newly joined consultants after verification.&nbsp;</span></div>
            <div><span style="color: #e03e2d;">You can negoite service charges, </span><span style="color: #e03e2d;">which generally ranges from 30-80k INR based no of hours and technologies involved in the project.</span></div>
            <div>&nbsp;</div>
            <p>1.&nbsp; Thank you for your interest. With on job support, you can earn part time income from 30-80k INR.<br />2.&nbsp; Resgistration is mandatory and we will contact you within 5 business days.<br />3.&nbsp; Provide precise details during registration for accurate matching. This will help us reach out to you in a timely manner.<br />4.&nbsp; Our goal is to provide quality support to our consultants.<br />5.&nbsp; We need experianced professionals who can provide support on technologies like.<br />&nbsp; &nbsp; JAVA Full Stack<br />&nbsp; &nbsp; .NET Full Stack<br />&nbsp; &nbsp; Cyber Security<br />&nbsp; &nbsp; Dev Ops<br />&nbsp; &nbsp; Azure&nbsp;<br />&nbsp; &nbsp; AWS<br />&nbsp; &nbsp; Salesforce<br />&nbsp; &nbsp; React&nbsp;<br />&nbsp; &nbsp; Angular<br />&nbsp; &nbsp; SQL<br />&nbsp; &nbsp; MERN Developer<br />&nbsp; &nbsp; MEAN Developer<br />&nbsp; &nbsp; Python&nbsp;<br />&nbsp; &nbsp; etc.<br />5. You need be able to spend 2 hrs/day and mostly on weekdays. Get more income if you want to spend weekends as well. Choice is upto you.</p>
            <div>&nbsp;</div>
            <div>If you have any questions, please contact us via whatsapp..</div>', 1, 1, NULL, NULL, NULL, 0, 0)
   END