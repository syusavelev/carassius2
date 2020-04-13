using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PNEditorReduction.Model;

namespace PNEditorReduction
{
    /// <summary>
    /// Author: Natalia Nikitina
    /// PAIS Lab, 2014
    /// </summary>
    public partial class PNEditorControl
    {
        private readonly Dictionary<object, PetriNetNode> _allFiguresObjectReferences = new Dictionary<object, PetriNetNode>();

        private object GetKeyByValueForFigures(PetriNetNode el)
        {
            return (from ex in _allFiguresObjectReferences where ex.Value.Equals(el) select ex.Key).FirstOrDefault();
        }

        public Shape GetKeyByValueForArcs(VArc arc, Dictionary<Shape, VArc> dictionary)
        {
            return (from ex in dictionary where ex.Value.Equals(arc) select ex.Key).FirstOrDefault();
        }

        public Dictionary<Shape, VArc> DictionaryForArcs = new Dictionary<Shape, VArc>();
        public Dictionary<Shape, VArc> DictionaryForArrowHeads1 = new Dictionary<Shape, VArc>();
        public Dictionary<Shape, VArc> DictionaryForArrowHeads2 = new Dictionary<Shape, VArc>();
        public Dictionary<PetriNetNode, Label> NodesToLabelsInCanvas = new Dictionary<PetriNetNode, Label>();

        public void RemoveNode(PetriNetNode node)
        {
            node.IsSelect = false;
            VPlace place = node as VPlace;
            if (place != null)
            {
                //Net.places.Remove(place);
                VPlace selPlace = place;
                MainModelCanvas.Children.Remove(selPlace.NumberOfTokensLabel); 
                while (selPlace.TokensList.Count != 0)
                {
                    MainModelCanvas.Children.Remove(selPlace.TokensList[0]); 
                    selPlace.TokensList.RemoveAt(0);
                }
                
            }
           // else
             //   Net.transitions.Remove(selected as Transition);

            DeleteArcs(node.ThisArcs);
            //selected.ThisArcs.Clear();

            var objectToRemove = GetKeyByValueForFigures(node);
            _allFiguresObjectReferences.Remove(objectToRemove); 

            Shape shapeToRemove = objectToRemove as Shape;
            if (shapeToRemove != null)
            {
                shapeToRemove.Stroke = Brushes.Black; 
                MainModelCanvas.Children.Remove(shapeToRemove); 
            }

            Label labelForRemove;
            NodesToLabelsInCanvas.TryGetValue(node, out labelForRemove);
            MainModelCanvas.Children.Remove(labelForRemove); 

            Net.RemoveNode(node);
        }
        

        public void RemoveTokens(VPlace place)
        {
            RemoveTokensFromCanvas(place);
            place.TokensList.Clear();
        }

        private void RemoveTokensFromCanvas(VPlace place)
        {
            foreach (var token in place.TokensList)
            {
                MainModelCanvas.Children.Remove(token);
            }
        }

           public void SetPropertiesForTokens(VPlace placeWithTokens, int width)
           {
               foreach (Ellipse ellipse in placeWithTokens.TokensList)
               {
                   ellipse.Width = width;
                   ellipse.Height = width;
                   ellipse.Fill = Brushes.Black;
                   ellipse.Visibility = Visibility.Visible;
               }
           }

           public void AddOneToken(VPlace place)
           {
               RemoveTokens(place);
               Ellipse token = new Ellipse();
               place.TokensList.Add(token);
               SetPropertiesForTokens(place, 10);
               Canvas.SetLeft(token, place.CoordX + 10);
               Canvas.SetTop(token, place.CoordY + 10);
               MainModelCanvas.Children.Add(token);
           }

           public void AddTwoTokens(VPlace place)
           {
               RemoveTokens(place);
               Ellipse token1 = new Ellipse();
               Ellipse token2 = new Ellipse();
               place.TokensList.Add(token1);
               place.TokensList.Add(token2);
               SetPropertiesForTokens(place, 7);
               Canvas.SetLeft(token1, place.CoordX + 6.5);
               Canvas.SetTop(token1, place.CoordY + 11.5);
               MainModelCanvas.Children.Add(token1);
               Canvas.SetLeft(token2, place.CoordX + 16.5);
               Canvas.SetTop(token2, place.CoordY + 11.5);
               MainModelCanvas.Children.Add(token2);
           }

