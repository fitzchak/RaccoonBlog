/*using System;
using System.Web.Routing;
using AttributeRouting.Web.Mvc;
using AttributeRouting.Web.Mvc.Framework;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Raven.Client;

namespace RaccoonBlog.Web.Helpers
{
	public static class RouteCollectionExtension
	{
		public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url)
		{
			return MapRouteLowerCase(routes, name, url, null /* defaults #1#, (object)null /* constraints #1#);
		}

		public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults)
		{
			return MapRouteLowerCase(routes, name, url, defaults, (object)null /* constraints #1#);
		}

		public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints)
		{
			return MapRouteLowerCase(routes, name, url, defaults, constraints, null /* namespaces #1#);
		}

		public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, string[] namespaces)
		{
			return MapRouteLowerCase(routes, name, url, null /* defaults #1#, null /* constraints #1#, namespaces);
		}

		public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
		{
			return MapRouteLowerCase(routes, name, url, defaults, null /* constraints #1#, namespaces);
		}

		public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
		{
			if (routes == null)
				throw new ArgumentNullException("routes");
			if (url == null)
				throw new ArgumentNullException("url");

			Route route = new AttributeRoute(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), null, new Constants.Configuration
			{
				UseLowercaseRoutes = true,
				PreserveCaseForUrlParameters = true,
			})
			{
				Defaults = new RouteValueDictionary(defaults),
				Constraints = new RouteValueDictionary(constraints),
				DataTokens = new RouteValueDictionary()
			};

			if ((namespaces != null) && (namespaces.Length > 0))
			{
				route.DataTokens["Namespaces"] = namespaces;
			}

			routes.Add(name, route);

			return route;
		}
	}
}*/