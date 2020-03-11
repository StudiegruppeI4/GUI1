using System.Collections.Generic;
using System.Windows.Documents;
using Prism.Mvvm;

namespace TheDebtBook.Models
{
    public class Debtor : BindableBase
    {
        private string _name;
        private double _depth;
        private List<DebtEntry> _debtEntries;

        public Debtor(string name, DebtEntry initialEntry)
        {
            Name = name;
            DebtEntries = new List<DebtEntry>();
            DebtEntries.Add(initialEntry);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public double Depth
        {
            get => _depth;
            set => SetProperty(ref _depth, value);
        }

        public List<DebtEntry> DebtEntries
        {
            get => _debtEntries;
            set => SetProperty(ref _debtEntries, value);
        }
    }
}