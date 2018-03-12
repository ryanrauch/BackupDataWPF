using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using WinForms = System.Windows.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BackupDataWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private ObservableCollection<String> _backupDirectories { get; set; }
        public ObservableCollection<String> BackupDirectories
        {
            get
            {
                return _backupDirectories ?? (_backupDirectories = new ObservableCollection<string>());
            }
            set
            {
                _backupDirectories = value;
                OnPropertyChanged("BackupDirectories");
            }
        }
        private String _selectedPath { get; set; }
        public String SelectedPath
        {
            get
            {
                return _selectedPath ?? String.Empty;
            }
            set
            {
                _selectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            ReadSettings("backupdata-settings.txt");
        }

        /// <summary>
        /// Reads a text file encoded like below:
        /// 1:path
        /// 2:username
        /// 3:password
        /// 4:local-path
        /// 4: ...
        /// 4: local-path
        /// </summary>
        /// <param name="file"></param>
        private void ReadSettings(String file)
        {
            BackupDirectories.Clear();
            if (!File.Exists(file))
                return;
            String line = String.Empty;
            using (StreamReader sr = new StreamReader(file))
            {

                while ((line = sr.ReadLine()) != null && !String.IsNullOrEmpty(line.Trim()))
                {
                    if (line.StartsWith("1:"))
                        TextBoxPath.Text = line.Substring(2);
                    else if (line.StartsWith("2:"))
                        TextBoxUsername.Text = line.Substring(2);
                    else if (line.StartsWith("3:"))
                        TextBoxPassword.Text = line.Substring(2);
                    else if (line.StartsWith("4:"))
                        BackupDirectories.Add(line.Substring(2));
                    else
                        throw new Exception(line);
                }
                sr.Close();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new WinForms.FolderBrowserDialog())
            {
                WinForms.DialogResult result = dialog.ShowDialog();
                if (result == WinForms.DialogResult.OK)
                    BackupDirectories.Add(dialog.SelectedPath);
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonBackup_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
