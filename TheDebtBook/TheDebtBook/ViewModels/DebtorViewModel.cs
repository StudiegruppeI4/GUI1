using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TheDebtBook.Models;

namespace TheDebtBook.ViewModels
{
    public class DebtorViewModel : BindableBase
    {
        private Debtor _currentDebtor;
        private double _valueField;

        public DebtorViewModel(Debtor debtor)
        {
            CurrentDebtor = debtor;
        }

        public Debtor CurrentDebtor
        {
            get => _currentDebtor;
            set => SetProperty(ref _currentDebtor, value);
        }

        public double ValueField
        {
            get => _valueField;
            set => SetProperty(ref _valueField, value);
        }

        private ICommand _addValueButtonCommand;

        public ICommand AddValueButtonCommand
        {
            get => _addValueButtonCommand ?? (_addValueButtonCommand =
                       new DelegateCommand(AddValueButtonCommandExecute, AddValueButtonCommandCanExecute)
                           .ObservesProperty(() => CurrentDebtor.Name).ObservesProperty(() => CurrentDebtor.Debt));
        }

        private void AddValueButtonCommandExecute()
        {
            if (ValueField != 0)
            {
                CurrentDebtor.DebtEntries.Add(new DebtEntry(DateTime.Now, ValueField));
                CurrentDebtor.Debt += ValueField;
            }
        }

        private bool AddValueButtonCommandCanExecute()
        {
            return IsValid;
        }

        public bool IsValid
        {
            get
            {
                bool isValid = true;
                if (string.IsNullOrWhiteSpace(ValueField.ToString()))
                    isValid = false;

                return isValid;
            }
        }
    }
}