using System.IO.Abstractions;
using System.IO.Compression;

namespace TerminRepeater.Model
{
    public class ModuleProvider
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly string moduleExt;
        private string moduleName;
        #endregion

        #region Constructors
        public ModuleProvider(IFileSystem fileSys, string containerPath, string moduleName)
        {
            this.fileSystem = fileSys;
            this.moduleExt = "json";
            ContainerFullname = containerPath;
            ContainerName = fileSystem.Path.GetFileName(containerPath);
            this.moduleName = moduleName;
        }
        #endregion

        #region Properties
        public string ContainerFullname { get; }
        /// <summary>
        /// The name of container file with extension.
        /// </summary>
        public string ContainerName { get; }
        /// <summary>
        /// The name of module entry name with extension.
        /// </summary>
        public string ModuleName { get => moduleName; }
        #endregion

        #region Methods
        public void Rename(string name)
        {
            if (!NameAcceptable(name))
                throw new ArgumentException($"Unable to rename module to: {name}.");
            using var stream = fileSystem.FileStream.New(ContainerFullname, FileMode.Open, FileAccess.ReadWrite);
            using var archive = new ZipArchive(stream);
            var entry = archive.GetEntry(ModuleName);
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            using var memStream = new MemoryStream();
            using var entryStream = entry.Open();
            entryStream.CopyTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            entryStream.Dispose();
            entry.Delete();
            var newEntry = archive.CreateEntry($"{name}.{moduleExt}");
            using var newEntryStream = newEntry.Open(); 
            memStream.CopyTo(newEntryStream);
            moduleName = $"{name}.{moduleExt}";
        }
        public void Delete()
        {
            using var stream = fileSystem.FileStream.New(ContainerFullname, FileMode.Open, FileAccess.ReadWrite);
            var archive = new ZipArchive(stream, ZipArchiveMode.Update);
            var entry = archive.GetEntry(ModuleName);
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            entry.Delete();
        }
        public void Save(ModuleItem module)
        {
            using var strem = fileSystem.FileStream.New(
               )
        }
        public ModuleItem GetModule()
        {

        }
        public void AppendRate(Dictionary<int, bool> terminIdToResult)
        {

        }
        public bool NameAcceptable(string? name)
        {
            var chars = fileSystem.Path.GetInvalidFileNameChars();
            return !string.IsNullOrWhiteSpace(name)
                && !name.Any(x => chars.Contains(x) || x == '.');
        }
        #endregion
    }
}
