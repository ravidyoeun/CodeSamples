using Microsoft.Practices.Unity;
using Sabio.Web.Models;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.User;
using Sabio.Web.Services.Interfaces;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sabio.Web.Services
{
	public class UserEmailService : BaseService, IUserEmailService
	{
		[Dependency]
		public ITokenService _TokenService { get; set; }
		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public void SendEmail(EmailRequest model)
		{
			var Email = new SendGridMessage
			{
				From = new System.Net.Mail.MailAddress(ConfigService.GetSupportEmailAddress()),
				Subject = model.Subject,
				Text = model.Content
			};

			Email.AddTo(model.UserEmail);

			var transportWeb = new SendGrid.Web(ConfigService.GetSendGridKey());

			transportWeb.DeliverAsync(Email);

		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public  EmailRequest BuildAccountConfirmEmail(InsertTokenRequest model)
		{
			
			string Token = _TokenService.RegisterToken(model).ToString();

			string ConfirmationUrl = String.Concat("http://quotemule.dev/Public/ConfirmAccount/", Token);

			string HelpDeskUrl = "https://help.market.quotemule.com";

			EmailRequest ToSend = new EmailRequest
			{
				UserEmail = model.Email,
				Subject = "Confirm Your QuoteMule Account",
				Content = String.Format("Hello,\n\nWelcome to QuoteMule!\n\nTo complete your registration, please click the link below:\n\n{0}\n\nIf you need help or have any questions, please visit {1}\n\nThanks!\n\nQuoteMule", ConfirmationUrl, HelpDeskUrl)
			};

			return ToSend;
		}



		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public void SendRegistrationConfirmationEmail(RegisterViewModel model, string userId)
		{
			InsertTokenRequest insertToken = new InsertTokenRequest
			{
				Email = model.Email,
				UserId = userId,
				TokenType = Enums.TokenType.Registration
			};

			EmailRequest ToSend = new EmailRequest();
			try
			{
				
				ToSend = BuildAccountConfirmEmail(insertToken);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			SendEmail(ToSend);
		}
		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


	}
}