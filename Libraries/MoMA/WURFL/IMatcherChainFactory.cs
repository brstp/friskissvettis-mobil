using System;
using System.Collections.Generic;
using WURFL.Resource;
namespace WURFL
{
	public interface IMatcherChainFactory
	{
		IMatchersChain Create(ICollection<IModelDevice> devices);
	}
}
