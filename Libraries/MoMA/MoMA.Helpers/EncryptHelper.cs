using System;
using System.Security.Cryptography;
using System.Text;
namespace MoMA.Helpers
{
	public class EncryptHelper
	{
		private static string strKey = "%¤/asd!1";
		public static string Encrypt(string strToEncrypt)
		{
			string result;
			try
			{
				TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				string s = EncryptHelper.strKey;
				byte[] key = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(s));
				tripleDESCryptoServiceProvider.Key = key;
				tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
				byte[] bytes = Encoding.ASCII.GetBytes(strToEncrypt);
				result = Convert.ToBase64String(tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length));
			}
			catch (Exception ex)
			{
				result = "Wrong Input. " + ex.Message;
			}
			return result;
		}
		public static string Decrypt(string strEncrypted)
		{
			string result;
			try
			{
				TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				string s = EncryptHelper.strKey;
				byte[] key = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(s));
				tripleDESCryptoServiceProvider.Key = key;
				tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
				byte[] array = Convert.FromBase64String(strEncrypted);
				string @string = Encoding.ASCII.GetString(tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length));
				result = @string;
			}
			catch (Exception ex)
			{
				result = "Wrong Input. " + ex.Message;
			}
			return result;
		}
		public static string GetMd5Sum(string str)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = Encoding.UTF8.GetBytes(str);
			array = mD5CryptoServiceProvider.ComputeHash(array);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				byte b = array2[i];
				stringBuilder.Append(b.ToString("x2").ToLower());
			}
			return stringBuilder.ToString();
		}
	}
}
