using System;
using System.Linq;
using System.Globalization;

namespace TimeVault
{
  class Program
  {
    private const int SUCCESS = 0;
    private const int ERROR = 1;
    private const string INPUT_FORMAT = "dd/MM/yyyy HH:mm";

    public static int Main(string[] args)
    {
      if (args.Length == 0)
      {
        PrintUsage();
        return ERROR;
      }

      if (args.Length == 1)
      {
        if (args.Contains("-r"))
        {
          return ProcessContinueWait(args);
        }
        else
        {
          PrintUsage();
          return ERROR;
        }
      }

      if (args.Contains("-i"))
      {
        return ProcessIncreaseWait(args);
      }
      else
      {
        return ProcessNewWait(args);
      }
    }

    private static int ProcessIncreaseWait(string[] args)
    {
      if (args.Length < 3)
      {
        PrintUsage();
        return ERROR;
      }

      int number = 0;
      if (!int.TryParse(args[1], out number))
      {
        PrintUsage();
        return ERROR;
      }

      TimeSpan increment;
      switch (args[2])
      {
        case "d":
          increment = TimeSpan.FromDays(Math.Abs(number));
          break;
        case "h":
          increment = TimeSpan.FromHours(Math.Abs(number));
          break;
        case "m":
          increment = TimeSpan.FromMinutes(Math.Abs(number));
          break;
        default:
          PrintUsage();
          return ERROR;
      }

      DeadlineWaiter waiter = new DeadlineWaiter(GetKey, RetrieveDecrypted);
      waiter.Increment(increment);
      waiter.Wait();

      return SUCCESS;
    }

    private static int ProcessNewWait(string[] args)
    {
      DateTime deadline;
      if (!DateTime.TryParseExact(args[1], INPUT_FORMAT, null, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out deadline))
      {
        Console.WriteLine("Time format is wrong. Ranges: \"1/1/0001 00:00\" - \"31/12/9999 23:59\"");
        return ERROR;
      }

      Console.WriteLine("You will have to wait (at least) until: " + deadline);
      if (!args.Contains("-n"))
      {
        ConfirmEncryption(args[0]);
      }

      PersistEncrypted(args[0]);
      var waiter = (new DeadlineWaiter(deadline, GetKey, RetrieveDecrypted));
      waiter.Wait();

      return SUCCESS;
    }

    private static int ProcessContinueWait(string[] args)
    {
      if (args.Contains("-r"))
      {
        new DeadlineWaiter(GetKey, RetrieveDecrypted).Wait();
        return SUCCESS;
      }
      else
      {
        PrintUsage();
        return ERROR;
      }
    }

    private static string GetKey()
    {
      return KeyGetter.GetKey().Substring(0, 16);
    }

    private static void ConfirmEncryption(string plain)
    {
      byte[] encrypted = Encryption.Encrypt(Encryption.StringToBytes(plain), GetKey());
      string decrypted = Encryption.BytesToString(Encryption.Decrypt(encrypted, GetKey()));
      Console.Write("Is <{0}> the text you inserted? (y/n, default: y) ", decrypted);
      string answer = Console.ReadLine();
      if (answer.ToLower() == "n")
      {
        Console.WriteLine("Please try again...");
        Environment.Exit(ERROR);
      }
    }

    private static void PersistEncrypted(string plain)
    {
      var file = new EncryptedFile("hidden.tv", GetKey);
      using (VaultData data = new VaultData
      {
        Password = plain,
        Deadline = 0d,
        Waited = 0d
      })
      {
        file.WriteData(data);
      }
    }

    private static string RetrieveDecrypted()
    {
      using (VaultData data = new EncryptedFile("hidden.tv", GetKey).ReadData())
      {
        return data.Password;
      }
    }

    private static void PrintUsage()
    {
      Console.WriteLine("usage: timevault.exe [-r | [\"<string>\" \"<dd/mm/yyyy HH:mm>\" [-n] | -i ### [d|h|m] >]" + Environment.NewLine +
                                "-r: Use the current state and continue waiting." + Environment.NewLine +
                                "-n: Do not confirm that password was encrypted correctly." + Environment.NewLine +
                                "-i: Increment the wait time by ### d: days, h: hours, m: minutes");
    }
  }
}
