using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNEditorSimulateView.Util
{
    public static class MathUtil
    {
        public static void SearchPointOfIntersection(double x0, double y0, double x1, double y1, out double x, out double y)
        {
            while (true)
            {
                var x_0 = x0;
                var y_0 = y0;
                var x_1 = x1;
                var y_1 = y1;
                var sqr = Math.Pow((x1 - x0), 2) + Math.Pow((y1 - y0), 2);
                var a = sqr / Math.Pow((x1 - x0), 2);
                if (double.IsInfinity(a))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double b = -2*sqr*x0/Math.Pow((x1 - x0), 2);
                if (double.IsInfinity(b))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double c = x0*x0*sqr/Math.Pow((x1 - x0), 2) - 225;
                if (double.IsInfinity(c))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double d = b*b - 4*a*c;
                double firstX = (-b + Math.Sqrt(d))/2/a;
                if (double.IsInfinity(firstX))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double secondX = (-b - Math.Sqrt(d))/2/a;
                if (double.IsInfinity(secondX))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double firstY = (firstX - x0)*(y1 - y0)/(x1 - x0) + y0;
                if (double.IsInfinity(firstY))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double secondY = (secondX - x0)*(y1 - y0)/(x1 - x0) + y0;
                if (double.IsInfinity(secondY))
                {
                    x_0 += 0.001;
                    x0 = x_0;
                    y0 = y_0;
                    x1 = x_1;
                    y1 = y_1;
                    continue;
                }
                double firstDistance = Math.Sqrt(Math.Pow(x1 - firstX, 2) + Math.Pow(y1 - firstY, 2));
                double secondDistance = Math.Sqrt(Math.Pow(x1 - secondX, 2) + Math.Pow(y1 - secondY, 2));
                if (firstDistance < secondDistance)
                {
                    x = firstX;
                    y = firstY;
                }
                else
                {
                    x = secondX;
                    y = secondY;
                }
                break;
            }
        }
    }
}
