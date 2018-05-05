using System;
using System.Collections.Generic;

namespace BackupDataWPF
{
    public interface IBackupProcedure
    {
        void CancelBackupProcedureAsync();
        void ExecuteBackupProcedureAsync(MainWindow gui, String source, String destination, IList<String> directories, IList<String> exclusions);
    }
}