using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using WinForms = System.Windows.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BackupDataWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IBackupProcedure backupProcedure { get; set; }
        public const String FILEPATH = "backupdata-settings.txt";
        private String _destinationPath { get; set; }
        public String DestinationPath
        {
            get
            {
                return _destinationPath ?? (_destinationPath = String.Empty);
            }
            set
            {
                _destinationPath = value;
                OnPropertyChanged("DestinationPath");
            }
        }
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
            ReadSettings(FILEPATH);
            backupProcedure = new BackupProcedure();
        }

        public void HandleError(Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(String.Format("{0}\n\n{1}", ex.Message, ex.StackTrace));
            });
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
            if (!String.IsNullOrEmpty(SelectedPath))
                BackupDirectories.Remove(SelectedPath);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FILEPATH))
                File.Delete(FILEPATH);
            using (TextWriter tw = new StreamWriter(FILEPATH))
            {
                if (!String.IsNullOrEmpty(TextBoxPath.Text))
                    tw.WriteLine("1:" + TextBoxPath.Text);
                if (!String.IsNullOrEmpty(TextBoxUsername.Text))
                    tw.WriteLine("2:" + TextBoxUsername.Text);
                if (!String.IsNullOrEmpty(TextBoxPassword.Text))
                    tw.WriteLine("3:" + TextBoxPassword.Text);
                foreach (String s in BackupDirectories)
                    tw.WriteLine("4:" + s);
                tw.Close();
            }
        }

        public void ReportStatus(String info)
        {
            Dispatcher.Invoke(() =>
            {
                LabelStatus.Content = info;
            });
        }
        public void ReportProgress(int completed, int total)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBarStatus.Value = (double)completed / (double)total * 100;
            });
        }

        private async void ButtonBackup_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (BackupDirectories == null || BackupDirectories.Count == 0)
                return;
            LabelStatus.Content = String.Empty;
            if (ButtonBackup.Content.Equals("Backup"))
            {
                ButtonBackup.Content = "Cancel";
                try
                {
                    String source = BackupDirectories[0].Split('\\')[0];
                    String path = TextBoxPath.Text;
                    if (!path.EndsWith("\\"))
                        path += "\\";
                    path += System.Environment.MachineName + "\\";
                    path += DateTime.Now.ToString("yyyyMMdd") + "\\";
                    await Task.Run(() => backupProcedure.ExecuteBackupProcedureAsync(this, source, path, BackupDirectories));
                    ButtonBackup.Content = "Backup";
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
            else
            {
                ButtonBackup.Content = "Backup";
            }
        }
    }
}
