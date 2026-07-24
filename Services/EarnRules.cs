namespace Bloom.Services;

public static class EarnRules
{
    public const int JournalWrite = 10;
    public const int Checkin = 6;
    public const int WindowOfTolerance = 6;
    public const int FoodLog = 4;
    public const int SymptomLog = 5;
    public const int DidLog = 6;
    public const int PageDecorated = 8;

    public const string KeyJournal = "earn.journal";
    public const string KeyCheckin = "earn.checkin";
    public const string KeyWot = "earn.wot";
    public const string KeyFood = "earn.food";
    public const string KeySymptom = "earn.symptom";
    public const string KeyDid = "earn.did";
    public const string KeyDecor = "earn.decor";

    public static string HabitKey(long habitId) => $"earn.habit.{habitId}";
}
