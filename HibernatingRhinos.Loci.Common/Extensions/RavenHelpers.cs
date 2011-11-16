using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HibernatingRhinos.Loci.Common.Extensions
{
	public static class RavenHelpers
	{
		public static int ToIntId(this string id)
		{
			return int.Parse(id.Substring(id.LastIndexOf('/') + 1));
		}
	}
}
