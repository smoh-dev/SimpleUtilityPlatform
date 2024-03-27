using System.Security.Cryptography;
using System.Text;

namespace Sup.Common.Utils;

/// <summary>
/// A class that provides AES-based encryption and decryption functions.
/// </summary>
public class Encrypter
{
    private const int KeyLength = 16;
    private const char KeyPaddingChar = '^';
    private readonly byte[] _key;
    private readonly byte[] _iv = Encoding.UTF8.GetBytes("oaMNx6lRuCFVQYte");

    /// <summary>
    /// Initializes a new instance of the <see cref="Encrypter"/> class.
    /// </summary>
    /// <param name="key">16 byte string. If it is short, it is filled with a specific value; if it is long, it is truncated.</param>
    public Encrypter(string key)
    {
        if (key.Length < KeyLength)
            key = key.PadRight(KeyLength, KeyPaddingChar);
        _key = Encoding.UTF8.GetBytes(key.Substring(0, KeyLength));
    }
    /// <summary>
    /// Encrypts the specified plain text.
    /// </summary>
    /// <param name="plainText">stirng to be enctrypted.</param>
    /// <returns>Encrypted string.</returns>
    public string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    /// <summary>
    /// Decrypts the specified cipher text.
    /// </summary>
    /// <param name="cipheredText">string to be decrypted.</param>
    /// <returns>Decrypted string.</returns>
    public string Decrypt(string cipheredText)
    {
        byte[] cipheredByte = Convert.FromBase64String(cipheredText);
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipheredByte))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
