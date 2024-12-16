namespace TerminRepeater.Model
{
    public class ContainerItem
    {
        public string Name { get; set; } = string.Empty;
        public ModuleItem[] Items { get; set; } = Array.Empty<ModuleItem>();
    }
    public class ModuleItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TerminItem[] Items { get; set; } = Array.Empty<TerminItem>();
    }
    public struct TerminItem
    {
        public int Id { get; set; }
        public string Termin { get; set; }
        public string Description { get; set; }
    }
}
