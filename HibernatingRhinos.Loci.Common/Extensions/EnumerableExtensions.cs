using System;
using System.Collections.Generic;

namespace HibernatingRhinos.Loci.Common.Extensions
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}
	}
}
