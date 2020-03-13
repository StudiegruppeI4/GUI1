using System;
using Prism.Mvvm;

namespace TheDebtBook.Models
{
    public class DebtEntry : BindableBase
    {
        private DateTime _entryDate;
        private double _amount;
        private string _description;

        public DebtEntry()
        {
            EntryDate = DateTime.Now;
            Amount = 0;
            Description = "";
        }

        public DebtEntry(DateTime entryDate, double amount, string description="")
        {
            EntryDate = entryDate;
            Amount = amount;
            Description = description;
        }

        public DateTime EntryDate
        {
            get => _entryDate;
            set => SetProperty(ref _entryDate, value);
        }

        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}