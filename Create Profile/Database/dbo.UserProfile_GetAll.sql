USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[UserProfile_GetAll]    Script Date: 3/22/2017 2:09:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[UserProfile_GetAll]

AS

BEGIN

SELECT 

	[userId]
	,[firstName]
	,[lastName]
	,[companyId]
	,[phoneNumber]
	,[mediaId]
	, (SELECT url FROM [Media] WHERE [UserProfile].mediaId = [Media].id) AS mediaUrl
	,(SELECT name FROM [Company] WHERE [UserProfile].companyId = [Company].id) AS companyName

FROM [dbo].[UserProfile]


END

/*-------------TEST CODE -------------------
EXECUTE [dbo].[UserProfile_GetAll]


SELECT 

	[userId]
	,[firstName]
	,[lastName]
	,[companyId]
	,[phoneNumber]
	,[mediaId]
	, (SELECT url FROM [Media] WHERE [UserProfile].mediaId = [Media].id) AS mediaUrl
	,(SELECT name FROM [Company] WHERE [UserProfile].companyId = [Company].id) AS companyName

FROM [dbo].[UserProfile]



*/