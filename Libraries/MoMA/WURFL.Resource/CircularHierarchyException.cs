using System;
using System.Collections.Generic;
namespace WURFL.Resource
{
	public class CircularHierarchyException : Exception
	{
		public CircularHierarchyException(List<string> hierarchy) : base(string.Format("Circular Hierarchy :  [{0}]", string.Join(",", hierarchy.ToArray())))
		{
		}
	}
}
