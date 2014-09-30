using System;
namespace Mobi.Mtld.DA.Exception
{
	public class JsonException : Exception
	{
		public enum ErrorCode
		{
			BAD_DATA = 100,
			JSON_VERSION = 200,
			FILE_NOT_FOUND_ERROR = 300
		}
		public JsonException.ErrorCode Code
		{
			get;
			set;
		}
		public JsonException(string message, JsonException.ErrorCode code) : base(message)
		{
			this.Code = code;
		}
	}
}
