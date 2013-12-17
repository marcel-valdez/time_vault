using System;
using NUnit.Framework;

namespace TimeVault.Test
{

  [TestFixture]
  public class EncryptionTest
  {
    private readonly static Random rand = new Random(DateTime.Now.Millisecond + System.Threading.Thread.CurrentThread.ManagedThreadId);

    [Test]
    public void TestUnicodeEncoding()
    {
      // Arrange
      const string expected = "abc";
      // Act
      string actual = Encryption.BytesToString(Encryption.StringToBytes(Encryption.BytesToString(Encryption.StringToBytes(expected))));
      // Assert
      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestBase64Encoding()
    {
      // Arrange
      string plain = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-=!@#$%^&*()_+|\\";
      byte[] data = Encryption.StringToBytes(plain);
      // Act
      byte[] base64 = Encryption.ToBase64Bytes(data);
      byte[] actual = Encryption.FromBase64Bytes(base64);
      string actualStr = Encryption.BytesToString(actual);
      // Assert
      Assert.AreEqual(plain, actualStr);
    }

    [Test]
    public void TestEncryptionDecryption()
    {
      int attempts = 0;
      while (++attempts < 1000)
      {
        // Arrange
        String key = GenerateStr(32);
        string plain = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-=!@#$%^&*()_+|\\";

        // Act
        string encrypted = Encryption.Encrypt(plain, key);
        string unencrypted = Encryption.Decrypt(encrypted, key);

        // Assert
        if (plain.Trim() == unencrypted.Trim())
        {
          Console.WriteLine("Success with key: " + key + " on iteration " + attempts);
          break;
        }
      }

      if (attempts == 1000)
      {
        Assert.Fail("Could not find a key that encrypts/decrypts correctly, there is an error somewhere.");
      }
    }

    private static string GenerateStr(int length)
    {
      string result = "";
      String allowed = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+-={[]}\\|;:'\",<.>/?`~¶²¨ª§¼¾À½ÂÄÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞÞßàáâãäåæçéêëìííĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŴŵŶŷŸŸŹźŻżŽžſƒƹǦǧǺǻǼǽǾǿȘșȷʔʕʼʽʾʿˀˁˆˇ˒˓˘˙˚˛˜";
      for (int i = 0; i < length; i++)
      {
        result += "" + allowed[rand.Next(0, allowed.Length)];
      }

      return result;
    }
  }
}
