using ReminderBot.ReminderSqlite.Utilites;

namespace ReminderBot.ReminderSqliteTests.Utilites;

[TestFixture] 
class DataBaseCreatorTests
{
    private const string dbName = "testDb";
    [TearDown]
    public void Setup()
    {
        if (File.Exists(dbName))
        {
            File.Delete(dbName);
        }
    }

    [Test]
    public void Run_CreateDatabase_DataBaseFileExists()
    {
        DataBaseCreator cdb = new DataBaseCreator(dbName);
        cdb.Run();
        Assert.That(File.Exists(dbName), Is.True);
    }

    [Test]
    public void Run_CreateDatabase_RunWithoutExceptions()
    {
        DataBaseCreator cdb = new DataBaseCreator(dbName);
        Assert.DoesNotThrow(() => cdb.Run());
    }
}