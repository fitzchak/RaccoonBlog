using System;
using System.Security.Cryptography;
using System.Text;

namespace RaccoonBlog.Web.Models
{
	public class User
	{
		const string ConstantSalt = "xi07cevs01q4#";
		public string Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		protected string HashedPassword { get; private set; }
		public bool Enabled { get; set; }

		public void SetPassword(string pwd)
		{
			HashedPassword = GetHashedPassword(pwd);
		}

		private string GetHashedPassword(string pwd)
		{
			string hashedPassword;
			using (var sha = SHA256.Create())
			{
				var saltPerUser = Id;
				var computedHash = sha.ComputeHash(
					Encoding.Unicode.GetBytes(saltPerUser + pwd + ConstantSalt)
					);

				hashedPassword = Convert.ToBase64String(computedHash);
			}
			return hashedPassword;
		}

		public bool ValidatePassword(string maybePwd)
		{
			return HashedPassword == GetHashedPassword(maybePwd);
		}
	}
}