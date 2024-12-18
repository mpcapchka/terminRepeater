using CommunityToolkit.Mvvm.Input;
using TerminRepeater.Services;

namespace TerminRepeater.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Constructors
        public MainViewModel(IContentManager contentManager, ILoggerService logger)
            : base(contentManager, logger)
        {
        }
        #endregion

        #region Methods
        [RelayCommand] private void NavigateToContainerEditor()
        {
            try
            {

            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        #endregion
    }
}
