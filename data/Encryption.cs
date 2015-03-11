using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TimeVault
{

  public class Encryption
  {
    private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

    public static string Encrypt(string data, string keyStr) {
      return BytesToString(Encrypt(StringToBytes(data), keyStr));
    }

    public static byte[] Encrypt(byte[] data, string keyStr)
    {
      byte[] key;
      byte[] iv;
      byte[] encryptedUnicodeBytes;

      using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(keyStr, SALT, 4))
      using (AesManaged aes = new AesManaged())
      {
        key = pdb.GetBytes(32);
        iv = pdb.GetBytes(16);
        aes.Padding = PaddingMode.Zeros;
        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        encryptedUnicodeBytes = Transform(data, encryptor);
        aes.Clear();
        pdb.Reset();
      }

      return encryptedUnicodeBytes;
    }

    public static string Decrypt(string dataString, string keyStr)
    {
      byte[] encryptedUnicodeBytes = StringToBytes(dataString);
      return BytesToString(Decrypt(encryptedUnicodeBytes, keyStr));
    }

    public static byte[] Decrypt(byte[] encrypted, string keyStr) {
      byte[] key;
      byte[] iv;
      byte[] plainUnicodeBytes;

      using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(keyStr, SALT, 4))
      using (AesManaged aes = new AesManaged())
      {
        key = pdb.GetBytes(32);
        iv = pdb.GetBytes(16);
        aes.Padding = PaddingMode.Zeros;
        byte[] paddedUnicodeBytes = Transform(encrypted, aes.CreateDecryptor(key, iv));
        int removed = 0;
        for (int i = paddedUnicodeBytes.Length - 1; i >= 0; i-=2)
        {
          if (paddedUnicodeBytes[i] == 0 && paddedUnicodeBytes[i - 1] == 0)
            removed += 2;
          else
            break;
        }

        plainUnicodeBytes = new byte[paddedUnicodeBytes.Length - removed];
        Array.Copy(paddedUnicodeBytes, plainUnicodeBytes, plainUnicodeBytes.Length);
        aes.Clear();
        pdb.Reset();
      }

      return plainUnicodeBytes;
    }

    private static byte[] Transform(byte[] data, ICryptoTransform transform)
    {
      byte[] transformedBytes = null;
      using (MemoryStream dataStream = new MemoryStream())
      using (CryptoStream encryptionStream = new CryptoStream(dataStream, transform, CryptoStreamMode.Write))
      {
        encryptionStream.Write(data, 0, data.Length);
        encryptionStream.FlushFinalBlock();
        dataStream.Position = 0;

        transformedBytes = new byte[dataStream.Length];
        dataStream.Read(transformedBytes, 0, transformedBytes.Length);
      }

      return transformedBytes;
    }

    public static byte[] ToBase64Bytes(byte[] dataBytes)
    {
      string base64str = Convert.ToBase64String(dataBytes);
      return Encoding.ASCII.GetBytes(base64str);
    }

    public static byte[] FromBase64Bytes(byte[] dataBytes)
    {
      string base64str = Encoding.ASCII.GetString(dataBytes);
      return Convert.FromBase64String(base64str);
    }

    public static byte[] StringToBytes(string data)
    {
      return Encoding.Unicode.GetBytes(data);
    }

    public static string BytesToString(byte[] data)
    {
      return Encoding.Unicode.GetString(data, 0, data.Length);
    }
  }
}