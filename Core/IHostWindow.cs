using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Interface for interacting with host window
    /// </summary>
    public interface IHostWindow
    {
        /// <summary>
        /// Sends command to window to disable/enable menu item with a given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="enabled"></param>
        void SetMenuItemEnabled(string path, bool enabled);
    }
}
