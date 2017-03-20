using Microsoft.Practices.Unity;
using quotemule.Web.Domain;
using quotemule.Web.Domain.Quotes;
using quotemule.Web.Enums;
using quotemule.Web.Enums.QuoteWorkflow;
using quotemule.Web.Hubs;
using quotemule.Web.Models.Requests;
using quotemule.Web.Models.Requests.User;
using quotemule.Web.Models.Responses;
using quotemule.Web.Services;
using quotemule.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace quotemule.Web.Controllers.Api
{
    [RoutePrefix("api/states")]
    public class StateMachineApiController : ApiController
    {
        //....// ================================ DEPENDENCY INJECTION START ===============================


        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }
        [Dependency]
        public IQuoteRequestService _QuoteRequestService { get; set; }
        [Dependency]
        public IBidService _BidService { get; set; }

        //....// ================================ DEPENDENCY INJECTION END ===============================

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route("{userId}"), HttpPut]

        public HttpResponseMessage updateStateMachine(QuoteRequestDomain model)
        {
            //- validate incoming payload model
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            // Set the workflow state to Publish 
            //No valid leaving transitions are permitted from state 'Draft' for trigger 'Publish'. Consider ignoring the trigger.

           QuoteRequestDomain updatedQR = _QuoteRequestService.GetQuoteRequestById(model.QrId);

            bool isSuccessful = false;
            int CompanyId = updatedQR.CompanyId;
            string QRmessage = "";
            switch (model.EventId)
            {

                case QREvent.Cancel:
                    // change to appropriate event type
                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptCancel(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been CANCELLED";

                    break;
                case QREvent.Delete:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptDelete(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been DELETED";

                    break;
                case QREvent.Publish:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptePublish(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been PUBLISHED";

                    break;
                case QREvent.Review:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptPending(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been PENDED";

                    break;
                case QREvent.Republish:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptRepublish(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been REPUBLISHED";

                    break;
                case QREvent.Reject:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptReject(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been REJECTED";

                    break;
                case QREvent.Withdraw:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptWithdraw(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been WITHDRAWN";

                    break;
                case QREvent.Complete:

                    isSuccessful = _QuoteRequestService.QuoteRequestSMAttemptComplete(model.QrId);
                    QRmessage = "QuoteRequest <a href='/quoterequest/manage/" + model.QrId + "'>" + model.Name + "</a> has been COMPLETED";

                    break;

                default:
                    break;

            }


            List<CompanyEmployeeDomain> employee_ActiveQR = _UserProfileService.GetAllEmployees(CompanyId);

            SignalRHub.QuoteRequestUpdated(model, employee_ActiveQR, QRmessage);

            
            // Load the response Item with the success boolean (true).
            ItemResponse<bool> response = new ItemResponse<bool> { Item = isSuccessful };


            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route("{id}"), HttpGet]


        public HttpResponseMessage getStatusId(int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            QuoteRequestDomain qrObject = new QuoteRequestDomain();

            qrObject = _QuoteRequestService.GetQuoteRequestStatusId(id);

            QuoteRequestDomain qrStatus = new QuoteRequestDomain();

            qrStatus._Status = qrObject.Status;
            qrStatus._QrName = qrObject.QuoteRequestName;

            var response = new ItemResponse<QuoteRequestDomain> { Item = qrObject };

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // TESTING SMS MESSAGING API - RAVID YOEUN
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route(), HttpPost]
        public HttpResponseMessage sendText(NotifySMSRequest model)
        {
            ItemResponse<object> response = new ItemResponse<object>();

            try
            {
                response.Item = NotifySMSService.SendConfirmText(model);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}