           public void AddThreeTokens(VPlace marked)
           {
               RemoveTokens(marked);
               Ellipse token1 = new Ellipse();
               Ellipse token2 = new Ellipse();
               Ellipse token3 = new Ellipse();
               marked.TokensList.Add(token1);
               marked.TokensList.Add(token2);
               marked.TokensList.Add(token3);
               SetPropertiesForTokens(marked, 7);
               Canvas.SetLeft(token1, marked.CoordX + 6.5);
               Canvas.SetTop(token1, marked.CoordY + 8);
               MainModelCanvas.Children.Add(token1);
               Canvas.SetLeft(token2, marked.CoordX + 16.5);
               Canvas.SetTop(token2, marked.CoordY + 8);
               MainModelCanvas.Children.Add(token2);
               Canvas.SetLeft(token3, marked.CoordX + 11.5);
               Canvas.SetTop(token3, marked.CoordY + 16);
               MainModelCanvas.Children.Add(token3);
           }

           public void AddFourTokens(VPlace marked)
           {
               RemoveTokens(marked);
               Ellipse token1 = new Ellipse();
               Ellipse token2 = new Ellipse();
               Ellipse token3 = new Ellipse();
               Ellipse token4 = new Ellipse();
               marked.TokensList.Add(token1);
               marked.TokensList.Add(token2);
               marked.TokensList.Add(token3);
               marked.TokensList.Add(token4);
               SetPropertiesForTokens(marked, 7);
               Canvas.SetLeft(token1, marked.CoordX + 6.5);
               Canvas.SetTop(token1, marked.CoordY + 8);
               MainModelCanvas.Children.Add(token1);
               Canvas.SetLeft(token2, marked.CoordX + 16.5);
               Canvas.SetTop(token2, marked.CoordY + 8);
               MainModelCanvas.Children.Add(token2);
               Canvas.SetLeft(token3, marked.CoordX + 6.5);
               Canvas.SetTop(token3, marked.CoordY + 16);
               MainModelCanvas.Children.Add(token3);
               Canvas.SetLeft(token4, marked.CoordX + 16.5);
               Canvas.SetTop(token4, marked.CoordY + 16);
               MainModelCanvas.Children.Add(token4);
           }

           public void AddFiveTokens(VPlace marked, Label number)
           {
               RemoveTokens(marked);
               MainModelCanvas.Children.Remove(number);
               number.Content = marked.NumberOfTokens;
               Canvas.SetLeft(number, marked.CoordX + 7);
               Canvas.SetTop(number, marked.CoordY + 2);
               MainModelCanvas.Children.Add(number);
           }

           public void AddFrom6To999Tokens(VPlace marked, Label number)
           {
               RemoveTokens(marked);
               MainModelCanvas.Children.Remove(number);
               number.Content = marked.NumberOfTokens;
               if (marked.NumberOfTokens < 10)
               {
                   Canvas.SetLeft(number, marked.CoordX + 6);
                   Canvas.SetTop(number, marked.CoordY + 2);
               }
               else
               {
                   Canvas.SetLeft(number, marked.CoordX + 1);
                   Canvas.SetTop(number, marked.CoordY + 2);
               }
               MainModelCanvas.Children.Add(number);
           }

           public void AddMoreThan1000Tokens(VPlace marked, Label number)
           {
               RemoveTokens(marked);
               MainModelCanvas.Children.Remove(number);
               if (marked.NumberOfTokens % 1000 == 0)
               {
                   number.Content = marked.NumberOfTokens / 1000 + "k";
                   Canvas.SetLeft(number, marked.CoordX + 2);
                   Canvas.SetTop(number, marked.CoordY + 2);
               }
               else if (marked.NumberOfTokens < 10000)
               {
                   double temp = marked.NumberOfTokens % 100;
                   if (temp < 50)
                       number.Content = "~" + (marked.NumberOfTokens - temp) / 1000 + "k";
                   else number.Content = "~" + (marked.NumberOfTokens - temp + 100) / 1000 + "k";
                   Canvas.SetLeft(number, marked.CoordX - 4);
                   Canvas.SetTop(number, marked.CoordY + 2);
               }
               else
               {
                   if (marked.NumberOfTokens % 1000 < 500)
                       number.Content = "~" + marked.NumberOfTokens / 1000 + "k";
                   else number.Content = "~" + (marked.NumberOfTokens + 1000) / 1000 + "k";
                   Canvas.SetLeft(number, marked.CoordX - 2);
                   Canvas.SetTop(number, marked.CoordY + 2);
               }
               MainModelCanvas.Children.Add(number);
           }        
    }
}