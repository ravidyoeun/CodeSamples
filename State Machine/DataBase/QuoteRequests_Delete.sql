USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[QuoteRequests_Delete]    Script Date: 3/19/2017 9:04:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[QuoteRequests_Delete] /*this is the name of your SQL function, CREATE FIRST then ALTER*/
	@Id int
AS

/*-------------TEST CODE -------------------


DECLARE
		@id int = 1

EXECUTE dbo.BlogPosts_delete
		@id                  
                            	

FROM [dbo].[BlogPosts]

*/

BEGIN


DELETE FROM [dbo].[QuoteRequests] 

WHERE 
[Id] = @Id

END