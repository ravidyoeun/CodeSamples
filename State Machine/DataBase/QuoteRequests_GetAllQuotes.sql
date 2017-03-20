USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[QuoteRequests_GetAllQuotes]    Script Date: 3/19/2017 9:03:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




ALTER proc [dbo].[QuoteRequests_GetAllQuotes]

AS

BEGIN

SELECT
[Id],
[UserId],
[CompanyId],
[Name],
[DueDate],
[CreatedDate],
[UpdatedDate],
[StatusId]


FROM
[dbo].[QuoteRequests]



END


/* TEST CODE

Declare 
		 @QuoteId int = 1

Execute dbo.[QuoteRequests_GetAllQuotes]
		 
		 @QuoteId

select *
From [dbo].[QuoteMapping]

Where 
		@QuoteId = 1

*/