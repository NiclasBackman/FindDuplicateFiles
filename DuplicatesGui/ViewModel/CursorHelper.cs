using System;

namespace DuplicatesGui.ViewModel
{
    class CursorHelper : IDisposable
    {
        private DuplicatesViewModel viewModel;
        private bool disposed;

        public CursorHelper(DuplicatesViewModel vm)
        {
            viewModel = vm;
            vm.Cursor = System.Windows.Input.Cursors.Wait;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    viewModel.Cursor = System.Windows.Input.Cursors.Arrow;
                }

                // Note disposing has been done.
                disposed = true;
            }
        }
    }
}
