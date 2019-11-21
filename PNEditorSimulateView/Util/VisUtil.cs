using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PNEditorSimulateView.Util
{
    public static class VisUtil
    {
        public static void SetCoordinatesOfLine(Line lineVisible, VArc arc)
        {
            var from = arc.From;
            var to = arc.To;
            var tempX1 = @from.CoordX;
            var tempX2 = to.CoordX;
            var tempY1 = @from.CoordY;
            var tempY2 = to.CoordY;

            if (tempX2 > tempX1)
            {
                if (to is VPlace)
                {
                    lineVisible.X1 = tempX1 + 20;
                    lineVisible.Y1 = tempY1 + 25;
                    lineVisible.Y2 = tempY2 + 15;
                }
                else
                {
                    lineVisible.X1 = tempX1 + 30;
                    lineVisible.Y1 = tempY1 + 15;
                    lineVisible.Y2 = tempY2 + 25;
                }
                lineVisible.X2 = tempX2;
            }
            else
            {
                lineVisible.X1 = tempX1;
                if (to is VPlace)
                {
                    lineVisible.Y1 = tempY1 + 25;
                    lineVisible.X2 = tempX2 + 30;
                    lineVisible.Y2 = tempY2 + 15;
                }
                else
                {
                    lineVisible.Y1 = tempY1 + 15;
                    lineVisible.X2 = tempX2 + 20;
                    lineVisible.Y2 = tempY2 + 25;
                }
            }
        }

        public static void ResizeCanvas(List<PetriNetNode> figures, PNEditorControl control, Canvas canvas)
        {
            var xMargin = control.ActualWidth - control.GridMainField.ActualWidth + 20;
            var yMargin = control.ActualHeight - control.GridMainField.ActualHeight + 40;

            if (figures.Count == 0) return;

            var maxX = figures[0].CoordX;
            var maxY = figures[0].CoordY;

            foreach (var figure in figures)
            {
                if (figure.CoordX > maxX)
                    maxX = figure.CoordX;
                if (figure.CoordY > maxY)
                    maxY = figure.CoordY;
            }

            double canvasHeight, canvasWidth;
            if (maxY + 50 >= control.ActualHeight - yMargin)
            {
                canvasHeight = maxY + 120;
            }
            else
            {
                canvasHeight = control.ActualHeight - yMargin;
            }
            if (maxX + 30 >= control.ActualWidth - xMargin)
            {
                canvasWidth = maxX + 80;
            }
            else
            {
                canvasWidth = control.ActualWidth - xMargin;
            }

            canvas.Height = canvasHeight;
            canvas.Width = canvasWidth;
        }

        public static void UpdateLineCoordinates(Line line, double x1, double x2, double y1, double y2)
        {
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
        }

        public static Line GetLineFromPath(System.Windows.Shapes.Path path)
        {
            var tempLine = new Line();
            var geom = path.Data as PathGeometry;
            var segment = geom.Figures[0].Segments[0] as BezierSegment;
            tempLine.X1 = segment.Point2.X;
            tempLine.Y1 = segment.Point2.Y;
            tempLine.X2 = segment.Point3.X;
            tempLine.Y2 = segment.Point3.Y;
            return tempLine;
        }
    }
}
