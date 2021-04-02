using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using PrismBase.Mvvm;
using System.Windows;

namespace CombinePdf_GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields and properties
        private ViewModelBase mSelectedViewModel;
        private DelegateCommand mShutdownCommand;

        public ViewModelBase SelectedViewModel
        {
            get => mSelectedViewModel;
            set => SetProperty(ref mSelectedViewModel, value);
        }
        public DelegateCommand ShutdownCommand
        {
            get => mShutdownCommand ?? (mShutdownCommand = new DelegateCommand(ShutdownExecute));
            set => mShutdownCommand = value;
        }
        #endregion

        #region Constructors
        public MainWindowViewModel(IEventAggregator eventAggregator, IContainerProvider container) : base(eventAggregator, container)
        {
            SelectedViewModel = new CombinePdfViewModel(eventAggregator, container);
        }
        #endregion

        #region Command methods
        private void ShutdownExecute()
        {
            Application.Current.Shutdown();
        } 
        #endregion
    }
}
