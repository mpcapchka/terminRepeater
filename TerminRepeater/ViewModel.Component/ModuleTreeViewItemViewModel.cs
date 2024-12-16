using CommunityToolkit.Mvvm.ComponentModel;

namespace TerminRepeater.ViewModel.Component
{
    public partial class ModuleTreeViewItemViewModel : ObservableObject
    {
        [ObservableProperty] private string name = string.Empty;
        public ModuleTreeViewItemViewModel(string name)
        {
            this.Name = name;
        }
    }
}
