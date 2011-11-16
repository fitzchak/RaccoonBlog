using System;
using System.Linq;
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

        // User's settings
        public string TwitterNick { get; set; }
        public string RelatedTwitterNick { get; set; }
        public string RelatedTwitterNickDesc { get; set; }

		private Guid passwordSalt;
		private Guid PasswordSalt
		{
			get
			{
				if (passwordSalt == Guid.Empty)
					passwordSalt = Guid.NewGuid();
				return passwordSalt;
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
			string hashedPassword;
			using (var sha = SHA256.Create())
			{
				var computedHash = sha.ComputeHash(
					PasswordSalt.ToByteArray().Concat(
						Encoding.Unicode.GetBytes(PasswordSalt + pwd + ConstantSalt)
						).ToArray()
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
