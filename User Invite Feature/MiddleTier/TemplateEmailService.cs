using RazorEngine;
using RazorEngine.Templating;
using Sabio.Web.Models;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.ViewModels;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Sabio.Web.Services
{
    public class TemplateEmailService : ITemplateEmailService
    {
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // TemplateEmailService to convert HTML Pages to String Developed by Ravid Yoeun
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //RenderRazorViewToString accepts two params, the directory of the html page, and the request model to be display user info.
        public string RazorViewToString(string viewName, EmailRequest model)
        {
            // RazorEngine code that converts all html code to string 
            // Caution - Please make sure the razor HTML template you reference has inlined-css if being styled or it will not show
            var path = HostingEnvironment.MapPath(viewName);
            var viewRaw = File.ReadAllText(path);

            var key = new NameOnlyTemplateKey("EmailReply", ResolveType.Global, null);
            Engine.Razor.AddTemplate(key, new LoadedTemplateSource(viewRaw));
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
                Engine.Razor.RunCompile(key, sw, null, model);
            {
                return sb.ToString();
            }

        }
    }
}