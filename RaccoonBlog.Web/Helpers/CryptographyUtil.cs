using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RaccoonBlog.Web.Helpers
{
    public class CryptographyUtil
    {
        private static int _iterations = 2;
        private static int _keySize = 256;

        private readonly string _salt; 
        private readonly string _vector;

        public CryptographyUtil(string salt, string iv)
        {
            _salt = salt;
            _vector = iv;
        }

        public string Encrypt(string value, string password)
        {
            return Encrypt<AesManaged>(value, password);
        }
        public string Encrypt<T>(string value, string password)
                where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);

            byte[] encrypted;
            using (T cipher = new T())
            {
                var keyBytes = GetKeyBytes<T>(password, saltBytes);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        private static byte[] GetKeyBytes<T>(string password, byte[] saltBytes) where T : SymmetricAlgorithm, new()
        {
            Rfc2898DeriveBytes passwordBytes =
                new Rfc2898DeriveBytes(password, saltBytes, _iterations);
            byte[] keyBytes = passwordBytes.GetBytes(_keySize/8);
            return keyBytes;
        }

        public string Decrypt(string value, string password)
        {
            return Decrypt<AesManaged>(value, password);
        }

        public string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T())
            {
                Rfc2898DeriveBytes passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, _iterations);
                byte[] keyBytes = passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream from = new MemoryStream(valueBytes))
                    {
                        using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                        {
                            decrypted = new byte[valueBytes.Length];
                            decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                        }
                    }
                }

                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        public static string GenerateRandomString(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GenerateKey()
        {
            var aes = new AesManaged();
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        public static string GenerateIv()
        {
            var aes = new AesManaged();
            aes.GenerateIV();
            return Encoding.ASCII.GetString(aes.IV);
        }

    }
}
