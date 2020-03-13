using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Serialization;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using TheDebtBook.Models;
using TheDebtBook.Views;

namespace TheDebtBook.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private Debtor _currentDebtor;
        private ObservableCollection<Debtor> _debtors;
        private int _currentIndex;
        private DispatcherTimer timer = new DispatcherTimer();
        private string _filepath = "";
        private string _filename = "New";
        private readonly string _appTitle = "The Debt Book";

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

        #region Properties

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

        public double TotalDebt
        {
            get
            {
                double debt = 0;
                foreach (var debtor in Debtors)
                {
                    debt += debtor.Debt;
                }
                return debt;
            }
        }

        public string Filename
        {
            get => _filename;
            set
            {
                SetProperty(ref _filename, value);
                RaisePropertyChanged("Title");
            }
        }

        public string Title
        {
            get => $"{_appTitle} - {Filename}";
        }

        #endregion

        #region Commands

        private ICommand _addDebtorCommand;

        public ICommand AddDebtorCommand
        {
            get => _addDebtorCommand ?? (_addDebtorCommand = new DelegateCommand(() =>
            {
                var newDebtor = new Debtor();
                var vm = new AddDebtorViewModel(newDebtor);
                var dlg = new AddDebtorView()
                {
                    DataContext = vm
                };
                if (dlg.ShowDialog() == true)
                {
                    if (NameAlreadyTaken(newDebtor))
                        MessageBox.Show($"{newDebtor.Name} already taken. Please enter a new name.", "Name already taken", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        Debtors.Add(newDebtor);
                        newDebtor.DebtEntries.Add(new DebtEntry(DateTime.Now, newDebtor.Debt));
                        CurrentDebtor = newDebtor;
                        CurrentIndex = (Debtors.Count - 1);
                    }
                    RaisePropertyChanged("TotalDebt");
                }
            }));
        }

        private bool NameAlreadyTaken(Debtor debtor)
        {
            bool taken = false;
            foreach (var person in Debtors)
            {
                if (person.Name == debtor.Name)
                    taken = true;
            }
            return taken;
        }

        private ICommand _listViewItemDoubleClickCommand;

        public ICommand ListViewItemDoubleClickCommand
        {
            get => _listViewItemDoubleClickCommand ??
                   (_listViewItemDoubleClickCommand = new DelegateCommand(() =>
                   {
                       var tempDebtor = CurrentDebtor.Clone();
                       var vm = new DebtorViewModel(tempDebtor);

                       var dlg = new DebtorView()
                       {
                           DataContext = vm,
                           Owner = App.Current.MainWindow
                       };
                       if (dlg.ShowDialog() == true)
                       {
                           CurrentDebtor.Debt = tempDebtor.Debt;
                           CurrentDebtor.DebtEntries = tempDebtor.DebtEntries;
                           RaisePropertyChanged("TotalDebt");
                       }
                   }, () => 
                       { return CurrentIndex >= 0; }
                       ).ObservesProperty(() => CurrentIndex));
        }

        private ICommand _deleteDebtorCommand;

        public ICommand DeleteDebtorCommand
        {
            get => _deleteDebtorCommand ??
                   (_deleteDebtorCommand = new DelegateCommand(DeleteDebtorCommandExecute, DeleteDebtorCommandCanExecute).ObservesProperty(() => CurrentIndex));
        }

        private void DeleteDebtorCommandExecute()
        {
            MessageBoxResult res = MessageBox.Show($"Are you sure you want to delete {CurrentDebtor.Name}?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Debtors.Remove(CurrentDebtor);
                if (Debtors.Count > 0)
                {
                    CurrentDebtor = Debtors[(Debtors.Count - 1)];
                    CurrentIndex = (Debtors.Count - 1);
                }
                RaisePropertyChanged("TotalDebt");
            }
        }

        private bool DeleteDebtorCommandCanExecute()
        {
            if (Debtors.Count > 0 && CurrentIndex >= 0)
                return true;
            return false;
        }

        private ICommand _saveAsCommand;

        public ICommand SaveAsCommand
        {
            get => _saveAsCommand ?? (_saveAsCommand = new DelegateCommand<string>(SaveAsCommandExecute));
        }

        private void SaveAsCommandExecute(string fileName)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "Debtors Book documents|*.txt|All Files|*.*",
                DefaultExt = "txt"
            };
            if (_filepath == "")
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                dialog.InitialDirectory = Path.GetDirectoryName(_filepath);
            }

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                _filepath = dialog.FileName;
                Filename = Path.GetFileName(_filepath);
                SaveFile();
            }
        }

        private ICommand _saveCommand;

        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute).
                       ObservesProperty(() => Debtors.Count));
        }

        private void SaveCommandExecute()
        {
            SaveFile();
        }

        private bool SaveCommandCanExecute()
        {
            return (_filename != "") && (Debtors.Count > 0);
        }

        private void SaveFile()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Debtor>));
                TextWriter writer = new StreamWriter(_filepath);
                serializer.Serialize(writer, Debtors);
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ICommand _newFileCommand;
        public ICommand NewFileCommand
        {
            get { return _newFileCommand ?? (_newFileCommand = new DelegateCommand(NewFileCommandExecute)); }
        }

        private void NewFileCommandExecute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Debtors.Clear();
                Filename = "";
            }
        }

        private ICommand _openFileCommand;

        public ICommand OpenFileCommand
        {
            get => _openFileCommand ?? (_openFileCommand = new DelegateCommand<string>(OpenFileCommandExecute));
        }

        private void OpenFileCommandExecute(string fileName)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Debtors Book documents|*.txt|All Files|*.*",
                DefaultExt = "txt"
            };
            if (_filepath == "")
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                dialog.InitialDirectory = Path.GetDirectoryName(_filepath);
            }

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                _filepath = dialog.FileName;
                Filename = Path.GetFileName(_filepath);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Debtor>));
                    TextReader reader = new StreamReader(_filepath);
                    Debtors = (ObservableCollection<Debtor>) serializer.Deserialize(reader);
                    if (Debtors.Count > 0)
                    {
                        CurrentDebtor = Debtors[0];
                        CurrentIndex = 0;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new DelegateCommand(ExitCommandExecute)); }
        }

        private void ExitCommandExecute()
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to close the program? Remember to save your work before you close.", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Application.Current.MainWindow.Close();
            }
        }

        #endregion
    }
}