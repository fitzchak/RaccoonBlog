using System;
using System.Security.Cryptography;
using System.Text;

namespace RaccoonBlog.Web.Models
{
	public class User
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public bool Enabled { get; set; }

		// User's settings
		public string TwitterNick { get; set; }
		public string RelatedTwitterNick { get; set; }
		public string RelatedTwitterNickDesc { get; set; }

		const string ConstantSalt = "xi07cevs01q4#";
		protected string HashedPassword { get; private set; }
		private string passwordSalt;
		private string PasswordSalt
		{
			get
			{
				return passwordSalt ?? (passwordSalt = Guid.NewGuid().ToString("N"));
			}
			set { passwordSalt = value; }
		}

		public User SetPassword(string pwd)
		{
			HashedPassword = GetHashedPassword(pwd);
			return this;
		}

		private string GetHashedPassword(string pwd)
		{
			using (var sha = SHA256.Create())
			{
				var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(PasswordSalt + pwd + ConstantSalt));
				return Convert.ToBase64String(computedHash);
			}
		}

		public bool ValidatePassword(string maybePwd)
		{
			if (HashedPassword == null)
				return true;
			return HashedPassword == GetHashedPassword(maybePwd);
		}
	}
}
