using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Support class with useful utils
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Get menu path of given method
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string PathOfMethod(Action action)
        {
            var methodInfo = action.Method;
            var attr = methodInfo.GetCustomAttribute<MenuItemHandlerAttribute>();
            if (attr == null)
                throw new Exception(methodInfo.Name + "is not a menu item");
            return attr.Path;

        }
    }
}
