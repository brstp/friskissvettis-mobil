using MoMA.Mobile.Enum;
using System;
namespace MoMA.Mobile
{
	public class MobileNumber
	{
		private string _mobileNumber = "";
		private Country _country = Country.Sweden;
		public MobileNumber(string mobileNumber, Country country)
		{
			this._mobileNumber = mobileNumber;
			this._country = country;
		}
		public string GetCorrentMobileNumber()
		{
			Country country = this._country;
			if (country == Country.Bahrain)
			{
				return MobileNumber.Bahrain(this._mobileNumber);
			}
			if (country != Country.Sweden)
			{
				return "";
			}
			return MobileNumber.Sweden(this._mobileNumber);
		}
		public bool IsCorrentMobileNumber()
		{
			Country country = this._country;
			if (country != Country.Bahrain)
			{
				return country == Country.Sweden && this._mobileNumber == MobileNumber.Sweden(this._mobileNumber);
			}
			return this._mobileNumber == MobileNumber.Bahrain(this._mobileNumber);
		}
		private static string Bahrain(string mobileNumber)
		{
			if (mobileNumber.Length == 13 && mobileNumber.StartsWith("00973"))
			{
				return mobileNumber;
			}
			if (mobileNumber.Length == 8)
			{
				return "00973" + mobileNumber;
			}
			return "";
		}
		private static string Sweden(string mobileNumber)
		{
			mobileNumber = mobileNumber.Replace(" ", "").Replace("-", "").Replace("/", "");
			while (!mobileNumber.StartsWith("7") && !string.IsNullOrEmpty(mobileNumber))
			{
				mobileNumber = mobileNumber.Remove(0, 1);
			}
			if (mobileNumber.Length != 9)
			{
				return "";
			}
			return "0046" + mobileNumber;
		}
	}
}
