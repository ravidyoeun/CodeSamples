using Sabio.Web.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services.Interfaces
{
    public interface ITemplateEmailService
    {
        // Converts Razor View into a string 
        string RazorViewToString(string viewName, EmailRequest model);
    }
}