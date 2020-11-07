using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DuplicatesLib
{
    public interface IDuplicateFinder
    {
        List<SingleFileEntry> QueryDuplicates(BackgroundWorker bgw, DoWorkEventArgs eventArgs, string path, string filter = "*.*");
    }
}
