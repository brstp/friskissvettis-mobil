using System;
using System.Collections.Generic;
namespace WURFL.Resource
{
	public class OrphanHierarchyException : Exception
	{
		public OrphanHierarchyException(List<string> hierarchy) : base(string.Format("Orphan Hierarchy :  [{0}]", string.Join(",", hierarchy.ToArray())))
		{
		}
	}
}
