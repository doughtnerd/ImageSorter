using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    interface ISorterView
    {
        event Action<string> SourceChosenEvent;
        event Action<string> TargetChosenEvent;
        event Action<int> ThreadsChosenEvent;
        event Action<bool> CopyFilesEvent;
        event Action<bool> AllowDuplicatesEvent;
        event Action StartEvent;
        event Action CancelEvent;

        void ResetControls();
        void DisableControls();

        void Invoke(Action action);

    }
}
