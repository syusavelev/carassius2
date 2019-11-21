using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PNEditorEditView.PropertyControls
{
    public abstract class PropertyEditorBase : UserControl
    {
        protected object _item;
        protected PNEditorControl _mainControl;

        public void BindMainControl(PNEditorControl mainControl)
        {
            _mainControl = mainControl;
        }
        public virtual void SetItem(object item)
        {
            _item = item;
            if (_item == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }
            if (CheckTypeSupported())
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                SetItem(null);
            }
            Refresh();
        }
        /// <summary>
        /// Refresh the property value from the model
        /// parse data from the object if needed
        /// </summary>
        public abstract void Refresh();

        protected T CastItem<T>() where T : class 
        {
            return _item as T;
        }

        public abstract IEnumerable<Type> SupportedTypes { get; }

        public virtual bool CheckTypeSupported()
        {
            if (_item == null)
                return false;
            return SupportedTypes.Contains(_item.GetType());
        }
    }
}
