using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemToolbarIE
{
    /// <summary>
    /// Base class for IE toobar elements.
    /// </summary>
    internal abstract class BaseToolbarItem : IToolbarItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal BaseToolbarItem(Toolbar engine, System.Drawing.Image img)
        {
            this.image = img;
            this.engine = engine;
            items = new List<ToolStripItem>();
        }

        protected Toolbar engine;
        protected List<ToolStripItem> items;

        #region IToolbarItem Members

        /// <summary>
        /// Visibility indicator
        /// </summary>
        public virtual bool Visible
        {
            get
            {
                if (engine == null || engine.ToolStrip == null || engine.ToolStrip.IsDisposed) return false;
                foreach (ToolStripItem item in items)
                {
                    if (item.Visible) return true;
                }

                return false;
            }
            set
            {
                if (engine == null || engine.ToolStrip == null || engine.ToolStrip.IsDisposed) return;
                foreach (ToolStripItem item in items)
                {
                    item.Visible = value;
                }
            }
        }

        /// <summary>
        /// Type identifier.
        /// </summary>
        public abstract ToolbarItemType TypeID
        {
            get;
        }

        virtual public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        Image image = null;



        /// <summary>
        /// State cleanup
        /// </summary>
        public virtual void Reset()
        {
            if (!(engine == null || engine.ToolStrip == null || engine.ToolStrip.IsDisposed))
            {
                foreach (ToolStripItem item in items)
                {
                    if (item.IsDisposed) continue;
                    engine.ToolStrip.Items.Remove(item);
                    item.Dispose();
                }
                items.Clear();
            }
        }

        #endregion

        #region IDisposable Members

        bool disposed = false;
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Reset();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;			
            }
            disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~BaseToolbarItem()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }


        #endregion
    }
}
