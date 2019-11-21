using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace PNEditorEditView.ImportExport
{
    public static class BitmapExporter
    {
        private const string MODELISEMPTY = "Current model is empty!";
        private const string FILEFILTER = "PNG Images(*.png) | *.png";

        /// <summary>
        /// Save current model from the canvas as a PNG file
        /// </summary>
        /// <param name="canvas"></param>
        public static void SaveModelAsAPicture(Canvas canvas, List<PetriNetNode> figures)
        {
            if (figures.Count != 0)
            {
                //todo magic numbers
                var canvasBitmap = new RenderTargetBitmap((int)canvas.ActualWidth,
                                                          (int)canvas.ActualHeight,
                                                          96d,
                                                          96d,
                                                          PixelFormats.Pbgra32);

                canvasBitmap.Render(canvas);

                double minX = figures[0].CoordX,
                       maxX = figures[0].CoordX,
                       minY = figures[0].CoordY,
                       maxY = figures[0].CoordY;

                foreach (var figure in figures)
                {
                    if (figure.CoordX < minX)
                        minX = figure.CoordX;
                    if (figure.CoordX > maxX)
                        maxX = figure.CoordX;

                    if (figure.CoordY < minY)
                        minY = figure.CoordY;
                    if (figure.CoordY > maxY)
                        maxY = figure.CoordY;
                }

                // Crop canvas field           

                var x1 = minX > 10 ? minX - 10 : 0;
                var y1 = minY > 10 ? minY - 10 : 0;

                var piece = new CroppedBitmap(canvasBitmap, new Int32Rect((int)x1, (int)y1,
                                              (int)(maxX - minX + 50),
                                              (int)(maxY - minY + 80)));

                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(piece));

                var sFdialog = new SaveFileDialog {Filter = FILEFILTER};

                if (sFdialog.ShowDialog() != true) return;

                var file = File.Create(sFdialog.FileName);
                pngEncoder.Save(file);
                file.Close();
            }
            else MessageBox.Show(MODELISEMPTY);
        }
    }
}
