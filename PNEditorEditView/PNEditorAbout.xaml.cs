using System.Windows;

namespace PNEditorEditView
{
    /// <summary>
    /// Interaction logic for PNEditorAbout.xaml
    /// </summary>
    public partial class PNEditorAbout
    {
        public PNEditorAbout()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://pais.hse.ru/research/projects/carassius");
        }
    }
}
