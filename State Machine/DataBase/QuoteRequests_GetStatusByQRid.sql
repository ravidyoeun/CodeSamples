USE [C27]
GO
/****** Object:  StoredProcedure [dbo].[QuoteRequests_GetStatusByQRid]    Script Date: 3/19/2017 9:05:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[QuoteRequests_GetStatusByQRid]/*this is the name of your SQL function, CREATE FIRST then ALTER*/
	@QuoteRequestId int
AS

BEGIN

SELECT [Id]
	,[StatusId]
	,[Name]
	

FROM dbo.QuoteRequests
WHERE [Id] = @QuoteRequestId;

END

/*-------------TEST CODE -------------------


Execute [dbo].[QuoteRequests_GetStatusByQRid] 237

DECLARE
	@QuoteRequestId int = 246

EXECUTE dbo.[QuoteRequests_GetStatusByQRid]
		@QuoteRequestId
	    
*/