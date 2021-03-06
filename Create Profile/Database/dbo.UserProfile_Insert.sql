USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[UserProfile_Insert]    Script Date: 3/22/2017 2:10:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER Procedure [dbo].[UserProfile_Insert] 
	 @userId		nvarchar(128)
	,@email			nvarchar(256)
	,@firstName		nvarchar(150)
	,@lastName		nvarchar(150)
	,@companyId		int
	,@phoneNumber	nvarchar(MAX)
	,@id			int OUTPUT

As
SET @id = SCOPE_IDENTITY()

Begin
INSERT INTO [dbo].[UserProfile]
           ([userId]
		   ,[email]
           ,[firstName]
           ,[lastName]
           ,[companyId]
           ,[phoneNumber])

     VALUES
           (@userId
		   ,@email
           ,@firstName
           ,@lastName
           ,@companyId
           ,@phoneNumber)

SET @id = SCOPE_IDENTITY()

END


/*
Declare
			@userId nvarchar(128) = '37bdb705-3223-4133-886d-24fd6d7c76df'
		   ,@email nvarchar(256) = 'SteveJobs@gmail.com'
           ,@firstName nvarchar(150) = 'Steve'
           ,@lastName nvarchar(150) = 'Job'
           ,@jobTitle nvarchar(50) = 'Janitor'
           ,@companyId int = '2'
           ,@phoneNumber nvarchar(MAX) = '34536353636'
		   ,@id int

Execute [dbo].[UserProfile_Insert] 

			@userId 
		   ,@email 
           ,@firstName 
           ,@lastName 
           ,@jobTitle 
           ,@companyId 
           ,@phoneNumber 
		    ,@id
			
SELECT
			userId 
		   ,email 
           ,firstName 
           ,lastName 
           ,jobTitle 
           ,companyId 
           ,phoneNumber
		   ,id
		   ,mediaId

From [dbo].[UserProfile]
*/