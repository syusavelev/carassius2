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
    /// Interaction logic for IdProperty.xaml
    /// </summary>
    public partial class IdProperty : PropertyEditorBase
    {
        public IdProperty()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            if(_item == null)
                return;

            var id = GetId();
            if (id != null)
                TbId.Text = id;

        }

        string GetId()
        {
            var id = CastItem<VArc>()?.Id;
            if (id == null)
                id = CastItem<VPlace>()?.Id;
            if (id == null)
                id = CastItem<VTransition>().Id;
            return id;
        }
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(VArc);
                yield return typeof(VPlace);
                yield return typeof(VTransition);

            }
        }
    }
}
