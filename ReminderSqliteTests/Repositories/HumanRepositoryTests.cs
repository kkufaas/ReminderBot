using ReminderBot.ReminderModels.Models;
using ReminderBot.ReminderSqlite.Utilites;

namespace ReminderBot.ReminderSqliteTests.Repositories;

[TestFixture]

class HumanRepositoryTests
{
    private const string dbName = "testDb";
    private  RepositoryProviderSqlite sqliteProvider = null!;
    [SetUp]

    public void Setup()
    {
        if (File.Exists(dbName))
        {
            File.Delete(dbName);
        }
        DataBaseCreator cdb = new DataBaseCreator(dbName);
        cdb.Run(false);
        sqliteProvider = new RepositoryProviderSqlite(dbName, false);
    }

    [Test]
    public void AddHuman_SimpleAddition_WithoutException()
    {
        Assert.DoesNotThrow(() => sqliteProvider.HumanRepository.Add(new Human()));
        
    }
    [Test]
    public void GetHuman_GetByIdAfterAddition_ReturnSameObject()
    {
        Human human = new Human();
        sqliteProvider.HumanRepository.Add(human);
        Human result = sqliteProvider.HumanRepository.GetById(human.Id);
        Assert.That(result, Is.EqualTo(human));
    }

    [Test]
    public void GetHuman_GetByIdAfterAddition_NoExceptions()
    {
        Human human = new Human();
        sqliteProvider.HumanRepository.Add(human);
        Assert.DoesNotThrow(() => sqliteProvider.HumanRepository.GetById(human.Id));
    }
}
