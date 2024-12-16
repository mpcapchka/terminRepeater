using CommunityToolkit.Mvvm.ComponentModel;
using TerminRepeater.Services;

namespace TerminRepeater.ViewModel
{
    public abstract class DemandViewModelBase : ViewModelBase
    {
        #region Fields
        private AutoResetEvent exitHandle = null!;
        #endregion

        #region Constructors
        public DemandViewModelBase(IContentManager contentManager, ILoggerService loggerService)
            : base(contentManager, loggerService)
        {

        }
        #endregion

        #region Methods
        public void SetExitHandle(AutoResetEvent exitHandle)
        {
            this.exitHandle = exitHandle;
        }
        protected void Exit()
        {
            exitHandle?.Set();
        }
        #endregion
    }
}
