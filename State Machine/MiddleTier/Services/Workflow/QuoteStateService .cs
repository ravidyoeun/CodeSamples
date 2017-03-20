using quotemule.Web.Domain;
using quotemule.Web.Domain.Quotes;
using quotemule.Web.Enums.QuoteRequestBidWorkflow;
using Stateless;
using System;

namespace quotemule.Web.Services.Workflow
{
    public class QuoteStateService
    {
        private readonly QuoteDomain Quote;

        public QuoteStateService(QuoteDomain quote)
        {
            {
                if (quote == null)
                {
                    throw new ArgumentNullException(nameof(quote));
                }

                Quote = quote;

                //if (quote.Status == QRState.Cancelled)
                //{
                //    quote.Status = QRState.Draft;
                //}


                 StateMachine = new StateMachine<QuoteState, QuoteEvent>(Quote.QuoteState);

                // State which actions may be taken in each step of the work flow
                StateMachine.Configure(QuoteState.Pending)
                    .Permit(QuoteEvent.Submit, QuoteState.Active);

                StateMachine.Configure(QuoteState.Active)
                    .Permit(QuoteEvent.Accept, QuoteState.Approved)
                   .Permit(QuoteEvent.AcceptedAddContract, QuoteState.ApprovedAddContract)
                   .Permit(QuoteEvent.ContractAdded, QuoteState.ContractAddedForApproval)
                   .Permit(QuoteEvent.Alter, QuoteState.Alter)
                   .Permit(QuoteEvent.Decline, QuoteState.Declined);

                StateMachine.Configure(QuoteState.Approved)
                    .Permit(QuoteEvent.Complete, QuoteState.Completed);

                StateMachine.Configure(QuoteState.ApprovedAddContract)
                    .Permit(QuoteEvent.ContractAdded, QuoteState.Approved);

                StateMachine.Configure(QuoteState.ContractAddedForApproval)
                    .Permit(QuoteEvent.ContractApproved, QuoteState.Completed);

                StateMachine.Configure(QuoteState.Alter)
                    .Permit(QuoteEvent.Submit, QuoteState.Active);


                StateMachine.Configure(QuoteState.Declined)              
                    .Ignore(QuoteEvent.Submit)
                    .Ignore(QuoteEvent.Accept)
                    .Ignore(QuoteEvent.AcceptedAddContract)
                    .Ignore(QuoteEvent.ContractAdded)
                    .Ignore(QuoteEvent.Alter)
                    .Ignore(QuoteEvent.Complete);
            }
        }

        public StateMachine<QuoteState, QuoteEvent> StateMachine { get; }
    }
}
