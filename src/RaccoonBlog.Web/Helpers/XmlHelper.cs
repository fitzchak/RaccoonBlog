// -----------------------------------------------------------------------
//  <copyright file="XmlHelper.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Xml;

namespace RaccoonBlog.Web.Helpers
{
	public static class XmlHelper
	{
		public static string Attribute(string unescaped)
		{
			var doc = new XmlDocument();
			var node = doc.CreateAttribute("foo");
			node.InnerText = unescaped;
			return node.InnerXml;
		}

		public static string Content(string unescaped)
		{
			var doc = new XmlDocument();
			var node = doc.CreateElement("foo");
			node.InnerText = unescaped;
			return node.InnerXml;
		}
	}
}