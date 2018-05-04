using System;
using System.Collections.Generic;
using System.IO;

namespace BackupDataWPF
{
    public class BackupProcedure : IBackupProcedure
    {
        public Boolean StillWorking { get; set; }

        private Boolean DirectoryCopy(MainWindow gui, String sourceDirName, String destDirName)
        {
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
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(gui, subdir.FullName, temppath);
            }
            return true;
        }

        public void ExecuteBackupProcedureAsync(MainWindow gui, String source, String destination, IList<String> directories)
        {
            StillWorking = true;
            int complete = 0,
                total = directories.Count;
            gui.ReportProgress(complete, total);
            foreach(String dir in directories)
            {
                if (!StillWorking)
                    break;
                if (!DirectoryCopy(gui, dir, dir.Replace(source, destination)))
                    break;
                gui.ReportProgress(++complete, total);
            }
            StillWorking = false;
        }
        public BackupProcedure()
        {
            StillWorking = false;
        }
    }
}
