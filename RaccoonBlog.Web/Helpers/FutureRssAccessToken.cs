using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Helpers
{
    public class FutureRssAccessToken
    {
        public DateTime ExpiresOn { get; set; }

        public int NumberOfDays { get; set; }

        public string User { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresOn;

        public FutureRssAccessToken(DateTime expiresOn, int numberOfDays, string user)
        {
            ExpiresOn = expiresOn;
            NumberOfDays = numberOfDays;
            User = user;
        }

        public string GetToken(BlogConfig blogConfig)
        {
            return GetCrypto(blogConfig).Encrypt(JsonConvert.SerializeObject(this), blogConfig.FuturePostsEncryptionKey);
        }

        public static FutureRssAccessToken Parse(string token, BlogConfig blogConfig)
        {
            var json = GetCrypto(blogConfig).Decrypt(token, blogConfig.FuturePostsEncryptionKey);
            return JsonConvert.DeserializeObject<FutureRssAccessToken>(json);
        }

        private static CryptographyUtil GetCrypto(BlogConfig blogConfig)
        {
            return new CryptographyUtil(blogConfig.FuturePostsEncryptionSalt, blogConfig.FuturePostsEncryptionIv);
        }
    }
}