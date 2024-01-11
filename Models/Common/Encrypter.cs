using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Encrypter
    {
        private readonly static IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
        private readonly static string _key = _config.GetValue<string>("Encripter:Key")!;
        private readonly static string _iv = _config.GetValue<string>("Encripter:IV")!;

        public static string Encrypt(string plainText)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                return StringEncrypter.AESStringEncrypt(_key, _iv, plainText);
            }
            else
            {
                return plainText;
            }
        }

        public static string DecryptEmpty(string encText)
        {
            if (!string.IsNullOrEmpty(encText))
            {
                try
                {
                    return StringEncrypter.AESStringDecrypt(_key, _iv, encText);
                }
                catch
                {
                    return string.Empty;
                }
            }
            else
            {
                return encText;
            }
        }

        public static string Decrypt(string encText)
        {
            if (!string.IsNullOrEmpty(encText))
            {
                return StringEncrypter.AESStringDecrypt(_key, _iv, encText);
            }
            else
            {
                return encText;
            }
        }

        public static string EncryptURLEncode(string plainText)
        {
            string encText = Encrypt(plainText);
            return HttpUtility.UrlEncode(encText);
        }

        public static string DecryptURLEncode(string urlEncText)
        {
            string encText = HttpUtility.UrlDecode(urlEncText);
            return Decrypt(encText);
        }

        public static string Encrypt_MD5(string text)
        {
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            using MD5CryptoServiceProvider x = new();
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            byte[] bs = Encoding.UTF8.GetBytes(text);
            bs = x.ComputeHash(bs);
            StringBuilder s = new();

            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
    }
}