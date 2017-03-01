using ImageSorter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    class Controller
    {
        private ISorterView view;
        private Model model;

        public Controller(ISorterView view)
        {
            this.view = view;
            this.model = new Model();
            view.SourceChosenEvent += HandleSourceChosen;
            view.TargetChosenEvent += HandleTargetChosen;
            view.ThreadsChosenEvent += HandleThreadsChosen;
            view.AllowDuplicatesEvent += HandleAllowDuplicates;
            view.CopyFilesEvent += HandleCopyFiles;
            view.StartEvent += HandleStart;
            view.CancelEvent += HandleCancel;
        }

        private void HandleSourceChosen(string s)
        {
            model.SourceFolderLocation = s;
        }

        private void HandleTargetChosen(string s)
        {
            model.TargetFolderLocation = s;
        }

        private void HandleThreadsChosen(int n)
        {
            model.NumberOfThreads = n;
        }

        private void HandleCopyFiles(bool b)
        {
            model.CopyFiles = b;
        }

        private void HandleAllowDuplicates(bool b)
        {
            model.AllowDuplicates = b;
        }

        private void HandleStart()
        {
            view.DisableControls();
            Task.Run(()=>DoSorting());
        }

        private void HandleCancel()
        {

        }

        private void DoSorting()
        {
            try
            {
                model.StartSorting();
                view.Invoke(()=> { view.ResetControls(); });
            }
            catch (OperationCanceledException)
            {
                view.Invoke(() => { view.ResetControls(); });
            }
        }
    }
}
