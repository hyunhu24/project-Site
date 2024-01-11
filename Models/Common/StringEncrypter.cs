using System.Security.Cryptography;
using System.Text;
using System.Collections;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    /// <summary>
    /// 문자열 암호화 / 복호화를 위한 개체입니다.
    /// </summary>
    public class StringEncrypter
    {
        // 유니코드 바이트 배열을 문자열로 변화t
        private static string ConvertByteArrayToString(byte[] b)
        {
            return new UnicodeEncoding().GetString(b, 0, b.Length);
        }

        // 문자열을 안시 바이트 배열로 변환
        private static byte[] ConvertStringToByteArrayA(string s)
        {
            return new ASCIIEncoding().GetBytes(s);
        }

        // 안시 바이트 배열을 문자열로 변화
        private static string ConvertByteArrayToStringA(byte[] b)
        {
            return new ASCIIEncoding().GetString(b, 0, b.Length);
        }

        // 문자열을 Base64 배열로 변환
        private static byte[] ConvertStringToByteArrayB(string s)
        {
            return Convert.FromBase64String(s);
        }

        // Base64 배열을 문자열로 변화
        private static string ConvertByteArrayToStringB(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        // Byte[] 형을 Hex 값으로 반환
        public static string ToHex(byte[] b)
        {
            string result = "";
            foreach (byte ch in b)
            {
                result += string.Format("{0:x2}", ch);
            }
            return result;
        }

        public static string ToHex(byte b)
        {
            return string.Format("{0:x2}", b);
        }

        public static byte[] ToByte16(string str)
        {
            ArrayList rows = new();
            try
            {
                int k = 0;
                while (k * 2 + 2 <= str.Length)
                {
                    string ch = str.Substring(k * 2, 2);
                    k++;
                    byte b = Convert.ToByte(ch, 16);
                    rows.Add(b);
                }
            }
            finally
            {

            }
            return (byte[])rows.ToArray(typeof(byte));
        }

        public static string DESKeyCreate()
        {
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            DESCryptoServiceProvider des = new();
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            des.GenerateKey();
            return ToHex(des.Key);
        }

        public static string AESKeyCreate(out string sIV)
        {
            Aes aesAlg = Aes.Create();
            // aesAlg.GenerateKey();
            //aesAlg.GenerateIV();
            sIV = ToHex(aesAlg.IV);
            return ToHex(aesAlg.Key);
        }

        public static string AESIVCreate(out string sKey)
        {
            Aes aesAlg = Aes.Create();
            //aesAlg.GenerateIV();
            sKey = ToHex(aesAlg.Key);
            return ToHex(aesAlg.IV);
        }

        public static string AESCipherKeyCreate(out string sIV)
        {
#pragma warning disable SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            RijndaelManaged rijndael = new()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
#pragma warning restore SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            rijndael.GenerateIV();
            rijndael.GenerateKey();
            sIV = ToHex(rijndael.IV);
            return ToHex(rijndael.Key);
        }

        // 복호화
        public static string DESDecrypt(string key, string str)
        {
            byte[] btKey = key.Length == 8 ? ConvertStringToByteArrayA(key) : ToByte16(key);
            byte[] btEncData = ConvertStringToByteArrayB(str);

            return DESDecrypt(btKey, btEncData);
        }

        public static string DESDecrypt(byte[] btKey, string str)
        {
            byte[] btEncData = ConvertStringToByteArrayB(str);
            return DESDecrypt(btKey, btEncData);
        }

        public static string DESDecrypt(byte[] btKey, byte[] btEncData)
        {
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            DESCryptoServiceProvider des = new()
            {
                Key = btKey,
                IV = btKey
            };
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.

            ICryptoTransform desencrypt = des.CreateDecryptor();
            MemoryStream ms = new();
            CryptoStream cs = new(ms, desencrypt, CryptoStreamMode.Write);

            cs.Write(btEncData, 0, btEncData.Length);
            cs.FlushFinalBlock();
            cs.Close();

            byte[] btSrc = ms.ToArray();

            return ConvertByteArrayToStringA(btSrc);
        }

        // 암호화
        public static string DESEncrypt(string key, string str)
        {
            byte[] btKey = key.Length == 8 ? ConvertStringToByteArrayA(key) : ToByte16(key);
            byte[] btSrc = ConvertStringToByteArrayA(str);

            return DESEncrypt(btKey, btSrc);
        }

        public static string DESEncrypt(byte[] btKey, string str)
        {
            byte[] btSrc = ConvertStringToByteArrayA(str);
            return DESEncrypt(btKey, btSrc);
        }

        public static string DESEncrypt(byte[] btKey, byte[] btSrc)
        {
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            DESCryptoServiceProvider des = new()
            {
                Key = btKey,
                IV = btKey
            };
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.

            ICryptoTransform desencrypt = des.CreateEncryptor();
            MemoryStream ms = new();
            CryptoStream cs = new(ms, desencrypt, CryptoStreamMode.Write);

            cs.Write(btSrc, 0, btSrc.Length);
            cs.FlushFinalBlock();
            cs.Close();

            byte[] btEncData = ms.ToArray();

            return ConvertByteArrayToStringB(btEncData);
        }

        // AES 암호화
        public static string AESEncrypt(string key, string IV, string str)
        {
            byte[] btKey = ConvertStringToByteArrayA(key);
            byte[] btIV = ConvertStringToByteArrayA(IV);

            return AESEncrypt(btKey, btIV, str);
        }

        public static string AESEncrypt(byte[] btKey, byte[] btIV, string str)
        {
            Aes? aesAlg = null;

            MemoryStream ms = new();
            CryptoStream? cs = null;
            StreamWriter? sw = null;

            try
            {
                aesAlg = Aes.Create();
                aesAlg.Key = btKey;
                aesAlg.IV = btIV;

                ICryptoTransform encrypter = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                cs = new CryptoStream(ms, encrypter, CryptoStreamMode.Write);
                sw = new StreamWriter(cs);

                sw.Write(str);
            }
            finally
            {
                if (sw != null)
                    sw.Close();

                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return ConvertByteArrayToString(ms.ToArray());
        }

        // AES 복호화
        public static string AESDecrypt(string key, string IV, string str)
        {
            byte[] btKey = ConvertStringToByteArrayA(key);
            byte[] btIV = ConvertStringToByteArrayA(IV);
            return AESDecrypt(btKey, btIV, str);
        }

        public static string AESDecrypt(byte[] btKey, byte[] btIV, string str)
        {
            byte[] btstr = ConvertStringToByteArrayA(str);

            //byte[] encrypted = null;
            Aes? aesAlg = null;
            MemoryStream? ms = null;
            CryptoStream? cs = null;
            StreamReader? sr = null;

            try
            {
                aesAlg = Aes.Create();
                aesAlg.Key = btKey;
                aesAlg.IV = btIV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                ms = new MemoryStream(btstr);
                cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                sr = new StreamReader(cs);

                string mReturn = sr.ReadToEnd();
            }
            finally
            {
                if (sr != null)
                    sr.Close();

                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return ConvertByteArrayToString(ms.ToArray());
        }

        // AES Rij 암호화
        public static string AESCipherEncrypt(string key, string iv, string str)
        {
#pragma warning disable SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            RijndaelManaged rijndael = new()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = ToByte16(key),
                IV = ToByte16(iv)
            };
#pragma warning restore SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            str ??= "";

            byte[]? cipherBytes;
            try
            {
                cipherBytes = Array.Empty<byte>();
                ICryptoTransform? transform = rijndael.CreateEncryptor();
                UTF8Encoding utf8Encoding = new();
                byte[] plainText = utf8Encoding.GetBytes(str);
                cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch
            {
                //System.Console.WriteLine(e.StackTrace);
                ArgumentException argumentException = new("text is not a valid string!(Encrypt)", "text");
                throw argumentException;
            }
            finally
            {
                //if (this.rijndael != null)      
                //this.rijndael.Clear();
            }

            return Convert.ToBase64String(cipherBytes);
        }

        // AES Rij 복호화
        public static string AESCipherDecrypt(string key, string iv, string str)
        {
#pragma warning disable SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            RijndaelManaged rijndael = new()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = ToByte16(key),
                IV = ToByte16(iv)
            };
#pragma warning restore SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            byte[]? plainText;
            try
            {
                plainText = Array.Empty<byte>();
                ICryptoTransform? transform = rijndael.CreateDecryptor();
                byte[] encryptedValue = Convert.FromBase64String(str);
                plainText = transform.TransformFinalBlock(encryptedValue, 0, encryptedValue.Length);
            }
            catch
            {
                //System.Console.WriteLine(e.StackTrace);
                throw new ArgumentException("text is not a valid string!(Decrypt)", "text");
            }
            finally
            {
                //if (this.rijndael != null)
                //this.rijndael.Clear();
            }

            UTF8Encoding utf8Encoding = new();
            return utf8Encoding.GetString(plainText);
        }

        // AES String 암호화
        public static string AESStringEncrypt(string key, string iv, string str)
        {
            if (key == null || key == "")
                throw new ArgumentException("The key can not be null or an empty string.", "key");

            if (iv == null || iv == "")
                throw new ArgumentException("The initial vector can not be null or an empty string.", "initialVector");
#pragma warning disable SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            RijndaelManaged rijndael = new()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
#pragma warning restore SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.

            // Initialize an encryption key and an initial vector.
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            MD5 md5 = new MD5CryptoServiceProvider();
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.

            UTF8Encoding utf8Encoding = new();
            rijndael.Key = md5.ComputeHash(utf8Encoding.GetBytes(key));
            rijndael.IV = md5.ComputeHash(utf8Encoding.GetBytes(iv));

            str ??= "";

            // Get an encryptor interface.
            ICryptoTransform transform = rijndael.CreateEncryptor();

            // Get a UTF-8 byte array from a unicode string.
            byte[] utf8Value = utf8Encoding.GetBytes(str);

            // Encrypt the UTF-8 byte array.
            byte[] encryptedValue = transform.TransformFinalBlock(utf8Value, 0, utf8Value.Length);

            // Return a base64 encoded string of the encrypted byte array.
            return Convert.ToBase64String(encryptedValue);
        }

        // AES String 복호화
        public static string AESStringDecrypt(string key, string iv, string str)
        {
            if (key == null || key == "")
                throw new ArgumentException("The key can not be null or an empty string.", "key");

            if (iv == null || iv == "")
                throw new ArgumentException("The initial vector can not be null or an empty string.", "initialVector");

            UTF8Encoding utf8Encoding = new();
#pragma warning disable SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.
            RijndaelManaged rijndael = new()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
#pragma warning restore SYSLIB0022 // 형식 또는 멤버는 사용되지 않습니다.

            // Initialize an encryption key and an initial vector.
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            MD5 md5 = new MD5CryptoServiceProvider();
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            rijndael.Key = md5.ComputeHash(utf8Encoding.GetBytes(key));
            rijndael.IV = md5.ComputeHash(utf8Encoding.GetBytes(iv));

            if (str == null || str == "")
                throw new ArgumentException("The cipher string can not be null or an empty string.");

            // Get an decryptor interface.
            ICryptoTransform transform = rijndael.CreateDecryptor();

            // Get an encrypted byte array from a base64 encoded string.
            byte[] encryptedValue = Convert.FromBase64String(str);

            // Decrypt the byte array.

            byte[] decryptedValue = transform.TransformFinalBlock(encryptedValue, 0, encryptedValue.Length);

            // Return a string converted from the UTF-8 byte array.
            return utf8Encoding.GetString(decryptedValue);
        }

        public static string TripleDESEncrypt(string input, string key)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(input);
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            TripleDESCryptoServiceProvider tripleDES = new()
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string TripleDESDecrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
#pragma warning disable SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            TripleDESCryptoServiceProvider tripleDES = new()
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
#pragma warning restore SYSLIB0021 // 형식 또는 멤버는 사용되지 않습니다.
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}