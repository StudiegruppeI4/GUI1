using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TheDebtBook.Models;

namespace TheDebtBook.ViewModels
{
    public class AddDebtorViewModel : BindableBase
    {
        private Debtor _currentDebtor;

        public AddDebtorViewModel(Debtor debtor)
        {
            CurrentDebtor = debtor;
        }

        public Debtor CurrentDebtor
        {
            get => _currentDebtor;
            set => SetProperty(ref _currentDebtor, value);
        }
        
        private ICommand _okButtonCommand;

        public ICommand OkButtonCommand
        {
            get => _okButtonCommand ?? (_okButtonCommand =
                       new DelegateCommand(OkButtonCommandExecute, OkButtonCommandCanExecute)
                           .ObservesProperty(() => CurrentDebtor.Name).ObservesProperty(() => CurrentDebtor.Debt));
        }

        private void OkButtonCommandExecute()
        { }

        private bool OkButtonCommandCanExecute()
        {
            return IsValid;
        }

        public bool IsValid
        {
            get
            {
                bool isValid = true;
                if (string.IsNullOrWhiteSpace(CurrentDebtor.Name))
                    isValid = false;
                if (string.IsNullOrWhiteSpace(CurrentDebtor.Debt.ToString()))
                    isValid = false;
                return isValid;
            }
        }
    }
}