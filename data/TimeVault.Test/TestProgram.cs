using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using TestingTools.Core;
using TestingTools.Extensions;

namespace TimeVault.Test
{
  [TestFixture]
  public class TestProgram
  {
    private const string DATA_FILE = "hidden.tv";
    private const string BACKUP_FILE = "hidden.tv.bak";
    private readonly static List<Thread> threads = new List<Thread>();

    [TearDown]
    public void cleanup()
    {
      cleanup(DATA_FILE);
      cleanup(BACKUP_FILE);
    }

    [TearDown]
    public void killThreads()
    {
      threads.ForEach(t =>
      {
        try
        {
          t.Abort();
        }
        catch (ThreadAbortException)
        {
        }
      });

      threads.Clear();
    }

    [Test]
    public void TestThatItCanWaitOneSecond()
    {
      // Arrange
      String noWait = DateTime.Now.Subtract(TimeSpan.FromMinutes(1)).ToString("dd/MM/yyyy HH:mm");
      String password = "my password";
      // Act
      Action act = () => Program.Main(new string[] { password, noWait, "-n" });
      // Assert
      Verify.That(act)
            .WritesToConsole(password)
            .And()
            .DoesntWriteToConsole("Error while trying to read data: Input string was not in a correct format")
            .Now();
    }

    [Test]
    public void TestThatItCanWaitOneDay()
    {
      // Arrange
      String oneDayWait = DateTime.Now.AddDays(1).AddMinutes(1).ToString("dd/MM/yyyy HH:mm");
      String expect = "1 days";
      // Act
      Action act = () => StartThread(() => Program.Main(new string[] { "my password", oneDayWait, "-n" }), 300);
      // Assert
      Verify.That(act)
            .WritesToConsole(expect)
            .And()
            .DoesntWriteToConsole("Error while trying to read data: Input string was not in a correct format")
            .Now();
    }

    [Test]
    public void TestThatItCanIncreaseOneDay()
    {
      // Arrange
      String expect = "1 days";
      string magnitude = "d";
      string amount = "1";
      CheckCanIncrease(expect, magnitude, amount);
    }

    [Test]
    public void TestThatItCanIncreaseTwoDays()
    {
      // Arrange
      String expect = "2 days";
      string magnitude = "d";
      string amount = "2";
      CheckCanIncrease(expect, magnitude, amount);
    }

    [Test]
    public void TestThatItCanIncreaseOneHour()
    {
      // Arrange
      String expect = "1 hrs";
      string magnitude = "h";
      string amount = "1";
      CheckCanIncrease(expect, magnitude, amount);
    }

    [Test]
    public void TestThatItCantDecreaseOneDay()
    {
      // Arrange
      String expect = "1 days";
      string magnitude = "d";
      string amount = "-1";
      CheckCanIncrease(expect, magnitude, amount);
    }

    [Test]
    public void TestThatItCantDecreaseOneHour()
    {
      // Arrange
      String expect = "1 hrs";
      string magnitude = "h";
      string amount = "-1";
      CheckCanIncrease(expect, magnitude, amount);
    }

    private static void CheckCanIncrease(String expect, string magnitude, string amount)
    {
      String minuteWait = DateTime.Now.AddMinutes(1).ToString("dd/MM/yyyy HH:mm");
      StartThread(() => Program.Main(new string[] { "my password", minuteWait, "-n" }), 300);

      // Act
      Action act = () => StartThread(() => Program.Main(new string[] { "-i", amount, magnitude }), 400);
      // Assert
      Verify.That(act)
            .WritesToConsole(expect)
            .And()
            .DoesntWriteToConsole("Error while trying to read data: Input string was not in a correct format")
            .Now();
    }

    private static void StartThread(ThreadStart act, int millis)
    {
      Thread t = new Thread(act);
      threads.Add(t);
      t.Start();
      t.Join(millis);
    }

    private static void cleanup(string filename)
    {
      try
      {
        String filePath = TestingTools.TestEnvironment.GetExecutionFilepath(filename);
        if (File.Exists(filePath))
        {
          File.Delete(filePath);
        }
      }
      catch (FileNotFoundException)
      { // ignore
      }
    }
  }
}
