using System;
namespace MoMA.Mobile.Barcode
{
	public class Code
	{
		private string _code = "";
		public Code(string template)
		{
			this._code = this.GenerateCode(template);
		}
		public string GenerateCode(string template)
		{
			char[] array = new char[template.Length];
			char[] array2 = "ABCDEFGHJKLMNOPQRSTUVWXYZ23456789".ToCharArray();
			char[] array3 = "ABCDEFGHJKLMNOPQRSTUVWXYZ".ToCharArray();
			char[] array4 = "23456789".ToCharArray();
			Random random = new Random();
			for (int i = 0; i < template.Length; i++)
			{
				char c = template[i];
				if (c != 'A')
				{
					if (c != 'N')
					{
						if (c == 'X')
						{
							array[i] = array2[random.Next(0, array2.Length)];
						}
						else
						{
							array[i] = template.ToCharArray()[i];
						}
					}
					else
					{
						array[i] = array4[random.Next(0, array4.Length)];
					}
				}
				else
				{
					array[i] = array3[random.Next(0, array3.Length)];
				}
			}
			this._code = new string(array);
			return this._code;
		}
		public override string ToString()
		{
			return this._code;
		}
	}
}
