using Microsoft.Practices.Unity;
using Sabio.Data;
using Sabio.Web.Domain;
using Sabio.Web.Domain.Quotes;
using Sabio.Web.Enums.QuoteRequestBidWorkflow;
using Sabio.Web.Enums.QuoteWorkflow;
using Sabio.Web.Hubs;
using Sabio.Web.Enums;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.Quotes;
using Sabio.Web.Models.Requests.User;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services.Interfaces;
using Sabio.Web.Services.S3Service;
using Sabio.Web.Services.Workflow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;


namespace Sabio.Web.Services
{
	public class QuoteRequestService : BaseService, IQuoteRequestService 
	{
		[Dependency]
		public IUserProfileService _UserProfileService { get; set; }

		// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


		public List<QuoteRequestsMediaDomain> GetAllMediaByQuoteRequestId(int QRID)
		{

			List<QuoteRequestsMediaDomain> quoteMediaList = null;

			DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequestsMedia_SelectAllByQuoteRequestId"
				, inputParamMapper: delegate (SqlParameterCollection paramCollection)
				{
					// Pass in data to the Database
					paramCollection.AddWithValue("@QuoteRequestsId", QRID);

				}, map: delegate (IDataReader reader, short set)
				{
					QuoteRequestsMediaDomain SingleQuoteRequestMedia = new QuoteRequestsMediaDomain();
					int startingIndex = 0;


					SingleQuoteRequestMedia.QuoteRequestId = reader.GetSafeInt32(startingIndex++);
					SingleQuoteRequestMedia.MediaId = reader.GetSafeInt32(startingIndex++);

					if (quoteMediaList == null)
					{
						quoteMediaList = new List<QuoteRequestsMediaDomain>();
					}

					quoteMediaList.Add(SingleQuoteRequestMedia);
				});

			return quoteMediaList;
		}



		// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		public List<MediaDomain> GetMediaByQRId(int qrId)
		{
			List<MediaDomain> mediaList = null;

			try
			{
				// Returns a list of QR-id and media-id pairs. 
				List<QuoteRequestsMediaDomain> mediaItems = GetAllMediaByQuoteRequestId(qrId);

				// We will be using the media ids to get a list of media domains.

				if(mediaItems != null)
				{
					foreach (QuoteRequestsMediaDomain qrMedia in mediaItems)
					{
						MediaService mediaService = new MediaService();

						MediaDomain media = mediaService.GetMediaById(qrMedia.MediaId);

						if (mediaList == null)
						{
							mediaList = new List<MediaDomain>();
						}

						mediaList.Add(media);
					}
				}
			}
			catch(Exception ex)
			{
				throw ex;
			}

			return mediaList;

		}

		// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



		public bool QuoteRequestMediaListInsert(QRMediaInsertListRequest model)
		{
			bool isSuccessful = false;


			foreach (int mediaIdItem in model.MediaIdList)
			{
				QRMediaInsertRequest mediaItem = new QRMediaInsertRequest();
				mediaItem.MediaId = mediaIdItem;
				mediaItem.QuoteRequestId = model.QuoteRequestId;
				//Call Service method to sent MediaItem
				InsertQuoteRequestMediaItem(mediaItem);

			}

			isSuccessful = true;

			return isSuccessful;
		}


		// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		public void InsertQuoteRequestMediaItem(QRMediaInsertRequest mediaItem)
		{

			DataProvider.ExecuteNonQuery(GetConnection, "dbo.QuoteRequestsMedia_Insert"
				, inputParamMapper: delegate (SqlParameterCollection paramCollection)
				{
					paramCollection.AddWithValue("@QuoteRequestId", mediaItem.QuoteRequestId);
					paramCollection.AddWithValue("@MediaId", mediaItem.MediaId);

				},
				returnParameters: delegate (SqlParameterCollection param)

				{

				});

		}


		// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


