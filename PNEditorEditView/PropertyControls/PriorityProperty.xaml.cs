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
    /// Interaction logic for NumberOfTokensProperty.xaml
    /// </summary>
    public partial class PriorityProperty : PropertyEditorBase
    {
        public PriorityProperty()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            if(_item == null)
                return;
            TbPriority.Text = CastItem<VTransition>().Priority.ToString();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { yield return typeof(VTransition); }
        }

        private void TbLabel_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if(ValidateValue())
                ApplyValue();
            else
                RevertValue();
        }

        private void TbLabel_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                RevertValue();
            if (e.Key == Key.Enter)
            {
                if(ValidateValue())
                    ApplyValue();
            }
                
        }

        bool ValidateValue()
        {
            int res;
            if (!int.TryParse(TbPriority.Text, out res))
            {
                MessageBox.Show("Entered value is not int");
                return false;
            }
            
            return res>=0;
        }
        void ApplyValue()
        {
            _mainControl.ChangePriorityWithCommand(CastItem<VTransition>(), int.Parse(TbPriority.Text));
        }

        void RevertValue()
        {
            Refresh();
        }

        private void TbTokens_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                try
                {
                    TbPriority.Text = (int.Parse(TbPriority.Text) + 1).ToString();

                }
                catch
                {
                }
            }
            if (e.Key == Key.Down)
            {
                try
                {
                    TbPriority.Text = Math.Max(int.Parse(TbPriority.Text) - 1,0).ToString();

                }
                catch
                {
                }
            }
        }
    }
}
