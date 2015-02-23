// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
#pragma warning disable 1591, 3008, 3009, 0108
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace RaccoonBlog.Web.Controllers
{
    public partial class SearchController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SearchController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected SearchController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SearchResult()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SearchResult);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SearchController Actions { get { return MVC.Search; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Search";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Search";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string SearchResult = "SearchResult";
            public readonly string GoogleCse = "GoogleCse";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string SearchResult = "SearchResult";
            public const string GoogleCse = "GoogleCse";
        }


        static readonly ActionParamsClass_SearchResult s_params_SearchResult = new ActionParamsClass_SearchResult();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SearchResult SearchResultParams { get { return s_params_SearchResult; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SearchResult
        {
            public readonly string q = "q";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string GoogleCse = "GoogleCse";
                public readonly string SearchResult = "SearchResult";
            }
            public readonly string GoogleCse = "~/Views/Search/GoogleCse.cshtml";
            public readonly string SearchResult = "~/Views/Search/SearchResult.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_SearchController : RaccoonBlog.Web.Controllers.SearchController
    {
        public T4MVC_SearchController() : base(Dummy.Instance) { }

        [NonAction]
        partial void SearchResultOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string q);

        [NonAction]
        public override System.Web.Mvc.ActionResult SearchResult(string q)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SearchResult);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "q", q);
            SearchResultOverride(callInfo, q);
            return callInfo;
        }

        [NonAction]
        partial void GoogleCseOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult GoogleCse()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GoogleCse);
            GoogleCseOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108
