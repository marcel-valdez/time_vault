using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using TestingTools.Core;
using TestingTools.Extensions;

namespace TimeVault.Test
{
  [TestFixture]
  public class DeadlineWaiterTest
  {
    private const string PASSWORD = "password";
    private const string DATA_FILE = "hidden.tv";
    private const string BACKUP_FILE = "hidden.tv.bak";
    private readonly static Func<string> KEY_GETTER = () => "ziGdDyAnIFioPnv3";
    private readonly static Func<string> PASS_GETTER = () => PASSWORD;

    [TearDown]
    public void cleanup()
    {
      cleanup(DATA_FILE);
      cleanup(BACKUP_FILE);
    }

    [Test]
    public void TestIfInitializesFile()
    {
      // Arrange

      DateTime deadline = DateTime.Now.AddSeconds(-1);
      var target = new DeadlineWaiter(deadline, KEY_GETTER, PASS_GETTER);

      // Act
      target.InitializeFile(PASSWORD);

      // Assert
      String filePath = TestingTools.TestEnvironment.GetExecutionFilepath(DATA_FILE);
      Verify.That(File.Exists(filePath)).IsTrue().Now();
    }

    [TestAttribute]
    public void TestIfItInitializes_ValidValues_PastDeadline()
    {
      checkInitializeFile(DateTime.Now.AddSeconds(-1), true);
    }

    [TestAttribute]
    public void TestIfItInitializes_ValidValues()
    {
      checkInitializeFile(DateTime.Now.AddSeconds(1), false);
    }

    [TestAttribute]
    public void TestIfItCanWait100Milliseconds()
    {
      // Arrange
      var target = new DeadlineWaiter(DateTime.Now.AddMilliseconds(100d), KEY_GETTER, PASS_GETTER);
      Stopwatch stopWatch = Stopwatch.StartNew();
      // Act
      Action act = () =>
      {
        stopWatch.Restart();
        target.Wait();
        stopWatch.Stop();
      };

      // Assert
      Verify.That(act).WritesToConsole(PASSWORD).Now();
      Verify.That(stopWatch.ElapsedMilliseconds >= 100).IsTrue(stopWatch.ElapsedMilliseconds + " is not >= 100")
            .And(stopWatch.ElapsedMilliseconds < 200).IsTrue(stopWatch.ElapsedMilliseconds + " is not < 200")
            .Now();
    }

    [TestAttribute]
    public void TestIfItCanIncrease100Milliseconds()
    {
      // Arrange
      var target = new DeadlineWaiter(DateTime.Now.AddMilliseconds(100d), KEY_GETTER, PASS_GETTER);
      target.InitializeFile(PASSWORD);
      target.Increment(TimeSpan.FromMilliseconds(100d));
      Stopwatch stopWatch = Stopwatch.StartNew();
      // Act
      stopWatch.Restart();
      target.Wait();
      stopWatch.Stop();

      // Assert
      Verify.That(stopWatch.ElapsedMilliseconds >= 100).IsTrue(stopWatch.ElapsedMilliseconds + " is not >= 100")
            .And(stopWatch.ElapsedMilliseconds < 300).IsTrue(stopWatch.ElapsedMilliseconds + " is not < 200")
            .Now();
    }

    private static void checkInitializeFile(DateTime deadline, bool expected)
    {
      // Arrange

      var target = new DeadlineWaiter(deadline, KEY_GETTER, PASS_GETTER);
      target.InitializeFile(PASSWORD);

      // Act
      bool actual = DeadlineWaiter.IsDeadlineAchieved(KEY_GETTER);

      // Assert
      Verify.That(actual).IsEqualTo(expected).Now();

    }

    private static void cleanup(string hiddentv)
    {
      try
      {
        String filePath = TestingTools.TestEnvironment.GetExecutionFilepath(hiddentv);
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