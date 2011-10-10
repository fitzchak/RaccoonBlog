using System.Security.Cryptography;
using System.Text;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class EmailHashResolver
	{
		public static string Resolve(string email)
		{
			if (email == null) return null;

			var str = email.Trim().ToLowerInvariant();
			return GetMd5Hash(str);
		}
		
		public static string GetMd5Hash(string input)
		{
			// Create a new Stringbuilder to collect the bytes  
			// and create a string.  
			var sBuilder = new StringBuilder();
	
			// Create a new instance of the MD5CryptoServiceProvider object.  
			using (var md5Hasher = MD5.Create())
			{
				// Convert the input string to a byte array and compute the hash.  
				var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

				// Loop through each byte of the hashed data  
				// and format each one as a hexadecimal string.  
				for (var i = 0; i < data.Length; i++)
				{
					sBuilder.Append(data[i].ToString("x2"));
				}
			}

			return sBuilder.ToString();  // Return the hexadecimal string.  
		}  
	}
}
