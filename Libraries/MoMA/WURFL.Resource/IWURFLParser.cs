using System;
namespace WURFL.Resource
{
	public interface IWURFLParser
	{
		ResourceData Parse(IWURFLResource wurflResource);
		ResourceData Parse(IWURFLResource wurflResource, IWURFLResource[] patchResources);
	}
}
