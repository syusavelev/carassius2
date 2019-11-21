using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core;

namespace Carassius
{
    /// <summary>
    /// The wrapper class for menu item
    /// </summary>
    public class MenuItemWrapper
    {
        /// <summary>
        /// Special value of priority, that indicates, that priority should be calculated as average priority of children
        /// </summary>
        public const int PRIORITY_RECALC = Int32.MinValue;
        /// <summary>
        /// Is item clickable or it is just intermediate node
        /// </summary>
        public bool IsClickable
        {
            get { return Attr != null; }
        }
        /// <summary>
        /// Is item enabled or disabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        /// The attribute of menu item
        /// </summary>
        public MenuItemHandlerAttribute Attr { get; set; }
        /// <summary>
        /// Method that is called when menu item is clicked
        /// </summary>
        public Action Action { get; set; }
        /// <summary>
        /// The name of the item (the displayed text and path node)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The priority of the item in menu
        /// </summary>
        public int Priority { get; set; } = 20;
        /// <summary>
        /// Nested elements ofs the item
        /// </summary>
        public Dictionary<string, MenuItemWrapper> Children { get; set; } = new Dictionary<string, MenuItemWrapper>();

        public MenuItemWrapper()
        {
        }

        public MenuItemWrapper(MenuItemHandlerAttribute attr, Action action)
        {
            Action = action;
            Attr = attr;
            Name = attr.Path.Split('/').Last();
            Priority = attr.Priority;
        }
        /// <summary>
        /// Recalculate priority if needed and recalculate priority of the children
        /// </summary>
        public void RecalculatePriority()
        {
            int sum = 0;
            foreach (var ch in Children)
            {
                ch.Value.RecalculatePriority();
                sum += ch.Value.Priority;
            }
            if (Priority == PRIORITY_RECALC)
            {
                Priority = sum / Children.Count;
            }

        }
        /// <summary>
        /// Convert wrapper into wpf menu item
        /// </summary>
        /// <returns></returns>
        public MenuItem ToMenuItem()
        {
            var self = new MenuItem();
            self.IsEnabled = IsEnabled;
            self.Header = HandleItemName(Name);
            if (IsClickable)
                self.Click += delegate (object sender, RoutedEventArgs args) { Action(); };

            foreach (var kv in Children.OrderBy(t => t.Value.Priority))
            {
                var mi = kv.Value.ToMenuItem();
                self.Items.Add(mi);
            }

            return self;
        }
        /// <summary>
        /// Modify item name to become more pretty (make the first letter uppercase)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string HandleItemName(string name)
        {
            return name.First().ToString().ToUpper() + name.Substring(1);
        }
        /// <summary>
        /// Get subitems as wpf menu items
        /// </summary>
        /// <returns></returns>
        public List<MenuItem> ToMenu()
        {
            return new List<MenuItem>(Children.OrderBy(t => t.Value.Priority).Select(t => t.Value.ToMenuItem()));
        }
        /// <summary>
        /// Add new menu item wrapper as child
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        public void PlaceNewItem(MenuItemWrapper item, string path)
        {
            var queue = new Queue<string>(path.Split('/'));
            PlaceNewItem(item, queue);
        }
        /// <summary>
        ///// Add new menu item wrapper as child
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        public void PlaceNewItem(MenuItemWrapper item, Queue<string> path)
        {
            if (this.IsClickable)
            {
                throw new Exception("Cannot add item to a clickable item");
            }
            string location = path.Dequeue();
            if (path.Count == 0)
            {
                //this is last part of path, child of this
                Children[location] = item;
            }
            else
            {
                if (Children.ContainsKey(location))
                {
                    var child = Children[location];
                    child.PlaceNewItem(item, path);
                }
                else
                {
                    var child = new MenuItemWrapper()
                    {
                        Name = location,
                        Priority = MenuItemWrapper.PRIORITY_RECALC
                    };
                    Children[location] = child;
                    child.PlaceNewItem(item, path);
                }
            }
        }

    }
}
