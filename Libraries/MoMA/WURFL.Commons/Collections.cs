using System;
using System.Collections.Generic;
namespace WURFL.Commons
{
	public static class Collections
	{
		public static T Find<T>(ICollection<T> collection, Predicate<T> match)
		{
			new List<T>(collection).Find(match);
			T result = default(T);
			foreach (T current in collection)
			{
				if (match(current))
				{
					result = current;
					break;
				}
			}
			return result;
		}
		public static ICollection<T> Select<T>(IEnumerable<T> userAgentsSet, Predicate<T> match)
		{
			IList<T> list = new List<T>();
			foreach (T current in userAgentsSet)
			{
				if (match(current))
				{
					list.Add(current);
				}
			}
			return list;
		}
		public static bool Exist<T>(IEnumerable<T> collections, Predicate<T> match)
		{
			bool result = false;
			foreach (T current in collections)
			{
				if (match(current))
				{
					result = true;
					break;
				}
			}
			return result;
		}
		internal static ICollection<T> AddAll<T>(ICollection<T> one, ICollection<T> two)
		{
			ICollection<T> collection = new List<T>(one);
			foreach (T current in two)
			{
				collection.Add(current);
			}
			return collection;
		}
	}
}
