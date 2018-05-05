using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackupDataWPF
{
    public class BackupProcedure : IBackupProcedure
    {
        private Boolean _stillWorking { get; set; }
        private IList<String> _exclusions { get; set; }

        private Boolean DirectoryCopy(MainWindow gui, String sourceDirName, String destDirName)
        {
            if (!_stillWorking)
                return true;
            if (IsDirectoryExcluded(sourceDirName))
                return true;
            gui.ReportStatus(sourceDirName);
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                return false;
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!_stillWorking)
                    return true;
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                if (!_stillWorking)
                    return true;
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(gui, subdir.FullName, temppath);
            }
            return true;
        }

        public void CancelBackupProcedureAsync()
        {
            _stillWorking = false;
        }

        public void ExecuteBackupProcedureAsync(MainWindow gui, String source, String destination, IList<String> directories, IList<String> exclusions)
        {
            _stillWorking = true;
            _exclusions = exclusions;
            int complete = 0,
                total = directories.Count;
            gui.ReportProgress(complete, total);
            foreach(String dir in directories)
            {
                if (!_stillWorking)
                    break;
                if (!DirectoryCopy(gui, dir, dir.Replace(source, destination)))
                    break;
                gui.ReportProgress(++complete, total);
            }
            gui.ReportStatus("Completed - " + DateTime.Now.ToString());
            _stillWorking = false;
        }

        private Boolean IsDirectoryExcluded(String source)
        {
            if (_exclusions != null && _exclusions.Any(e => e.Equals(source, StringComparison.OrdinalIgnoreCase)))
                return true;
            return false;
        }

        public BackupProcedure()
        {
            _stillWorking = false;
        }
    }
}
