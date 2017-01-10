using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeDsm
{
    public class CursorWait : IDisposable
    {
        public CursorWait(bool appStarting = false, bool applicationCursor = false)
        {
            // Wait
            Cursor.Current = appStarting ? Cursors.AppStarting : Cursors.WaitCursor;
            if (applicationCursor)
                Application.UseWaitCursor = true;
        }

        public void Dispose()
        {
            // Reset
            Cursor.Current = Cursors.Default;
            Application.UseWaitCursor = false;
        }
    }
}
