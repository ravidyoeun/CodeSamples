using quotemule.Web.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quotemule.Web.Services.Interfaces
{
    public interface ITemplateEmailService
    {
        // Converts Razor View into a string 
        string RazorViewToString(string viewName, EmailRequest model);
    }
}