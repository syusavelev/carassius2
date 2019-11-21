using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace Core
{
    /// <summary>
    /// The main controller of the application
    /// </summary>
    public class MainController
    {
        #region singleton
        private static MainController _self;
        public static MainController Self
        {
            get
            {
                if (_self == null)
                    _self = new MainController();
                return _self;
            }
        }
        private MainController() { }
        #endregion
        /// <summary>
        /// The model of the application
        /// </summary>
        public PetriNet Net { get; set; } = new PetriNet();

        /// <summary>
        /// The host windo (is set by host window on initialization)
        /// </summary>
        public IHostWindow HostWindow { get; set; }
        
        /// <summary>
        /// Changes enabled property of a given menu item (defined by a path)
        /// </summary>
        /// <param name="pathOfMethod"></param>
        /// <param name="enabled"></param>
        public void SetMenuItemEnabled(string pathOfMethod, bool enabled)
        {
            HostWindow.SetMenuItemEnabled(pathOfMethod, enabled);
        }
    }
}
