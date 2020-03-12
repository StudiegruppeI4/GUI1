using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Prism.Mvvm;

namespace TheDebtBook.Models
{
    public class Debtor : BindableBase
    {
        private string _name;
        private double _debt;
        private ObservableCollection<DebtEntry> _debtEntries = new ObservableCollection<DebtEntry>();

        public Debtor()
        {
        }

        public Debtor(string name, DebtEntry initialEntry)
        {
            Name = name;
            DebtEntries.Add(initialEntry);
            Debt = initialEntry.Amount;
        }

        public Debtor Clone()
        {
            return this.MemberwiseClone() as Debtor;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public double Debt
        {
            get => _debt;
            set => SetProperty(ref _debt, value);
        }

        public ObservableCollection<DebtEntry> DebtEntries
        {
            get => _debtEntries;
            set => SetProperty(ref _debtEntries, value);
        }
    }
}