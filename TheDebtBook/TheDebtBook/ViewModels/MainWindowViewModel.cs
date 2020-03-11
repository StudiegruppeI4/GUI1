using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Runtime.Remoting.Channels;
using System.Windows.Threading;
using Prism.Mvvm;
using TheDebtBook.Models;

namespace TheDebtBook.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private Debtor _currentDebtor;
        private ObservableCollection<Debtor> _debtors;
        private int _currentIndex;
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindowViewModel()
        {
            Debtors = new ObservableCollection<Debtor>()
            {
                new Debtor("Annie", new DebtEntry(DateTime.Now, 100)),
                new Debtor("Charles", new DebtEntry(DateTime.Now, 300)),
                new Debtor("Johnny", new DebtEntry(DateTime.Now, 50)),
            };

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler((object sender, EventArgs e) => Clock.Update());
            timer.Start();
        }

        public Debtor CurrentDebtor
        {
            get => _currentDebtor;
            set => SetProperty(ref _currentDebtor, value);
        }

        public ObservableCollection<Debtor> Debtors
        {
            get => _debtors;
            set => SetProperty(ref _debtors, value);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetProperty(ref _currentIndex, value);
        }

        public Clock Clock { get; set; } = new Clock();
    }
}