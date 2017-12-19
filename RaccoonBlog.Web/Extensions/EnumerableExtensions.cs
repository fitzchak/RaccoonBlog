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

		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			var rng = new Random();
			var n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}
}
