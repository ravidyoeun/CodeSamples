using Microsoft.Practices.Unity;
using Sabio.Web.Domain;
using Sabio.Web.Enums;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.User;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/profile")]
    public class UserProfileApiController : ApiController
    {
        //....// ================================ DEPENDENCY INJECTION START ===============================


            [Dependency]
            public IUserProfileService _UserProfileService { get; set; }


        //....// ================================ DEPENDENCY INJECTION START ===============================



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route(), HttpPost]

        public HttpResponseMessage ProfileInsert(CreateProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = _UserProfileService.UpdateProfile(model);
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route(), HttpGet]

        public HttpResponseMessage GetAllProfiles()
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            List<UserDomain> userList = _UserProfileService.GetAllProfiles();

            ItemsResponse<UserDomain> response = new ItemsResponse<UserDomain> { Items = userList };

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route("{userId}"), HttpPut]

        public HttpResponseMessage UpdateProfile(CreateProfileRequest model)
        {
            //- validate incoming payload model
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            // Create and call a UpdateProfile method in the userService
            bool isSuccessful = _UserProfileService.UpdateProfile(model);

            // Load the response Item with the success boolean (true).
            ItemResponse<bool> response = new ItemResponse<bool> { Item = isSuccessful };
            
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route("loggedin"), HttpGet]


        public HttpResponseMessage ProfileGetByUserId()
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }



            string currentUserId = UserService.GetCurrentUserId();

            UserDomain singleUser = _UserProfileService.ProfileGetByUserId(currentUserId);

            var response = new ItemResponse<UserDomain> { Item = singleUser};

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route("{userId}"), HttpDelete]

        public HttpResponseMessage DeleteProfile(string userId)
        {
            // Validate incoming payload model.
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            // Create and call a BlogUpdate method in the BlogService.
            bool isSuccessful = _UserProfileService.DeleteProfile(userId);

            // Load the response Item with the success boolean (true).
            ItemResponse<bool> response = new ItemResponse<bool> { Item = isSuccessful };
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [Route("{userId}/media"), HttpPut]

        public HttpResponseMessage UpdateProfileMediaId(UserProfileMediaIdUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            bool isSuccessful = _UserProfileService.UpdateProfileMediaId(model);

            ItemResponse<bool> response = new ItemResponse<bool> { Item = isSuccessful };
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}