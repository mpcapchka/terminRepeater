using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TerminRepeater.ViewModel.Component
{
    public partial class ContainerTreeViewItemViewModel : ObservableObject
    {
        [ObservableProperty] private string name = string.Empty;

        public ContainerTreeViewItemViewModel(string name, IEnumerable<string> moduleNames)
        {
            Name = name;
            Modules = new(moduleNames.Select(x => new ModuleTreeViewItemViewModel(x)));
        }

        public ObservableCollection<ModuleTreeViewItemViewModel> Modules { get; } = new();
    }
}