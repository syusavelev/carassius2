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
    /// <summary>
    /// Interaction logic for LabelProperty.xaml
    /// </summary>
    public partial class LabelProperty : PropertyEditorBase
    {
        public LabelProperty()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            if(_item == null)
                return;
            TbLabel.Text = CastItem<PetriNetNode>().Label;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(VTransition);
                yield return typeof(VPlace);
            }
        }

        private void TbLabel_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ApplyValue();
        }

        private void TbLabel_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                RevertValue();
            if(e.Key == Key.Enter)
                ApplyValue();
        }

        void ApplyValue()
        {
            var node = CastItem<PetriNetNode>();
            _mainControl.ChangeLabelWithCommand(node, TbLabel.Text);
        }

        void RevertValue()
        {
            Refresh();
        }
    }
}
