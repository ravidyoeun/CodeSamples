using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using Sabio.Data;
using Sabio.Web.Domain;
using Sabio.Web.Domain.Quotes;
using Sabio.Web.Enums.QuoteWorkflow;
using Sabio.Web.Models;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.User;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sabio.Web.Services
{
    public class UserProfileService : BaseService, IUserProfileService
    {

        //....// =================================== DEPENDENCY INJECTION START ================================

        [Dependency]
        public IAdminService _AdminService { get; set; }

        [Dependency]
        public ITokenService _TokenService { get; set; }
        

        //....// =================================== DEPENDENCY INJECTION END =================================


        //....// =============================================================================================

        public bool IsCompanyOwner()
        {
            IdentityUser user = UserService.GetCurrentUser();

            string role = _AdminService.GetRoleByUserId(user.Id);

            if (role == _AdminService.GetCompanyOwnerRoleId())
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        //....// =============================================================================================

        public bool IsCompanyOwner(string userId)
        {
            //IdentityUser user = UserService.GetCurrentUser();

            string role = _AdminService.GetRoleByUserId(userId);

            if (role == _AdminService.GetCompanyOwnerRoleId())
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //....// =============================================================================================

        public bool IsCompanyAdmin(string userId)
        {
            //IdentityUser user = UserService.GetCurrentUser();

            string role = _AdminService.GetRoleByUserId(userId);

            if (role == _AdminService.GetCompanyAdminRoleId())
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //....// =============================================================================================

        public bool ConfirmUserEmail(Guid token)
        {
            bool result = false;

            // Get User.
            ApplicationUser user = _TokenService.GetUserByToken(token, Enums.TokenType.Registration);

            if(user == null)
            {
                user = _TokenService.GetUserByToken(token, Enums.TokenType.ForgotPassword);
            }

            // Expire token.
            _TokenService.MarkTokenAsUsed(token);

            // Confirm Email address in the DB.
            if (user != null)
            {
                {
                    DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_Confirm_Email"
                    , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                    {
                        // This assumes an account email and accout userName are the same
                        paramCollection.AddWithValue("@UserName", user.Email);
                    }
                    );
                }

                // set result to true.
                result = true;
            }

            return result;
        }






        //....// =============================================================================================

        public int InsertProfile(CreateProfileRequest model)
        {
            int id = 0;

            try
            {
                DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserProfile_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@userId", model.UserId);
                   paramCollection.AddWithValue("@firstName", model.FirstName);
                   paramCollection.AddWithValue("@lastName", model.LastName);
                   paramCollection.AddWithValue("@userRole", model.userRole);
                   paramCollection.AddWithValue("@name", model.CompanyName);
                   paramCollection.AddWithValue("@phoneNumber", model.PhoneNumber);

                   SqlParameter p = new SqlParameter("@id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@id"].Value.ToString(), out id);
               });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return id;
        }






        //....// =============================================================================================

        public int InsertBlankProfile(CreateBlankProfileRequest model)
        {
            int id = 0;

            try
            {
                DataProvider.ExecuteNonQuery(GetConnection, "UserProfile_Insert_with_UserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@userId", model.UserId);
                    paramCollection.AddWithValue("@email", model.UserEmail);
                    paramCollection.AddWithValue("@companyId", model.CompanyId);

                    SqlParameter p = new SqlParameter("@id", System.Data.SqlDbType.Int);
                    p.Direction = System.Data.ParameterDirection.Output;

                    paramCollection.Add(p);
                },
                returnParameters: delegate (SqlParameterCollection param)
                {
                    int.TryParse(param["@id"].Value.ToString(), out id);
                }
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return id;
        }






        //....// =============================================================================================

        public bool UpdateProfile(CreateProfileRequest model)
        {
            bool result = false;

            try
            {
                DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserProfile_Update"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@userId", model.UserId);
                    paramCollection.AddWithValue("@firstName", model.FirstName);
                    paramCollection.AddWithValue("@lastName", model.LastName);
                    paramCollection.AddWithValue("@phoneNumber", model.PhoneNumber);
                    paramCollection.AddWithValue("@mediaId", model.mediaId);
                    result = true;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }






        //....// =============================================================================================

        public List<UserDomain> GetAllProfiles()
        {

            List<UserDomain> userList = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfile_GetAll"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

               }, map: delegate (IDataReader reader, short set)
               {
                   UserDomain singleUser = new UserDomain();
                   int startingIndex = 0;

                   singleUser.UserId = reader.GetSafeString(startingIndex++);
                   singleUser.FirstName = reader.GetSafeString(startingIndex++);
                   singleUser.LastName = reader.GetSafeString(startingIndex++);
                   singleUser.CompanyId = reader.GetSafeInt32(startingIndex++);
                   singleUser.PhoneNumber = reader.GetSafeString(startingIndex++);
                   singleUser.MediaId = reader.GetSafeInt32(startingIndex++);
                   singleUser.MediaUrl = reader.GetSafeString(startingIndex++);
                   singleUser.CompanyName = reader.GetSafeString(startingIndex++);

                   if (userList == null)
                   {
                       userList = new List<UserDomain>();
                   }
                   userList.Add(singleUser);
               });

            return userList;
        }






        //....// =============================================================================================

        public List<CompanyEmployeeDomain> GetEmployeesByCompanyId(EmployeeProfileRequest model)
        {
            List<CompanyEmployeeDomain> companyEmployeeList = null;
            int totalCount = 0;
            try
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfile_GetByCompanyId"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@companyId", model.companyId);
                  // server side pagination
                  paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                      paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);

                  }, map: delegate (IDataReader reader, short set)
                  {
                      var singleUser = new CompanyEmployeeDomain();

                      int startingIndex = 0; //startingOrdinal

                      totalCount = reader.GetSafeInt32(startingIndex++);
                      singleUser.UserId = reader.GetSafeString(startingIndex++);
                      singleUser.Email = reader.GetSafeString(startingIndex++);
                      singleUser.FirstName = reader.GetSafeString(startingIndex++);
                      singleUser.LastName = reader.GetSafeString(startingIndex++);
                      singleUser.url = reader.GetSafeString(startingIndex++);
                      singleUser.userRole = reader.GetSafeString(startingIndex++);
                      singleUser.Name = reader.GetSafeString(startingIndex++);
                      singleUser.PhoneNumber = reader.GetSafeString(startingIndex++);

                      if (companyEmployeeList == null)
                      {
                          companyEmployeeList = new List<CompanyEmployeeDomain>();
                      }
                      companyEmployeeList.Add(singleUser);
                  });
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return companyEmployeeList;
        }


        public List<CompanyEmployeeDomain> GetAllEmployees(int CompanyId)
        {
            List<CompanyEmployeeDomain> companyEmployeeList = null;
            try
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfile_GetAllByCompanyId"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@companyId", CompanyId);


                  }, map: delegate (IDataReader reader, short set)
                  {
                      var singleUser = new CompanyEmployeeDomain();

                      int startingIndex = 0; //startingOrdinal
                      
                      singleUser.UserId = reader.GetSafeString(startingIndex++);
                      singleUser.Email = reader.GetSafeString(startingIndex++);
                      singleUser.FirstName = reader.GetSafeString(startingIndex++);
                      singleUser.LastName = reader.GetSafeString(startingIndex++);
                      singleUser.url = reader.GetSafeString(startingIndex++);
                      singleUser.userRole = reader.GetSafeString(startingIndex++);
                      singleUser.Name = reader.GetSafeString(startingIndex++);
                      singleUser.PhoneNumber = reader.GetSafeString(startingIndex++);

                      if (companyEmployeeList == null)
                      {
                          companyEmployeeList = new List<CompanyEmployeeDomain>();
                      }
                      companyEmployeeList.Add(singleUser);
                  });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return companyEmployeeList;
        }




        //....// =============================================================================================

        public UserDomain ProfileGetByUserId(string userId)
        {

            UserDomain singleUser = new UserDomain();
            try
            {

                DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfile_GetByuserId"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@userId", userId);

                  }, map: delegate (IDataReader reader, short set)
                  {
                      int startingIndex = 0; //startingOrdinal

                      singleUser.UserId = reader.GetSafeString(startingIndex++);
                      singleUser.Email = reader.GetSafeString(startingIndex++);
                      singleUser.FirstName = reader.GetSafeString(startingIndex++);
                      singleUser.LastName = reader.GetSafeString(startingIndex++);
                      singleUser.CompanyName = reader.GetSafeString(startingIndex++);
                      singleUser.PhoneNumber = reader.GetSafeString(startingIndex++);
                      singleUser.url = reader.GetSafeString(startingIndex++);
                      singleUser.MediaId = reader.GetSafeInt32(startingIndex++);

                  });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return singleUser;
        }



        //....// =============================================================================================

        public UserDomain GetPhoneNumberByEmail(string email)
        {

            UserDomain singleUser = new UserDomain();
            try
            {
                
                DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfilePhoneNumb_GetByEmail"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@email", email);

                  }, map: delegate (IDataReader reader, short set)
                  {
                      int startingIndex = 0; //startingOrdinal

                      singleUser.Email = reader.GetSafeString(startingIndex++);
                      singleUser.PhoneNumber = reader.GetSafeString(startingIndex++);
                      singleUser.FirstName = reader.GetSafeString(startingIndex++);
                      singleUser.LastName = reader.GetSafeString(startingIndex++);
                   

                  });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return singleUser;
        }



        //....// =============================================================================================

        public bool DeleteProfile(string userId)
        {
            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserProfile_Delete"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@userId", userId);

                          success = true;
                      });

            return success;
        }





        //....// =============================================================================================

        public bool UpdateProfileMediaId(UserProfileMediaIdUpdateRequest model)
        {
            bool result = false;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserProfile_UpdateMediaId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@userId", model.UserId);
                    paramCollection.AddWithValue("@mediaId", model.MediaId);

                    result = true;
                });

            return result;
        }





        //....// =============================================================================================
        // add userRole for invite users

        public async Task<IdentityResult> AddRoleToUser(InsertNewUserRequest model)
        {
            // instantiate a new UserManager
            ApplicationUserManager userManager = UserService.GetUserManager();

            // return identityresult
            var result = await userManager.AddToRoleAsync(model.UserId, model.UserRole);
            return result;

        }

        //....// =============================================================================================

        public QuoteRequestDomain GetQuoteRequestByCompanyIdAndStatusId(int companyId)
        {
            QuoteRequestDomain quoteStatusbyCompany = null;

            try
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.QuoteRequests_GetCompanyId_Status"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@CompanyId", companyId);

            }, map: delegate (IDataReader reader, short set)
            {
                quoteStatusbyCompany = new QuoteRequestDomain();

                int startingIndex = 0; //startingOrdinal

                quoteStatusbyCompany.QrId = reader.GetSafeInt32(startingIndex++);
                quoteStatusbyCompany.Status = (QRState)reader.GetSafeInt32(startingIndex++);
                //quoteStatus.EventId = (QREvent)reader.GetSafeInt32(startingIndex++);
                quoteStatusbyCompany.QuoteRequestName = reader.GetSafeString(startingIndex++);

                // Passing through the values to the Domain Object
                quoteStatusbyCompany.StatusName = quoteStatusbyCompany.Status.ToString();
                
            });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return quoteStatusbyCompany;
        }

        

      

        public List<CompanyEmployeeDomain> GetAllEmployeesByCompanyId(EmployeeProfileRequest model, ref int TotalCount)
        {
            List<CompanyEmployeeDomain> companyEmployeeList = null;
            int totalCount = 0;
            try
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfile_GetByCompanyId"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@companyId", model.companyId);
                      // server side pagination
                      paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                      paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);

                  }, map: delegate (IDataReader reader, short set)
                  {
                      var singleUser = new CompanyEmployeeDomain();

                      int startingIndex = 0; //startingOrdinal

                      totalCount = reader.GetSafeInt32(startingIndex++);
                      singleUser.UserId = reader.GetSafeString(startingIndex++);
                      singleUser.Email = reader.GetSafeString(startingIndex++);
                      singleUser.FirstName = reader.GetSafeString(startingIndex++);
                      singleUser.LastName = reader.GetSafeString(startingIndex++);
                      singleUser.url = reader.GetSafeString(startingIndex++);
                      singleUser.userRole = reader.GetSafeString(startingIndex++);
                      singleUser.Name = reader.GetSafeString(startingIndex++);
                      singleUser.PhoneNumber = reader.GetSafeString(startingIndex++);

                      if (companyEmployeeList == null)
                      {
                          companyEmployeeList = new List<CompanyEmployeeDomain>();
                      }
                      companyEmployeeList.Add(singleUser);
                  });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return companyEmployeeList;
        }

        
    }

}