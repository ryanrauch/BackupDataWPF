using System;
using System.Collections.Generic;

namespace BackupDataWPF
{
    public interface IBackupProcedure
    {
        bool StillWorking { get; set; }
        void ExecuteBackupProcedureAsync(MainWindow gui, String source, String destination, IList<String> directories);
    }
}