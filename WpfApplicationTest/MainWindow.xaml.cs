using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Core;
using PNEditorEditView;

namespace Carassius
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow:IHostWindow
    {
        public MainWindow()
        {
            MainController.Self.HostWindow = this;
            PNEditorControl.ShowMainWindowTitleDelegate = ShowTitle;
            PNEditorSimulateView.PNEditorControl.ShowMainWindowTitleDelegate = ShowTitle;

        }

        private Dictionary<string, bool> MenuEnabledState = new Dictionary<string, bool>();
        public void SetMenuItemEnabled(string path, bool enabled)
        {
            MenuEnabledState[path] = enabled;
            RefreshMenu();
        }

        private IView activeView;
        public void ShowTitle(string name)
        {
            PetriNetEditorWindow.Title = name;
        }

        private void ActiveView(IView view)
        {
            activeView.Deactivate();
            activeView = view;
            EditControl.Visibility = Visibility.Hidden;
            SimulateControl.Visibility = Visibility.Hidden;
            if (view == EditControl)
                EditControl.Visibility = Visibility.Visible;
            else
            {
                SimulateControl.Visibility = Visibility.Visible;
            }
            activeView.Activate();
            RefreshMenu();
        }
        private void PetriNetEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO: handle changed file
            //if (PNEditorControl.Net.Nodes.Count != 0)
            //{
            //    PNEditorControl.IsWindowClosing = true;
            //    UserControl11.MenuNew_Click(new object(), new RoutedEventArgs());
            //}
            //if (PNEditorControl.IsCancelPressed)
            //    e.Cancel = true;
            //else EditControl.StopStopWatch();
        }        

        private void PetriNetEditorWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            activeView.UserControlKeyUp(sender, e);
            //UserControl11.DisableRedoButton();
            //if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            //{
            //    PNEditorControl.isCtrlPressed = false;
            //}
        }

        private void PetriNetEditorWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            activeView.UserControlKeyDown(sender, e);
        }
        private void PetriNetEditorWindow_Initialized(object sender, System.EventArgs e)
        {
            activeView = EditControl;
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            var stat = GetStaticMenu();
            AddActiveViewMenuItems(stat, activeView);
            stat.RecalculatePriority();
            var m = stat.ToMenu();
            MainMenu.Items.Clear();
            foreach (var mItem in m)
            {
                MainMenu.Items.Add(mItem);
            }
        }

        private MenuItemWrapper AddActiveViewMenuItems(MenuItemWrapper root, object view)
        {
            var type = view.GetType();
            foreach (var mi in type.GetMethods(BindingFlags.Public|BindingFlags.Instance))
            {
                var attr = mi.GetCustomAttribute<MenuItemHandlerAttribute>();
                if (attr == null)
                    continue;
                var menuItem = new MenuItemWrapper(attr, (Action)Delegate.CreateDelegate(typeof(Action),view, mi));
                menuItem.IsEnabled = GetMenuItemEnabled(attr.Path, attr.DefaultEnabled);
                root.PlaceNewItem(menuItem, attr.Path);
            }
            return root;
        }
        
        private MenuItemWrapper GetStaticMenu()
        {
            var root = new MenuItemWrapper();
            (string name, int priority)[] rootItemTitles = new (string, int)[]
            {
                ("file",1),
                ("edit",2),
                ("help", 99)
            };
            foreach (var q in rootItemTitles)
            {
                var child = new MenuItemWrapper()
                {
                    Name = q.name,
                    Priority = q.priority
                };
                root.PlaceNewItem(child, q.name);
            }
            //var ass = Assembly.GetAssembly(typeof(MainController));
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t=>t.GetTypes()))
            {
                foreach (var mi in type.GetMethods(BindingFlags.Public|BindingFlags.Static))
                {
                    var attr = mi.GetCustomAttribute<MenuItemHandlerAttribute>();
                    if(attr == null)
                        continue;
                    var menuItem = new MenuItemWrapper(attr, (Action) Delegate.CreateDelegate(typeof(Action), mi));
                    menuItem.IsEnabled = GetMenuItemEnabled(attr.Path, attr.DefaultEnabled);
                    root.PlaceNewItem(menuItem,attr.Path);

                }
            }

            return root;
        }

        private bool GetMenuItemEnabled(string path, bool defaultEnabled)
        {
            bool value;
            if (MenuEnabledState.TryGetValue(path, out value))
                return value;
            else
            {
                MenuEnabledState[path] = defaultEnabled;
                return defaultEnabled;
            }

        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            ActiveView(EditControl);
        }

        private void btSimulate_Click(object sender, RoutedEventArgs e)
        {
            ActiveView(SimulateControl);
        }
    }
}
