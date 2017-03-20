

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[QuoteRequests_Update_Status]
	@StatusId int
   ,@Id int
AS



BEGIN

UPDATE [dbo].[QuoteRequests]

SET 
 [StatusId] = @StatusId
,[UpdatedDate] = GETDATE()

WHERE
	Id = @Id

END


/*-------------TEST CODE -------------------


DECLARE
		@StatusId = 20
	   ,@Id int = 1

EXECUTE [dbo].[QuoteRequests_Update_Status]
		@StatusId
		,@Id                   
                            	
SELECT 
		  [id]
		, [UserId]
        ,[CompanyId]
        ,[Name]
		,[DueDate]
		,[CreatedDate]


FROM [dbo].[QuoteRequests]

*/