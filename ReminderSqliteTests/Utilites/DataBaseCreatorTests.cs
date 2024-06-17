using ReminderBot.ReminderSqlite.Utilites;

namespace ReminderBot.ReminderSqliteTests.Utilites;

[TestFixture] 
class DataBaseCreatorTests
{
    private const string dbName = "testDb";
    [TearDown]
    public void Setup()
    {
        Console.WriteLine("Setup");
        if (File.Exists(dbName))
        {
            Console.WriteLine("Deleting file");
            File.Delete(dbName);
            Console.WriteLine("OK");
        }
    }

    [Test]
    public void Run_CreateDatabase_DataBaseFileExists()
    {
        Console.WriteLine("Start Test");
        DataBaseCreator cdb = new DataBaseCreator(dbName);
        cdb.Run();
        Assert.That(File.Exists(dbName), Is.True);
        Console.WriteLine("End Test");
    }

    [Test]
    public void Run_CreateDatabase_RunWithoutExceptions()
    {
        Console.WriteLine("Start Test");
        DataBaseCreator cdb = new DataBaseCreator(dbName);
        Assert.DoesNotThrow(() => cdb.Run());
        Console.WriteLine("End Test");
    }
}