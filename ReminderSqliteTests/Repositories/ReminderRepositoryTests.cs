using ReminderBot.ReminderModels.Models;
using ReminderBot.ReminderSqlite.Utilites;

namespace ReminderBot.ReminderSqliteTests.Repositories;

[TestFixture]

class ReminderRepositoryTests
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
    public void AddReminder_SimpleAddition_WithoutException()
    {
        Human testHuman = new Human ();
        sqliteProvider.HumanRepository.Add(testHuman);
        Reminder testReminder = new Reminder ("newReminder", testHuman.Id);
        Assert.DoesNotThrow(() => sqliteProvider.ReminderRepository.Add(testReminder));
    }

    [Test]
    public void GetAllByUser_GetByUserIdAfterAddition_ReturnSameObject_checkQuantity()
    {
        Human human = new Human();
        sqliteProvider.HumanRepository.Add(human);
        Reminder testReminder = new Reminder ("newReminder", human.Id);
        sqliteProvider.ReminderRepository.Add(testReminder);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByUser(human.Id));
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public void GetAllByUser_GetByUserIdAfterAddition_ReturnSameObject_checkIdentity()
    {
        Human human = new Human();
        sqliteProvider.HumanRepository.Add(human);
        Reminder testReminder = new Reminder ("newReminder", human.Id);
        sqliteProvider.ReminderRepository.Add(testReminder);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByUser(human.Id));
        Assert.That(result[0], Is.EqualTo(testReminder));
    }

    [Test]
    public void GetAllByUser_GetByUserIdAfterAddition_ReturnSameObject_quantityForMoreThanOne()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder2 = new Reminder ("newReminder", human2.Id);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByUser(human1.Id));
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public void GetAllByUser_GetByUserIdAfterAddition_ReturnSameObject_checkIdentityForMoreThanOne()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder2 = new Reminder ("newReminder", human2.Id);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByUser(human2.Id));
        Assert.That(result[0], Is.EqualTo(testReminder2));
    }

    [Test]
    public void GetAllByUser_ThreeRemindersByUser_ReturnsAllThree()
    {
        Human human = new Human();
        sqliteProvider.HumanRepository.Add(human);
        Reminder testReminder1 = new Reminder ("newReminder", human.Id);
        Reminder testReminder2 = new Reminder ("newReminder", human.Id);
        Reminder testReminder3 = new Reminder ("newReminder", human.Id);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        sqliteProvider.ReminderRepository.Add(testReminder3);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByUser(human.Id));
        Assert.That(result.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetAllByUser_ThreeRemindersByUser_AllThreeAreAreUnique()
    {
        Human human = new Human();
        sqliteProvider.HumanRepository.Add(human);
        Reminder testReminder1 = new Reminder ("newReminder", human.Id);
        Reminder testReminder2 = new Reminder ("newReminder", human.Id);
        Reminder testReminder3 = new Reminder ("newReminder", human.Id);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        sqliteProvider.ReminderRepository.Add(testReminder3);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByUser(human.Id));
        Assert.That(result, Contains.Item(testReminder1));
        Assert.That(result, Contains.Item(testReminder2));
        Assert.That(result, Contains.Item(testReminder3));
    }

    [Test]
    public void GetAllByDate_TwoRemindersByTwoUsers_ReturnsAllTwo()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder2 = new Reminder ("newReminder", human2.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        DateTime date = new DateTime(2024, 08, 26);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDate(date));
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetAllByDate_TwoRemindersByTwoUsers_ReturnsTwoByDate()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder2 = new Reminder ("newReminder", human2.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        DateTime date = new DateTime(2024, 08, 26);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDate(date));
        Assert.That(result[0].DateTime, Is.EqualTo(date));
        Assert.That(result[1].DateTime, Is.EqualTo(date));
    }

    [Test]
    public void GetAllByDate_TwoRemindersByTwoUsers_ReturnsOneOfTwoDifferentDates()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder2 = new Reminder ("newReminder", human2.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 25);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        DateTime date = new DateTime(2024, 08, 26);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDate(date));
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public void GetAllByDate_TwoRemindersByTwoUsers_ReturnsOneUniqueOfTwoDifferentDates()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder2 = new Reminder ("newReminder", human2.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 25);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        DateTime date = new DateTime(2024, 08, 26);
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDate(date));
        Assert.That(result[0].DateTime, Is.EqualTo(date));
    }

    [Test]
    public void GetAllByDateAndUser_OneReminde_ReturnsSameDateAndId()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        DateTime date = new DateTime(2024, 08, 26);
        long user = human1.Id;
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDateAndUser(date, user));
        Assert.That(result[0].DateTime, Is.EqualTo(date));
        Assert.That(result[0].Id, Is.EqualTo(user));
    }

     [Test]
    public void GetAllByDateAndUser_DifferentUsersAndDates_ReturnsOne()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        Reminder testReminder2 = new Reminder ("newReminder", human1.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 25);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        Reminder testReminder3 = new Reminder ("newReminder", human2.Id);
        testReminder3.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder3);
        DateTime date = new DateTime(2024, 08, 26);
        long user = human1.Id;
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDateAndUser(date, user));
        Assert.That(result.Count, Is.EqualTo(1));
    }

     [Test]
    public void GetAllByDateAndUser_DifferentUsersAndDates_ReturnsUnique()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        Reminder testReminder2 = new Reminder ("newReminder", human1.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 25);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        Reminder testReminder3 = new Reminder ("newReminder", human2.Id);
        testReminder3.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder3);
        DateTime date = new DateTime(2024, 08, 26);
        long user = human1.Id;
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDateAndUser(date, user));
        Assert.That(result[0].DateTime, Is.EqualTo(date));
        Assert.That(result[0].Id, Is.EqualTo(user));
    }

    [Test]
    public void GetAllByDateAndUser_DifferentUsersAndDates_ReturnsTwo()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        Reminder testReminder2 = new Reminder ("newReminder", human1.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        Reminder testReminder3 = new Reminder ("newReminder", human2.Id);
        testReminder3.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder3);
        DateTime date = new DateTime(2024, 08, 26);
        long user = human1.Id;
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDateAndUser(date, user));
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetAllByDateAndUser_DifferentUsersAndDates_ReturnsTwoUnique()
    {
        Human human1 = new Human();
        sqliteProvider.HumanRepository.Add(human1);
        Human human2 = new Human();
        sqliteProvider.HumanRepository.Add(human2);
        Reminder testReminder1 = new Reminder ("newReminder", human1.Id);
        testReminder1.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder1);
        Reminder testReminder2 = new Reminder ("newReminder", human1.Id);
        testReminder2.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder2);
        Reminder testReminder3 = new Reminder ("newReminder", human2.Id);
        testReminder3.DateTime = new DateTime(2024, 08, 26);
        sqliteProvider.ReminderRepository.Add(testReminder3);
        DateTime date = new DateTime(2024, 08, 26);
        long user = human1.Id;
        List <Reminder> result = new List<Reminder>(sqliteProvider.ReminderRepository.GetAllByDateAndUser(date, user));
        Assert.That(result, Contains.Item(testReminder1));
        Assert.That(result, Contains.Item(testReminder2));
    }



    




    
}
