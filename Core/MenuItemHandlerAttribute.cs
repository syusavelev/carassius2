using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core

{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method )]
    public class MenuItemHandlerAttribute : System.Attribute
    {
        /// <summary>
        /// Path in menu, lowercase. Ex: file/exit
        /// </summary>
        public string Path;
        /// <summary>
        /// Defines the order menu items appear. The lower value, the higher is item displayed
        /// </summary>
        public int Priority = 20;
        /// <summary>
        /// Is given item enabled or disabled by default
        /// </summary>
        public bool DefaultEnabled = true;
        public MenuItemHandlerAttribute(string path)
        {
            Path = path;
        }

        public MenuItemHandlerAttribute(string path, int priority) : this(path)
        {
            Priority = priority;
        }

        public MenuItemHandlerAttribute(string path, int priority, bool defaultEnabled) : this(path, priority)
        {
            DefaultEnabled = defaultEnabled;
        }

        public MenuItemHandlerAttribute(string path, bool defaultEnabled) : this(path)
        {
            DefaultEnabled = defaultEnabled;
        }
    }
}
