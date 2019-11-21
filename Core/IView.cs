using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Core
{
    /// <summary>
    /// Interface for views
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Called when given view is activated (gets focus)
        /// </summary>
        void Activate();
        /// <summary>
        /// Called when given view is deactivated (lost focus)
        /// </summary>
        void Deactivate();
        /// <summary>
        /// Called when key is down if component is active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControlKeyDown(object sender, KeyEventArgs e);
        /// <summary>
        /// Called when key is up if component is active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControlKeyUp(object sender, KeyEventArgs e);
    }
}