		public bool DeleteQuoteRequestsMedia(int QRID)
		{
			bool success = false;

			try
			{
				DataProvider.ExecuteNonQuery(GetConnection, "dbo.QuoteReqeuestsMedia_Delete"
					   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
					   {
						   paramCollection.AddWithValue("@QuoteRequestId", QRID);

						   success = true;
					   });
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return success;
		}



		/// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



		public List<QuoteRequestDomain> GetAllQuoteRequest()
		{

			List<QuoteRequestDomain> quoteRequestList = null;

			try
			{
				DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequests_GetAllQuotes"
			  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
			  {
			  }, map: delegate (IDataReader reader, short set)
			  {
				  QuoteRequestDomain SingleQuoteRequest = new QuoteRequestDomain();
				  int startingIndex = 0; //startingOrdinal

				  SingleQuoteRequest.QrId = reader.GetSafeInt32(startingIndex++);
				  SingleQuoteRequest.UserId = reader.GetSafeString(startingIndex++);
				  SingleQuoteRequest.CompanyId = reader.GetSafeInt32(startingIndex++);
				  SingleQuoteRequest.Name = reader.GetSafeString(startingIndex++);
				  SingleQuoteRequest.DueDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.CreatedDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.UpdatedDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.Status = (QRState)reader.GetSafeInt32(startingIndex++);

				  if (quoteRequestList == null)
				  {
					  quoteRequestList = new List<QuoteRequestDomain>();
				  }

				  quoteRequestList.Add(SingleQuoteRequest);
			  }
		   );
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return quoteRequestList;
		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



		public List<QuoteRequestDomain> GetQuoteRequestsByCompanyId(int CompanyId)
		{

			List<QuoteRequestDomain> quoteList = null;

			try
			{
				DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequests_GetByCompanyId"
			  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
			  {
				  paramCollection.AddWithValue("@CompanyId", CompanyId);
			  }, map: delegate (IDataReader reader, short set)
			  {
				  QuoteRequestDomain SingleQuoteRequest = new QuoteRequestDomain();
				  int startingIndex = 0; //startingOrdinal

				  SingleQuoteRequest.QrId = reader.GetSafeInt32(startingIndex++);
				  SingleQuoteRequest.UserId = reader.GetSafeString(startingIndex++);
				  SingleQuoteRequest.CompanyId = reader.GetSafeInt32(startingIndex++);
				  SingleQuoteRequest.Name = reader.GetSafeString(startingIndex++);
				  SingleQuoteRequest.DueDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.CreatedDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.UpdatedDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.Status = (QRState)reader.GetSafeInt32(startingIndex++);

				  SingleQuoteRequest.StatusName = SingleQuoteRequest.Status.ToString();

				  if (quoteList == null)
				  {
					  quoteList = new List<QuoteRequestDomain>();
				  }
			  
				  

				  quoteList.Add(SingleQuoteRequest);
			  }
		   );
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return quoteList;
		}

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		public List<QuoteRequestDomain> GetQuoteRequestsByCompanyIdAndStatus(int CompanyId)
		{

			List<QuoteRequestDomain> quoteList = null;

			try
			{
				DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequests_GetCompanyId_Status"
			  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
			  {
				  paramCollection.AddWithValue("@CompanyId", CompanyId);
			  }, map: delegate (IDataReader reader, short set)
			  {
				  QuoteRequestDomain SingleQuoteRequest = new QuoteRequestDomain();
				  int startingIndex = 0; //startingOrdinal

				  SingleQuoteRequest.QrId = reader.GetSafeInt32(startingIndex++);
				  SingleQuoteRequest.UserId = reader.GetSafeString(startingIndex++);
				  SingleQuoteRequest.CompanyId = reader.GetSafeInt32(startingIndex++);
				  SingleQuoteRequest.Name = reader.GetSafeString(startingIndex++);
				  SingleQuoteRequest.DueDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.CreatedDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.UpdatedDate = reader.GetSafeDateTime(startingIndex++);
				  SingleQuoteRequest.Status = (QRState)reader.GetSafeInt32(startingIndex++);

				  SingleQuoteRequest.StatusName = SingleQuoteRequest.Status.ToString();

				  if (quoteList == null)
				  {
					  quoteList = new List<QuoteRequestDomain>();
				  }



				  quoteList.Add(SingleQuoteRequest);
			  }
		   );
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return quoteList;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



		public QuoteRequestDomain GetQuoteRequestById(int id)
		{
			QuoteRequestDomain SingleQuote = null;

			try
			{
				DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequests_GetById"
			, inputParamMapper: delegate (SqlParameterCollection paramCollection)
			{
				paramCollection.AddWithValue("@QuoteRequestId", id);

			}, map: delegate (IDataReader reader, short set)
			{
				SingleQuote = new QuoteRequestDomain();

				int startingIndex = 0; //startingOrdinal

				SingleQuote.QrId = reader.GetSafeInt32(startingIndex++);
				SingleQuote.UserId = reader.GetSafeString(startingIndex++);
				SingleQuote.CompanyId = reader.GetSafeInt32(startingIndex++);
				SingleQuote.Name = reader.GetSafeString(startingIndex++);
				SingleQuote.DueDate = reader.GetSafeDateTime(startingIndex++);
				SingleQuote.CreatedDate = reader.GetSafeDateTime(startingIndex++);
				SingleQuote.UpdatedDate = reader.GetSafeDateTime(startingIndex++);
				SingleQuote.Status = (QRState)reader.GetSafeInt32(startingIndex++);
				SingleQuote.AddressId = reader.GetSafeInt32(startingIndex++);
				SingleQuote.QrType = (QuoteRequestType)reader.GetSafeInt32(startingIndex++);



			});
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return SingleQuote;
		}

		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Get Current Status of a Quote by its QuoteId
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public QuoteRequestDomain GetQuoteRequestStatusId(int id)
		{
			QuoteRequestDomain quoteStatus = null;

			try
			{
				DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequests_GetStatusByQRid"
			, inputParamMapper: delegate (SqlParameterCollection paramCollection)
			{
				paramCollection.AddWithValue("@QuoteRequestId", id);

			}, map: delegate (IDataReader reader, short set)
			{
				quoteStatus = new QuoteRequestDomain();

				int startingIndex = 0; //startingOrdinal

				quoteStatus.QrId = reader.GetSafeInt32(startingIndex++);
				quoteStatus.Status = (QRState)reader.GetSafeInt32(startingIndex++);
				//quoteStatus.EventId = (QREvent)reader.GetSafeInt32(startingIndex++);
				quoteStatus.QuoteRequestName = reader.GetSafeString(startingIndex++);

				// Passing through the values to the Domain Object
				quoteStatus.StatusName = quoteStatus.Status.ToString();
		   





			});
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return quoteStatus;
		}
		
		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



		public int InsertQuoteRequest(QuoteRequestInsertRequest model)
		{
			int id = 0;

			try
			{
				DataProvider.ExecuteNonQuery(GetConnection, "dbo.QuoteRequests_Insert"
					  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
					  {
						  paramCollection.AddWithValue("@UserId", model.UserId);
						  paramCollection.AddWithValue("@CompanyId", model.CompanyId);
						  paramCollection.AddWithValue("@Name", model.Name);
						  paramCollection.AddWithValue("@QRType", model.QRType);

						  SqlParameter p = new SqlParameter("@id", System.Data.SqlDbType.Int);
						  p.Direction = System.Data.ParameterDirection.Output;

						  paramCollection.Add(p);

					  }, returnParameters: delegate (SqlParameterCollection param)
					  {
						  int.TryParse(param["@Id"].Value.ToString(), out id);

					  });
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return id;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



		public bool UpdateQuoteRequest(QuoteRequestUpdateRequest model)
		{
			bool success = false;

			try
			{
				DataProvider.ExecuteNonQuery(GetConnection, "dbo.QuoteRequests_Update"
				   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
				   {
					   paramCollection.AddWithValue("@Name", model.Name);
					   paramCollection.AddWithValue("@DueDate", model.DueDate);
					   paramCollection.AddWithValue("@Id", model.QrId);
					   paramCollection.AddWithValue("@AddressId", model.AddressId);

					   success = true;
				   });
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return success;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



		private static bool UpdateQuoteRequestStatus(QrUpdateStatusRequest model)
		{
			bool success = false;

			try
			{
				DataProvider.ExecuteNonQuery(GetConnection, "dbo.QuoteRequests_Update_Status"
				   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
				   {
					   paramCollection.AddWithValue("@StatusId", model.Status);
					   paramCollection.AddWithValue("@Id", model.QrId);

					   success = true;
				   });
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return success;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public bool DeleteQuoteRequest(int id)
		{
			bool success = false;

			try
			{
				DataProvider.ExecuteNonQuery(GetConnection, "dbo.QuoteRequests_Delete"
					   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
					   {
						   paramCollection.AddWithValue("@Id", id);

						   success = true;
					   });
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return success;
		}



		// /////////////////////////////////////////////////////////////////////////////////////////
		// Workflow Methods by Kevin Horan & Ravid Yoeun
		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		//- SABIOFIXME - replace naked variables with Request Model
		public bool QuoteRequestSMAttemptCancel(int id, string userId)
		{
			bool success = false;
			// Cancelling is the right of the originator
			QuoteRequestDomain model = GetQuoteRequestById(id);

			//- SABIOFIXME - this should be a check against the companyId instead of userId
			if (model.UserId == userId)
			{
				try
				{
					var StateHandler = new QuoteRequestStateService(model);

					// Verify this is valid action
					StateHandler.StateMachine.Fire(QREvent.Cancel);

					// Update Status on THIS model
					model.Status = QRState.Cancelled;

					// Update status in DB
					QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
					{
						QrId = model.QrId,
						Status = model.Status
					};

					UpdateQuoteRequestStatus(QrUpdate);
					
				}
				//- SABIOFIXME - Implement a catch for InvalidOperationException that sets success to false
				catch (Exception ex)
				{
					throw ex;
				}
			}
			return success;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public bool QuoteRequestSMAttemptComplete(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Complete);

				// Update Status on THIS model
				model.Status = QRState.Complete;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

				
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return success;
		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public bool QuoteRequestSMAttemptReview(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Review);

				// Update Status on THIS model
				model.Status = QRState.Pending;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

				
			}
			catch (InvalidOperationException)
			{
				return success = false;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}




		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		//public bool StateQuoteCancel(int id)
		//{
		//    bool success = false;
		//    try
		//    {
		//        QuoteRequestDomain model = GetQuoteById(id);

		//        QuoteStateService StateHandler = new QuoteStateService(model);

		//        // Verify this is valid action
		//        StateHandler.StateMachine.Fire(QREvent.Cancel);




		//        // Update Status on THIS model
		//        model.Status = QRState.Cancelled;

		//        // Update status in DB
		//        QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
		//        {
		//            Id = model.I;d,
		//            Status = model.Status
		//        };

		//        UpdateQuoteStatus(QrUpdate);
		//    }
		//    catch (InvalidOperationException)
		//    {
		//        return success = false;
		//    }
		//    catch (Exception ex)
		//    {
		//        throw ex;
		//    }
		//    return success;
		//}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public bool QuoteRequestSMAttemptPending(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Review);

				// Update Status on THIS model
				model.Status = QRState.Pending;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);
				

				//- Changing all Bids for QR to pending
				BidService BidService = new BidService();
				QuoteRequestItemService QRIService = new QuoteRequestItemService();

				List<QuoteRequestItemDomain> allQRItemsByQr = QRIService.GetAllQuoteRequestItems(model.QrId);

				if (allQRItemsByQr != null)
				{

					foreach (QuoteRequestItemDomain QRItem in allQRItemsByQr)
					{
						List<BidDomain> allBidsByQri = BidService.BidGetByQriId(QRItem.QrItemId);

						if (allBidsByQri != null)
						{
							foreach (BidDomain Bid in allBidsByQri)
							{

								BidDomain currentBid = Bid;

								QuoteRequestBidStatusRequest BidModel = BidService.GetQuoteRequestBidStatusId(currentBid.BidId);

								QuoteRequestBidStateService BidStateHandler = new QuoteRequestBidStateService(BidModel);

								BidStateHandler.StateMachine.Fire(QuoteRequestBidEvent.Review);

								currentBid.BidStatus = QuoteRequestBidState.Pending;

								QuoteRequestBidStatusRequest BidStateUpdate = new QuoteRequestBidStatusRequest
								{
									BidId = currentBid.BidId,
									BidStatus = currentBid.BidStatus
								};

								BidService.UpdateQuoteRequestBidStatus(BidStateUpdate);
							}
						}
					}
				}
			}
			catch (InvalidOperationException)
			{
				return success;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public bool QuoteRequestSMAttemptReject(int id)
		{
			 bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Reject);

				// Update Status on THIS model
				model.Status = QRState.Cancelled;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

			   


				

			}
			catch (Exception ex)
			{
				throw ex;
			}

			return success;
		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public bool QuoteRequestSMAttemptCancel(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Cancel);

				// Update Status on THIS model
				model.Status = QRState.Draft;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

		   
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}

		public bool QuoteRequestSMAttemptDelete(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Delete);

				// Update Status on THIS model
				model.Status = QRState.Cancelled;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

				
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}

		public bool QuoteRequestSMAttemptePublish(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Publish);

				// Update Status on THIS model
				model.Status = QRState.Active;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}

		public bool QuoteRequestSMAttemptRepublish(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Republish);

				// Update Status on THIS model
				model.Status = QRState.Active;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}

		public bool QuoteRequestSMAttemptWithdraw(int id)
		{
			bool success = false;
			try
			{
				QuoteRequestDomain model = GetQuoteRequestById(id);

				QuoteRequestStateService StateHandler = new QuoteRequestStateService(model);

				// Verify this is valid action
				StateHandler.StateMachine.Fire(QREvent.Withdraw);

				// Update Status on THIS model
				model.Status = QRState.Draft;

				// Update status in DB
				QrUpdateStatusRequest QrUpdate = new QrUpdateStatusRequest
				{
					QrId = model.QrId,
					Status = model.Status
				};

				UpdateQuoteRequestStatus(QrUpdate);

			   
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return success;
		}

		public bool QuoteRequestMediaInsert(QRMediaInsertRequest model)
		{
			throw new NotImplementedException();
		}
	}
}
