USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[UserProfile_Insert_with_UserId]    Script Date: 3/22/2017 2:10:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER Procedure [dbo].[UserProfile_Insert_with_UserId]
	  @userId		nvarchar(128)
	 ,@email		nvarchar(256)
	 ,@companyId	 int
	 ,@id			int OUTPUT


As
Begin
INSERT INTO [dbo].[UserProfile]
           ([userId]
		   ,email
		   ,companyId)
     VALUES
           (@userId
		   ,@email
		   ,@companyId)
SET @id = SCOPE_IDENTITY()

END


/*
Execute [dbo].[UserProfile_Insert_with_UserId] 'sabioUser@sabio.la', 0

Select * From dbo.UserProfile
*/