using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaccoonBlog.Web.Helpers;
using Xunit;

namespace RaccoonBlog.IntegrationTests
{
    public class CryptographyUtilTests
    {
        private readonly CryptographyUtil _cryptography;

        public CryptographyUtilTests()
        {
            var salt = CryptographyUtil.GenerateRandomString(16);
            var iv = CryptographyUtil.GenerateIv();
            _cryptography = new CryptographyUtil(salt, iv);
        }

        [Fact]
        public void CanDecryptWhatItEncrypted()
        {
            const string text = "This |s s0me 73X7";
            const string password = "m4g1c_missile99";
            var base64EncryptedText = _cryptography.Encrypt(text, password);
            Assert.NotEqual(text, base64EncryptedText);

            var decrypted = _cryptography.Decrypt(base64EncryptedText, password);
            Assert.Equal(text, decrypted);
        } 
    }
}
