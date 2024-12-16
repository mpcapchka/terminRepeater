using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TerminRepeater.Model;
using TerminRepeater.Services;
using TerminRepeater.ViewModel.Component;

namespace TerminRepeater.ViewModel
{
    public partial class ContainerEditorViewModel : ViewModelBase
    {
        #region Fields
        private readonly IDataManager dataManager;
        #endregion

        #region Constructors
        public ContainerEditorViewModel(IContentManager contentManager, 
            IDataManager dataManager,
            ILoggerService loggerService) 
            : base(contentManager, loggerService)
        {
            this.dataManager = dataManager;
        }
        #endregion

        #region Properties
        public ObservableCollection<ContainerTreeViewItemViewModel> Items { get; } = new();
        #endregion

        #region Methods
        [RelayCommand] private async Task CreateContainer()
        {
            try
            {
                var result = await contentManager.ShowPromptDialog("Enter the container's name.");
                if (!WarnIfContainerNameInvalid(result)) return;
                var container = new ContainerItem() { Name = result };
                dataManager.SaveContainer(container);
                Items.Insert(0, new ContainerTreeViewItemViewModel(result, Array.Empty<string>()));
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand] private async Task CreateModuleInContainer(ContainerTreeViewItemViewModel container)
        {
            try
            {
                var result = await contentManager.ShowPromptDialog("Enter the module's name.");
                if (!WarnIfModuleNameInvalid(result, container)) return;
                else
                {
                    var module = new ModuleItem() { Name = result };
                    var saveActionRequested = await contentManager.OpenModuleEditor(container.Modules.Select(x => x.Name), module);
                    if (!saveActionRequested || !WarnIfModuleNameInvalid(module.Name, container)) return;
                    dataManager.AppendModule(container.Name, module);
                    container.Modules.Add(new(module.Name));
                }
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand] private async Task RenameContainer(ContainerTreeViewItemViewModel container)
        {
            try
            {
                if (container == null) return;
                var result = await contentManager.ShowPromptDialog("Enter the module's name:");
                if (!WarnIfContainerNameInvalid(result)) return;
                dataManager.RenameContainer(container.Name, result);
                container.Name = result;
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand] private void DeleteContainer(ContainerTreeViewItemViewModel container)
        {
            try
            {
                if (container == null) return;
                dataManager.DeleteContainer(container.Name);
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand] private async Task EditModule(ModuleTreeViewItemViewModel module)
        {
            try
            {
                if (module == null) return;
                var container = Items.First(x => x.Modules.Contains(module));
                var dataContainer = dataManager.GetContainer(container.Name);
                var dataModuleIndex = Array.FindIndex(dataContainer.Items, x => x.Name == module.Name);
                var saveActionRequested = await contentManager.OpenModuleEditor(container.Modules.Select(x => x.Name), dataContainer.Items[dataModuleIndex]);
                if (!saveActionRequested) return;
                dataManager.SaveContainer(dataContainer);
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand] private async Task RenameModule(ModuleTreeViewItemViewModel module)
        {
            try
            {
                if (module == null) return;
                var container = Items.First(x => x.Modules.Contains(module));
                var result = await contentManager.ShowPromptDialog("Enter the module's name:");
                if (!WarnIfModuleNameInvalid(result, container)) return;
                dataManager.RenameModule(container.Name, module.Name, result);
                module.Name = result;
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand] private void DeleteModule(ModuleTreeViewItemViewModel module)
        {
            try
            {
                if (module == null) return;
                var container = Items.First(x => x.Modules.Contains(module));

            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        #endregion

        #region Helpers
        private bool WarnIfContainerNameInvalid(string? containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName)) return false;

            if (!dataManager.ContainerNameAcceptable(containerName))
            {
                contentManager.ShowWarnDialog("The entered container's name cannot contain a unrespected chars.");
                return false;
            }

            if (Items.Any(x => x.Name == containerName))
            {
                contentManager.ShowWarnDialog("The container with such name already exists.");
                return false;
            }

            return true;
        }
        private bool WarnIfModuleNameInvalid(string? moduleName, ContainerTreeViewItemViewModel container)
        {
            if (string.IsNullOrWhiteSpace(moduleName)) return false;
            
            if (!dataManager.ModuleNameAcceptable(moduleName))
            {
                contentManager.ShowWarnDialog("The entered module's name cannot contain a unrespected chars.");
                return false;
            }
            
            if (container.Modules.Any(x => x.Name == moduleName))
            {
                contentManager.ShowWarnDialog("The module with such name already exists in provided container.");
                return false;
            }
            
            return true;
        }
        #endregion
    }
}