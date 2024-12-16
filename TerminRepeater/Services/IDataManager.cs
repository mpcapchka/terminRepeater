using Newtonsoft.Json;
using System.IO.Abstractions;
using System.IO.Compression;
using TerminRepeater.Model;

namespace TerminRepeater.Services
{
    public interface IDataManager
    {
        string[] GetAvailableContainers();

        string[] GetContainerModuleNames(string containerName);

        ContainerItem GetContainer(string containerName);

        /// <summary>
        /// Saves container in the local storage by it's name. If container with the such name already exists it will be overwritten.
        /// </summary>
        /// <param name="containerItem"></param>
        void SaveContainer(ContainerItem containerItem);

        void DeleteContainer(string containerName);

        void RenameContainer(string oldContainerName, string newContainerName);

        void AppendModule(string containerName, ModuleItem module);

        void RenameModule(string containerName, string oldModuleName, string newModuleName);

        void DeleteModule(string containerName, string moduleName);

        /// <summary>
        /// Checks if provided <paramref name="containerName"/> is matches manager's rules.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns><see cref="bool">true</see> if name acceptable, otherwise <see cref="bool">false</see>.</returns>
        bool ContainerNameAcceptable(string? containerName);

        /// <summary>
        /// Checks if provided <paramref name="moduleName"/> is matches manager's rules.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns><see cref="bool">true</see> if name is acceptable, otherwise <see cref="bool">false</see>.</returns>
        bool ModuleNameAcceptable(string? moduleName);
    }
    public class DataManager : IDataManager
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly string localDirectory;
        private readonly string containerExtension = "zip";
        private readonly string moduleExtension = "json";
        #endregion

        #region Constructors
        public DataManager(IBuildInfo buildInfo)
        {
            this.fileSystem = buildInfo.FileSystem;
            this.localDirectory = buildInfo.LocalDataStorageDirectory;
        }
        #endregion

        #region Methods
        public string[] GetAvailableContainers()
        {
            return fileSystem.Directory.GetFiles(localDirectory)
                .Select(x => fileSystem.Path.GetFileNameWithoutExtension(x)).ToArray();
        }
        public string[] GetContainerModuleNames(string containerName)
        {
            var filePath = fileSystem.Path.Combine(localDirectory, $"{containerName}.{containerExtension}");
            if (!fileSystem.File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            using var fileStream = fileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.ReadWrite);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            return archive.Entries.Select(x => fileSystem.Path.GetFileNameWithoutExtension(x.Name)).ToArray();
        }
        public ContainerItem GetContainer(string containerName)
        {
            var filePath = fileSystem.Path.Combine(localDirectory, $"{containerName}.{containerExtension}");
            if (fileSystem.File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            using var stream = fileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var entries = archive.Entries;
            ModuleItem[] array = new ModuleItem[entries.Count];
            var i = 0;
            foreach (var entry in entries)
            {
                using var entryStream = entry.Open();
                using var reader = new StreamReader(entryStream);
                using var jsonReader = new JsonTextReader(reader);
                var serializer = new JsonSerializer();
                var module = serializer.Deserialize<ModuleItem>(jsonReader);
                if (module == null) { i++; continue; }
                array[i] = module;
                i++;
            }
            return new ContainerItem() { Name = containerName, Items = array.Where(x => x != null).ToArray() };
        }
        public void SaveContainer(ContainerItem containerItem)
        {
            var unrespectableChars = fileSystem.Path.GetInvalidPathChars();
            if (containerItem == null)
                throw new ArgumentNullException(nameof(containerItem));
            else if (string.IsNullOrWhiteSpace(containerItem.Name))
                throw new ArgumentException("The container's name cannot be empty!");
            else if (containerItem.Name.Any(x => unrespectableChars.Contains(x)))
                throw new ArgumentException("The container's name cannot contain unrespected characters!");
            else if (containerItem.Items.Any(x => string.IsNullOrWhiteSpace(x.Name)))
                throw new ArgumentException("Some of module's contains an empty name.");
            else if (containerItem.Items.Any(x => x.Name.Any(c => unrespectableChars.Contains(c))))
                throw new ArgumentException("Some of module's contains a unrespected characters in name.");
            else if (containerItem.Items.Any(x => IsSomeTerminIdDuplicated(x.Items)))
                throw new ArgumentNullException("Some ids were duplicated");

            var filePath = fileSystem.Path.Combine(localDirectory, $"{containerItem}.{containerExtension}");
            using var stream = fileSystem.FileStream.New(filePath, FileMode.Create, FileAccess.ReadWrite);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
            var serializer = new JsonSerializer();
            foreach (var module in containerItem.Items)
            {
                var entryName = $"{module.Name}.{moduleExtension}";
                var entry = archive.CreateEntry(entryName, CompressionLevel.SmallestSize);
                using var entryStream = entry.Open();
                using var sw = new StreamWriter(entryStream);
                using var jsonWriter = new JsonTextWriter(sw);
                serializer.Serialize(jsonWriter, module);
            }
        }
        public void DeleteContainer(string containerName)
        {
            var fileName = fileSystem.Path.Combine(localDirectory, $"{containerName}.{containerExtension}");
            if (!fileSystem.File.Equals(fileName))
                throw new FileNotFoundException(fileName);
            fileSystem.File.Delete(fileName);
        }
        public void RenameContainer(string oldContainerName, string newContainerName)
        {
            var oldFileName = fileSystem.Path.Combine(localDirectory, $"{oldContainerName}.{containerExtension}");
            var unrespectedChars = fileSystem.Path.GetInvalidFileNameChars();
            if (!fileSystem.File.Equals(oldFileName))
                throw new FileNotFoundException(oldFileName);
            else if (string.IsNullOrWhiteSpace(newContainerName))
                throw new ArgumentNullException(nameof(newContainerName));
            else if (string.IsNullOrWhiteSpace(oldContainerName))
                throw new ArgumentNullException(nameof(oldContainerName));
            else if (newContainerName.Any(x => newContainerName.Contains(x)))
                throw new ArgumentException("The new name for container cannot contain a unrespected characters.");
            var newFileName = fileSystem.Path.Combine(localDirectory, $"{newContainerName}.{containerExtension}");
            fileSystem.File.Move(oldFileName, newFileName);
        }
        public void RenameModule(string containerName, string oldModuleName, string newModuleName)
        {
            string filePath = fileSystem.Path.Combine(localDirectory, $"{containerName}.{containerExtension}");
            if (!fileSystem.File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            using var stream = fileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.ReadWrite);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
            var entries = archive.Entries.ToArray();
            var newEntry = archive.CreateEntry(newModuleName + $".{moduleExtension}");
            var oldEnty = archive.GetEntry($"{oldModuleName}.{moduleExtension}")!;

            using (var oldEntryStream = oldEnty.Open())
            {
                using var newEntryStream = newEntry.Open();
                oldEntryStream.CopyTo(newEntryStream);
            }
            oldEnty.Delete();
        }
        #endregion

        #region Helpers
        private bool IsSomeTerminIdDuplicated(IEnumerable<TerminItem> items)
        {
            foreach (var item in items)
                if (items.Where(x => x.Id == item.Id).Count() > 1) return true;
            return false;
        }
        #endregion
    }
}
