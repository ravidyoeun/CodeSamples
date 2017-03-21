using Microsoft.AspNet.Identity.EntityFramework;
using quotemule.Web.Domain;
using quotemule.Web.Enums;
using quotemule.Web.Exceptions;
using quotemule.Web.Models;
using quotemule.Web.Models.Requests;
using quotemule.Web.Models.Requests.company;
using quotemule.Web.Models.Requests.Uploads;
using quotemule.Web.Models.Requests.User;
using quotemule.Web.Models.Responses;
using quotemule.Web.Services;
using quotemule.Web.Services.S3Service;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using RazorEngine.Templating;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Text;
using SendGrid;
using quotemule.Web.Models.ViewModels;
using Microsoft.Practices.Unity;
using quotemule.Web.Services.Interfaces;

namespace quotemule.Web.Controllers.Api
{
    [RoutePrefix("api/invite")]
    public class InviteApiController : ApiController
    {
        [Dependency]
        public ITemplateEmailService _TemplateEmailService { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Create blank user profile by company invitation feature Developed by Ravid Yoeun
        // DI by Josh May
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        [Dependency]
        public ITokenService _TokenService { get; set; }


        //....// ===================================== DEPENDENCY INJECTION START =============================================

        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }

        //....// ===================================== DEPENDENCY INJECTION END =============================================

        [Route("registration"), HttpPost]

        public HttpResponseMessage PostRegistration(InviteRegistrationRequest model)
        {
            IdentityUser newUserRegistration;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            // When model is valid, CreateUser will post email and pw to database
            try
            {
                // creating a random password for the invited user
                string randomPassword = Guid.NewGuid().ToString();

                // setting the invited user's email and their password
                newUserRegistration = UserService.CreateUser(model.Email, randomPassword);

                //Instantiating a new blank profile
                CreateBlankProfileRequest newUserProfile = new CreateBlankProfileRequest();
                newUserProfile.UserEmail = model.Email;
                newUserProfile.CompanyId = model.CompanyId;
                newUserProfile.UserId = newUserRegistration.Id;

                // Instantiating a new user role
                InsertNewUserRequest newUserRole = new InsertNewUserRequest();
                newUserRole.UserRole = model.UserRole;
                newUserRole.UserId = newUserRegistration.Id;
                _UserProfileService.AddRoleToUser(newUserRole); //TODO: Confirm this method is not longer needed (K.H. advised he suspects code was changed to make this unecessary)

                // Inserting the Blank Profile data by calling on the UserService function for it.
                _UserProfileService.InsertBlankProfile(newUserProfile);


            }
            catch (IdentityResultException ex) // Display error code and message if user was not created
            {

                // var ExceptionError = new ErrorResponse("Failed to register new user. (server side)");

                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }

            // Insert new user's id into token table and generate new token
            string UserId = newUserRegistration.Id;

            // Pass new user's email and token into emailservice for activation link
            try
            {

                //- Pass the invite user's email data
                string NewUserEmail = newUserRegistration.Email;
                // Create inviteType token
                model.TokenType = Enums.TokenType.ForgotPassword;

                //- Instantiate a new token request
                InsertTokenRequest inviteUserToken = new InsertTokenRequest
                {
                    UserId = newUserRegistration.Id,
                    Email = model.Email,
                    TokenType = TokenType.ForgotPassword
                };
                //- Capturing user's information into the tokenRequest
                //inviteUserToken.UserId = newUserRegistration.Id;
                //inviteUserToken.Email = model.Email;
                //inviteUserToken.TokenType = TokenType.ForgotPassword;
                if(inviteUserToken.TokenType != 0)
                {
                    // Do something
                }

                Guid token = _TokenService.RegisterToken(inviteUserToken);

                //- Create the link for setting a new password 
                string link = "/public/setpassword/" + token.ToString();

                //- Use HttpContext to generate a clickable link in the email 
                link = HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) + link;
                // Call the service for the forgot password link
                //- Send Link to user email 
                EmailRequest confirm = new EmailRequest();

                // Building the object's information 
                confirm._email_link = link;
                confirm._CompanyName = model.CompanyName;
                confirm.UserEmail = newUserRegistration.Email;

             
                // sends user invitation via email and render HTML string
                // TemplateEmailService was created to render HTML pages to a string
                string renderedHTML = _TemplateEmailService.RazorViewToString("~/Views/Email_Template/Email_Template.cshtml", confirm);

                // Instantiating a new SendGridMessage
                SendGridMessage ActivateCrEmail = new SendGridMessage();
                ActivateCrEmail.AddTo(confirm.UserEmail);
                ActivateCrEmail.From = new MailAddress("contact@quotemule.dev");
                // The Stringified HTML page is now converted by SendGridMessage with .Html
                ActivateCrEmail.Html = renderedHTML;
                ActivateCrEmail.Subject = "You've been invited! Please set your password for your account.";

                // Instantiating the sendGridKey
                var sendGridKey = new SendGrid.Web(ConfigService.GetSendGridKey());

                // Delivering the email message
                sendGridKey.DeliverAsync(ActivateCrEmail);

                //UserEmailService useremailservice = new UserEmailService();

                //UserEmailService.SendEmail(confirm);

            }
            catch (NotImplementedException)
            {

                var ExceptionError = new ErrorResponse("Failed to send activation email to new user");

                return Request.CreateResponse(HttpStatusCode.BadRequest, ExceptionError);

            }


            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

    }
}
