using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace PNEditorEditView
{
    /// <summary>
    /// Author: Nikolay Chuykin
    /// Supervisor: Alexey Mitsyuk
    /// PAIS Lab, 2014
    /// 
    /// PGF/TikZ is used here
    /// </summary>
    class PNtoTeX
    {
        private static double
            minX = double.PositiveInfinity,
            minY = double.PositiveInfinity,
            maxX = double.NegativeInfinity,
            maxY = double.NegativeInfinity;

        public static double MinX { get { return minX; } }
        public static double MinY { get { return minY; } }
        public static double MaxX { get { return maxX; } }
        public static double MaxY { get { return maxY; } }

        public static double koefX = 50, koefY = 50;


        static List<VTransition> TrList = new List<VTransition>();
        static List<VPlace> PlList = new List<VPlace>();

        #region Preprocessing

        /// <summary>
        // Нормирование 
        /// </summary>      
        public static void Normalize()
        {
            TrList.Clear();
            PlList.Clear();
            //foreach (Transition transition in Transition.transitions)
            foreach (VTransition transition in PNEditorControl.Net.transitions)
            {
                minX = Math.Min(minX, transition.CoordX);
                minY = Math.Min(minY, transition.CoordY);
                maxX = Math.Max(maxX, transition.CoordX);
                maxY = Math.Max(maxY, transition.CoordY);
            }
            //foreach (Place place in Place.places)
            foreach (VPlace place in PNEditorControl.Net.places)
            {
                minX = Math.Min(minX, place.CoordX);
                minY = Math.Min(minY, place.CoordY);
                maxX = Math.Max(maxX, place.CoordX);
                maxY = Math.Max(maxY, place.CoordY);
            }

            //foreach (Place p in Place.places)
            foreach (VPlace p in PNEditorControl.Net.places)
            {
                //Place place = new Place(p.CoordX - minX, p.CoordY - minY - 10);
                var place = VPlace.Create(p.CoordX - minX, p.CoordY - minY - 10);
                place.Id = p.Id;
                place.Label = p.Label;
                place.NumberOfTokens = p.NumberOfTokens;
                place.ThisArcs = p.ThisArcs;
                PlList.Add(place);
            }

            //foreach (Transition t in Transition.transitions)
            foreach (VTransition t in PNEditorControl.Net.transitions)
            {
                //Transition transition = new Transition(t.CoordX - minX, t.CoordY - minY);
                var transition = VTransition.Create(t.CoordX - minX, t.CoordY - minY);
                transition.Id = t.Id;
                transition.Label = t.Label;
                transition.ThisArcs = t.ThisArcs;
                TrList.Add(transition);
            }
        }


        #endregion

        #region Output in file

        private static string GenerateHeader()
        {
            string s = @"[bend angle=45,node distance=0cm,auto,
  pre/.style={<-,shorten <=1pt,>=stealth’,semithick},
  post/.style={->,shorten >=2pt,>=stealth’,semithick}]
  
  \tikzstyle{place}=[circle,thick,draw=black!90,minimum size=" + Convert.ToString(300 / koefX, CultureInfo.GetCultureInfo("en-US")) + @"mm]
  \tikzstyle{transition}=[rectangle ,thick,draw=black!75,
  			  minimum height=" + Convert.ToString(500 / koefY, CultureInfo.GetCultureInfo("en-US")) + @"mm,minimum width = " + Convert.ToString(200 / koefX, CultureInfo.GetCultureInfo("en-US")) + @"mm]";
            return s;
        }

        private static string GenerateLatexTransition(VTransition transition)
        {
            return @"\node [transition,label=-90:" + transition.Label + "] (" + transition.Id + ") at (" + Convert.ToString((transition.CoordX / koefX),
                CultureInfo.GetCultureInfo("en-US")) + "," +
                Convert.ToString((-(transition.CoordY / koefY)), CultureInfo.GetCultureInfo("en-US")) + ") {};";
        }

        private static string GenerateLatexPlace(VPlace place)
        {
            return @"\node [place,label=-90:" + place.Label + ", tokens=" + ((place.NumberOfTokens > 4) ? 0 : place.NumberOfTokens) + "] (" + place.Id + ") at (" + Convert.ToString((place.CoordX / koefX),
                CultureInfo.GetCultureInfo("en-US")) + "," +
                Convert.ToString((-(place.CoordY / koefY)), CultureInfo.GetCultureInfo("en-US")) + ") {" + ((place.NumberOfTokens <= 4) ? "" :
                place.NumberOfTokens.ToString()) + "}" + AddAllArcs(place) + ";";
        }

        private static string AddAllArcs(VPlace place)
        {
            string s = "";
            foreach (var i in place.ThisArcs)
            {
                if (i.From.Id == place.Id)
                    s += "\n\t edge[post] node[auto,swap] {" + ((i.Weight == "1") ? "" : i.Weight) + "} (" + i.To.Id + ")";
                else
                    s += "\n\t edge[pre] node[auto,swap] {" + ((i.Weight == "1") ? "" : i.Weight) + "} (" + i.From.Id + ")";
            }

            return s;
        }

        #endregion


        public static void Transform()
        {
            Normalize();
            #region Text for transitions and places

            File.Delete("1.txt");

            using (
            StreamWriter writer = new StreamWriter("1.txt"))
            {
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                foreach (var i in TrList)
                    writer.WriteLine(GenerateLatexTransition(i));
                foreach (var i in PlList)
                    writer.WriteLine(GenerateLatexPlace(i));
                writer.Flush();
                writer.Close();
            }
            #endregion
        }

        public static string Implementation()
        {
            return @"\usepackage{pgf}
\usepackage{tikz}
\usetikzlibrary{petri}";
        }

        public static string Result()
        {
            string s = @"\begin{tikzpicture} [auto]";
            s += "\n" + GenerateHeader() + "\n";
            s += @"\begin{scope}" + "\n";
            foreach (var i in TrList)
                s += GenerateLatexTransition(i) + "\n";
            foreach (var i in PlList)
                s += GenerateLatexPlace(i) + "\n";
            s += @"\end{scope}" + "\n";
            s += @"\end{tikzpicture}";
            return s;
        }

        public static string CResult()
        {
            string s = @"\begin{center}";
            s += "\n" + Result() + "\n" + @"\end{center}";
            return s;
        }

        public static string RResult()
        {
            string s = @"\begin{flushright}";
            s += "\n" + Result() + "\n" + @"\end{flushright}";
            return s;
        }

    }
}
