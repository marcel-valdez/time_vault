using System;
using System.IO;

namespace TimeVault
{
  public class EncryptedFile
  {
    private readonly Func<string> keyGetter;
    private readonly string file;

    public EncryptedFile(string file, Func<string> keyGetter)
    {
      this.keyGetter = keyGetter;
      this.file = file;
    }

    private string[] ReadRawData()
    {
      byte[] data = File.ReadAllBytes(file);
      byte[] decryptable = new byte[data.Length + (data.Length % 8)];
      Array.Copy(data, decryptable, data.Length);
      string decrypted = Encryption.BytesToString(Encryption.Decrypt(decryptable, keyGetter()));
      return decrypted.Split(new string[] { Environment.NewLine },
                             3,
                             StringSplitOptions.RemoveEmptyEntries);
    }

    public VaultData ReadData()
    {
      return new VaultData(ReadRawData());
    }

    private void WriteData(string password, double deadline, double waited)
    {
      string data = password + Environment.NewLine
                  + deadline + Environment.NewLine
                  + waited;
      byte[] encrypted = Encryption.Encrypt(Encryption.StringToBytes(data), keyGetter());
      File.WriteAllBytes(file, encrypted);
    }

    public void WriteData(VaultData data)
    {
      WriteData(data.Password, data.Deadline, data.Waited);
    }

    public void CopyFrom(EncryptedFile from)
    {
      File.WriteAllBytes(this.file, File.ReadAllBytes(from.file));
    }
  }
}
