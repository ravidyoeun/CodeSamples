USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[QuoteRequests_Insert]    Script Date: 3/19/2017 9:05:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




ALTER proc [dbo].[QuoteRequests_Insert]

		  @UserId nvarchar(128)
		, @CompanyId int
		, @Name nvarchar(256)
		, @DueDate datetime2(7) = null
		, @QRType int
		, @Id int output


AS


BEGIN


INSERT INTO [dbo].[QuoteRequests]
		   (
		     UserId
		   , CompanyId
		   , Name
		   , DueDate
		   , QuoteRequestType
		   , CreatedDate
		   , StatusId
		
           )

     VALUES
           (
		     @UserId
		   , @CompanyId
		   , @Name
		   , @DueDate
		   , @QRType
		   , GETDATE()
		   , 1
		   )

SET @Id = SCOPE_IDENTITY()

END


/* TEST CODE

Declare 
		 @UserId nvarchar(128) = 'User2'
		,@CompanyId int = 42
		,@Name nvarchar(256) = 'JOSH'
		,@QRType int = 1
		,@DueDate datetime2(7) = '2017-01-30 00:00:00'
		,@Id int

Execute QuoteRequests_Insert
		 @UserId
		,@CompanyId
		,@Name
		,@DueDate
		,@QRType
		,@Id

select *
From [dbo].[QuoteRequests]

*/