public interface IHumanDataRepository
{
    string GetTitle(long tgId);
    void AssignTitle (long tgId, string title);
    string GetState (long tgId);
    void AssignState (long tgId, string state);
    void AssignDate (long tgId, DateTime date);
    DateTime GetDateTime (long tgId);
    bool HasData(long tgId);
    void CreateData (long tgId);
}