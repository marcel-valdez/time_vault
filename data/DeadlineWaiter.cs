namespace TimeVault
{
  using System;
  using System.Diagnostics;
  using System.Threading;

  public class DeadlineWaiter
  {
    private readonly EncryptedFile mBackupEncryptedFile;
    private readonly EncryptedFile mEncryptedFile;
    private const string FILENAME = "hidden.tv";
    // BACKUP after each successful write
    private const string BACKUP = "hidden.tv.bak";
    private const int MINUTE = 60 * 1000;
    private DateTime? initialDeadline;
    private readonly Func<string> passGetter;

    public DeadlineWaiter(Func<string> keyGetter, Func<string> passGetter)
      : this(null, keyGetter, passGetter)
    {
    }

    public DeadlineWaiter(DateTime? deadline, Func<string> keyGetter, Func<string> passGetter)
    {
      mEncryptedFile = new EncryptedFile(FILENAME, keyGetter);
      mBackupEncryptedFile = new EncryptedFile(BACKUP, keyGetter);
      this.initialDeadline = deadline;
      this.passGetter = passGetter;
    }

    public static bool IsDeadlineAchieved(Func<string> keyGetter)
    {
      using (VaultData data = (new EncryptedFile(FILENAME, keyGetter)).ReadData())
      {
        return data.Deadline <= data.Waited;
      }
    }

    public static string ToTimeString(double seconds)
    {
      var timeSpan = TimeSpan.FromSeconds(seconds);
      return string.Format("You still have to wait: {0} days {1} hrs {2} mins {3} secs",
                            timeSpan.Days,
                            timeSpan.Hours,
                            timeSpan.Minutes,
                            timeSpan.Seconds);
    }

    public void Wait()
    {
      InitializeFile(passGetter());
      bool useBackup = false;
      var stopWatch = Stopwatch.StartNew();
      while (true)
      {
        try
        {
          if (useBackup)
          {
            this.mEncryptedFile.CopyFrom(this.mBackupEncryptedFile);
          }
          else
          {
            this.mBackupEncryptedFile.CopyFrom(this.mEncryptedFile);
          }

          double sleepTime = MINUTE;
          using (VaultData data = this.mEncryptedFile.ReadData())
          {
            if (data.Deadline <= data.Waited)
            {
              Console.WriteLine("Password: {0}", data.Password);
              return;
            }

            Console.WriteLine(ToTimeString(data.Deadline - data.Waited));
            if (data.Deadline - data.Waited < MINUTE/1000)
            {
              sleepTime = (data.Deadline - data.Waited) * 1000;
            }
          }

          Thread.Sleep((int)sleepTime);

          double elapsed = stopWatch.ElapsedMilliseconds / 1000d;
          stopWatch.Restart(); // Time taken to write the message must be included in the next time-span.
          using (VaultData data = this.mEncryptedFile.ReadData())
          {
            data.Waited += elapsed;
            mEncryptedFile.WriteData(data);
          }

          useBackup = false;
        }
        catch (Exception e)
        {
          Console.WriteLine("Error while trying to read data: " + e.Message + e.StackTrace);
          if (!useBackup)
          {
            Console.WriteLine("Trying to use backup file...");
            useBackup = true;
          }
          else
          {
            Console.WriteLine("Your are fucked, your password is gone.");
            return;
          }
        }
      }
    }

    public void InitializeFile(string pass)
    {
      if (initialDeadline.HasValue)
      {
        TimeSpan wait = initialDeadline.Value - DateTime.Now;
        using (VaultData data = new VaultData()
        {
          Password = pass,
          Deadline = wait.TotalSeconds,
          Waited = 0d
        })
        {
          this.mEncryptedFile.WriteData(data);
        }
      }
    }

    public void Increment(TimeSpan increment)
    {
      if (this.initialDeadline.HasValue)
      {
        this.initialDeadline = this.initialDeadline.Value.Add(increment);
      }

      using (VaultData data = this.mEncryptedFile.ReadData())
      {
        data.Deadline = data.Deadline + increment.TotalSeconds;
        this.mEncryptedFile.WriteData(data);
      }
    }
  }
}
