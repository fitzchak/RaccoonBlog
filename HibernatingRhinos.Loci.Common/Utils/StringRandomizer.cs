using System;

namespace HibernatingRhinos.Loci.Common.Utils
{
	public static class StringRandomizer
	{
		private static Random _randomGen;
		private static Random RandomGen { get { return _randomGen ?? (_randomGen = new Random()); } }

		/// <summary>
		/// gets a random string
		/// </summary>
		/// <param name="length"></param>
		/// <param name="includeSpecialCharacters"></param>
		/// <param name="includeNumbers"></param>
		/// <returns></returns>
		public static string GetRandomString(int length, bool includeSpecialCharacters, bool includeNumbers)
		{
			var returnval = "";
			for (var i = 1; i <= length; i++)
			{
				returnval += GetRandomChar(includeSpecialCharacters, includeNumbers).ToString();
			}
			return returnval;
		}

		/// <summary>
		/// Gets a Random Character
		/// </summary>
		/// <param name="includeSpecialCharacters"></param>
		/// <param name="includeNumbers"></param>
		/// <returns></returns>
		public static char GetRandomChar(bool includeSpecialCharacters, bool includeNumbers)
		{
			//33-47 , 58-64, 91-96, 123-126 special characters
			//48-57 numbers
			//65-90 uppercase letters
			//97-122 lowercase letters

			var getNewChar = true;
			var randint = 0;
			if (includeNumbers && includeSpecialCharacters)
			{
				randint = RandomGen.Next(33, 122);
			}
			else if (includeNumbers)
			{
				//get a number between 48 and 122 but not between 58 and 64
				while (getNewChar)
				{
					randint = RandomGen.Next(48, 122);
					getNewChar = (randint >= 58 && randint <= 64) || (randint >= 91 && randint <= 96);
				}
			}
			else if (includeSpecialCharacters)
			{
				while (getNewChar)
				{
					randint = RandomGen.Next(33, 122);
					getNewChar = (randint >= 48 && randint <= 57);
				}
			}
			else
			{
				while (getNewChar)
				{
					randint = RandomGen.Next(65, 122);
					getNewChar = (randint >= 91 && randint <= 96);
				}
			}
			return Convert.ToChar(char.ConvertFromUtf32(randint));
		}
	}
}
