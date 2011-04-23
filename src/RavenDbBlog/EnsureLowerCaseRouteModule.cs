using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace RavenDbBlog
{
    /* http://stackoverflow.com/questions/696673/lower-case-urls-in-asp-net-mvc
     * Search engines treat URLs case-sensitively, meaning that if you have multiple links to the same content, that content's page ranking is distributed and hence diluted.
     * Returning HTTP 301 (Moved Permanently) for such links will cause search engines to 'merge' these links and hence only hold one reference to your content.
     */
    public class EnsureLowerCaseRouteModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += Application_BeginRequest;
        }

        public void Dispose()
        { }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication) sender;
            var request = application.Request;
            var response = application.Response;

            // Don't rewrite requests for content (.png, .css) or scripts (.js)
            if (request.Url.AbsolutePath.Contains("/Content/"))
                return;

            // If uppercase chars exist, redirect to a lowercase version
            var url = request.Url.ToString();
            if (Regex.IsMatch(url, @"[A-Z]"))
            {
                response.Clear();
                response.Status = "301 Moved Permanently";
                response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                response.AddHeader("Location", url.ToLower());
                response.End();
            }
        }
    }
}