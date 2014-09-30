using System;
using System.IO;
namespace WURFL.Resource
{
	public interface IWURFLResource
	{
		Stream GetStream();
	}
}
