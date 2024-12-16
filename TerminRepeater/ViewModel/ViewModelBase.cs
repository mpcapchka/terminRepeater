using CommunityToolkit.Mvvm.ComponentModel;
using TerminRepeater.Services;

namespace TerminRepeater.ViewModel
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected readonly IContentManager contentManager;
        protected readonly ILoggerService logger;
        public ViewModelBase(IContentManager contentManager, ILoggerService logger)
        {
            this.contentManager = contentManager;
            this.logger = logger;
        }
    }
}
