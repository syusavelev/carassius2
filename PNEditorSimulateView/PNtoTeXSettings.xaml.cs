using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PNEditorSimulateView
{
    /// <summary>
    /// Author: Nikolay Chuykin
    /// Supervisor: Alexey Mitsyuk
    /// PAIS Lab, 2014
    /// </summary>
    public partial class PNtoTeXSettings 
    {
        const string SELECTFILEPATH = "Select file path";
        const string PUTBEFORE = "%Put this in preamble:";
        const string INCORRECTWIDTH = "Incorrect width!";

        public PNtoTeXSettings()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            PNtoTeX.Normalize();
            NetCanvasWidth.Text = (PNtoTeX.MaxX / PNtoTeX.koefX).ToString("f3");
            NetCanvasHeight.Text = (PNtoTeX.MaxY / PNtoTeX.koefY).ToString("f3");
            CenterNetAlign.IsChecked = true;
            Save.IsEnabled = false;

            PreambleText.Text = PUTBEFORE + "\n" + PNtoTeX.Implementation();
        }

        private void LeftChecked(object sender, RoutedEventArgs e)
        {
            Result.Text += PNtoTeX.Result();
        }

        private void CenterChecked(object sender, RoutedEventArgs e)
        {
            Result.Text += PNtoTeX.CResult();
        }

        private void RightChecked(object sender, RoutedEventArgs e)
        {
            Result.Text += PNtoTeX.RResult();
        }

        private void WidthChedked(object sender, RoutedEventArgs e)
        {
            NetCanvasWidth.IsEnabled = true;
        }

        private void WidthUnchecked(object sender, RoutedEventArgs e)
        {
            NetCanvasWidth.IsEnabled = false;
            if (HChose.IsChecked == true)
                PNtoTeX.koefX = PNtoTeX.koefY;
            else
                PNtoTeX.koefX = PNtoTeX.koefY = 50;
            NetCanvasWidth.Text = (PNtoTeX.MaxX / PNtoTeX.koefX).ToString("f3");
            NetCanvasHeight.Text = (PNtoTeX.MaxY / PNtoTeX.koefY).ToString("f3");
        }

        private void UpdateResult()
        {


            if (RightNetAlign.IsChecked == true)
                //Result.Text = PUTBEFORE + "\n\n" + PNtoTeX.Implementation() + "\n --- \n" + PNtoTeX.RResult();
                Result.Text = PNtoTeX.RResult();
            else
                if (CenterNetAlign.IsChecked == true)
                    //Result.Text = PUTBEFORE + "\n\n" + PNtoTeX.Implementation() + "\n --- \n" + PNtoTeX.CResult();
                    Result.Text = PNtoTeX.CResult();
                else
                    //Result.Text = PUTBEFORE + "\n\n" + PNtoTeX.Implementation() + "\n --- \n" + PNtoTeX.Result();
                    Result.Text = PNtoTeX.Result();

        }

        private void HeightChedked(object sender, RoutedEventArgs e)
        {
            NetCanvasHeight.IsEnabled = true;
        }

        private void HeightUnchecked(object sender, RoutedEventArgs e)
        {
            NetCanvasHeight.IsEnabled = false;
            if (WChose.IsChecked == true)
                PNtoTeX.koefY = PNtoTeX.koefX;
            else
                PNtoTeX.koefX = PNtoTeX.koefY = 50;
            NetCanvasWidth.Text = (PNtoTeX.MaxX / PNtoTeX.koefX).ToString("f3");
            NetCanvasHeight.Text = (PNtoTeX.MaxY / PNtoTeX.koefY).ToString("f3");

        }

        private void WChanged(object sender, TextChangedEventArgs e)
        {
            double w;
            if (!double.TryParse(NetCanvasWidth.Text, out w))
            {
                Result.Text = INCORRECTWIDTH;
                return;
            }
            PNtoTeX.koefX = PNtoTeX.MaxX / w;
            if (HChose.IsChecked == false)
            {
                PNtoTeX.koefY = PNtoTeX.koefX;
                NetCanvasHeight.Text = (PNtoTeX.MaxY / PNtoTeX.koefY).ToString("f3");
            }

        }

        private void HChanged(object sender, TextChangedEventArgs e)
        {
            double h;
            if (!double.TryParse(NetCanvasHeight.Text, out h))
            {
                Result.Text = INCORRECTWIDTH;
                return;
            }
            PNtoTeX.koefY = PNtoTeX.MaxY / h;
            if (WChose.IsChecked == false)
            {
                PNtoTeX.koefX = PNtoTeX.koefY;
                NetCanvasWidth.Text = (PNtoTeX.MaxX / PNtoTeX.koefX).ToString("f3");
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            double d, w;
            if (double.TryParse(NetCanvasWidth.Text, out w) && double.TryParse(NetCanvasHeight.Text, out d))
                UpdateResult();
        }

        private void StartDialog(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog Dialog = new Microsoft.Win32.SaveFileDialog();
            //System.Windows.Forms.SaveFileDialog Dialog = new System.Windows.Forms.SaveFileDialog();
            Dialog.Filter = "TeX file|*.tex";
            ChosenPath.Content = SELECTFILEPATH;
            Save.IsEnabled = false;
            Dialog.FileOk += Dialog_FileOk;
            Dialog.CreatePrompt = false;
            Dialog.ShowDialog();
        }

        void Dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save.IsEnabled = true;
            ChosenPath.Content = ((Microsoft.Win32.SaveFileDialog)sender).FileName;
            SaveFullDocument(((Microsoft.Win32.SaveFileDialog)sender).FileName);
        }

        public void SaveFullDocument(string path)
        {
            string s = @"\documentclass[a4paper]{article}" + "\n";
            s += PNtoTeX.Implementation() + "\n";
            s += @"\begin{document}" + "\n\n";
            s += RightNetAlign.IsChecked == true ? PNtoTeX.RResult() :
                 CenterNetAlign.IsChecked == true ? PNtoTeX.CResult() : PNtoTeX.Result();
            s += "\n" + @"\end{document}";

            using (StreamWriter Writer = new StreamWriter(path))
            {
                Writer.Write(s);
            }

        }

        private void Save_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFullDocument(ChosenPath.Content.ToString());
        }
    }

}
