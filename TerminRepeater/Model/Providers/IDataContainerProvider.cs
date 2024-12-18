namespace TerminRepeater.Model
{
    public interface IDataContainerProvider
    {
        int Id { get; }
        string Name { get; }
        string LanguageTerm { get; }
        string LanguageDescription { get; }
    }
}
