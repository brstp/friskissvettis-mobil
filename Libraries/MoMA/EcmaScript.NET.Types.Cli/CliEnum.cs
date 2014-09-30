using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types.Cli
{
	[ComVisible(true)]
	public class CliEnum : CliObject
	{
		public override string ClassName
		{
			get
			{
				return "NativeCliEnum";
			}
		}
		public CliEnum(Enum enm)
		{
			base.Init(enm, enm.GetType());
		}
	}
}
