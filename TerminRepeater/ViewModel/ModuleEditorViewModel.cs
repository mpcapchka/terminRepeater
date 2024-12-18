using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using TerminRepeater.Model;
using TerminRepeater.Services;

namespace TerminRepeater.ViewModel
{
    public partial class ModuleEditorViewModel : DemandViewModelBase
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        [ObservableProperty] private string moduleName = string.Empty;
        [ObservableProperty] private string moduleDescription = string.Empty;
        #endregion

        #region Constructors
        public ModuleEditorViewModel(IContentManager contentManager,
            IFileSystem fileSystem,
            ILoggerService logger)
            : base(contentManager, logger)
        {
            this.fileSystem = fileSystem;
        }
        #endregion

        #region Properties
        public bool ChangesApplied { get; private set; }
        public IReadOnlyList<string> ExistingModuleNames { get; init; } = Array.Empty<string>();
        public ObservableCollection<TerminItem> Items { get; } = new();
        #endregion

        #region Methods
        [RelayCommand] private void AppendItem()
        {
            var newId = Items.Select(x => x.Id).Max() + 1;
            Items.Add(new TerminItem() { Id = newId });
        }
        [RelayCommand] private void RemoveItemById(int itmeId)
        {
            if (Items.Any(x => x.Id == itmeId))
            {
                var index = Items.IndexOf(Items.First(x => x.Id == itmeId));
                Items.RemoveAt(index);
            }
        }
        [RelayCommand] private void Save()
        {
            var unrespectedChars = fileSystem.Path.GetInvalidFileNameChars();
            if (ModuleName.Any(x => unrespectedChars.Contains(x)))
                contentManager.ShowWarnDialog("The module's name contains some of unrespected characters.");
            else if (ExistingModuleNames != null && ExistingModuleNames.Contains(ModuleName))
                contentManager.ShowWarnDialog("The module's name were already in use. Try another one.");
            else if (string.IsNullOrWhiteSpace(ModuleName))
                contentManager.ShowWarnDialog("The module's name cannot be empty.");
            else
            {
                ChangesApplied = true;
                Exit();
            }
        }
        [RelayCommand] private void Cancel()
        {
            ChangesApplied = false;
            Exit();
        }
        #endregion
    }
}
