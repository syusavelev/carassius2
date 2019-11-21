using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNEditorSimulateView;

namespace PNEditorSimulateView.ModelArrange
{
    class PetriNetColumnAndGraphForceBasedGeneralArranger : IArranger
    {        
        //todo А почему в ArrangePetriNetModel используем MoveFigure, тогда как в ArrangeMulticlusterGraph прекрасно обходимся без него?!

        public void ArrangePetriNetModel(List<PetriNetNode> allFigures, IList<VArc> arcs, PNEditorControl control)
        {
            var PNEditorControl = new
            {
                Mode = new
                {
                    Graph = 1,
                    PetriNet = 2
                }
            };
            var editorMode = 2;

            if (allFigures.Count == 0) return;

            foreach (var figure in allFigures)
            {
                figure.Column = 0;
                figure.IsChecked = false;
                figure.ModelNumber = 0;
            }

            SetColumnsForStartFigures(allFigures);
            NextColumn(allFigures);

            int modelNumber = 0;
            int numberOfFiguresChecked = 0;
            while (numberOfFiguresChecked != allFigures.Count)
            {
                PetriNetNode next = allFigures[0];
                modelNumber++;
                foreach (PetriNetNode figure in allFigures)
                {
                    if (figure.IsChecked == false)
                    {
                        next = figure;
                        next.ModelNumber = modelNumber;
                        next.IsChecked = true;
                        break;
                    }
                }
                DepthFirstSearch(next, next.ModelNumber);
                numberOfFiguresChecked = allFigures.Count(t => t.IsChecked);
            }

            int numberOfModels = allFigures.Select(t => t.ModelNumber).Concat(new[] { 1 }).Max();
            List<PetriNetNode> figuresInOneModel = new List<PetriNetNode>();


            double spaceBetweenColumnsCoefficient = 1;
            int maxNumberOfColumnsInAllModels = 1;
            if (editorMode == PNEditorControl.Mode.Graph)
            {
                ArrangeMulticlusterGraph(numberOfModels, figuresInOneModel, allFigures, arcs);
                foreach (PetriNetNode figure in allFigures)
                {
                    control.MoveFigure(figure); //todo это ужас, конечно. надо нормально декомпозировать и убрать
                }
            }
            else
            {
                if (editorMode != PNEditorControl.Mode.PetriNet) return; //todo нехорошо, в общем-то, привязки лишние
                double maxYinModel = 0;
                for (int n = 1; n < numberOfModels + 1; n++)
                {
                    List<PetriNetNode> modelFigures;
                    int maxNumberOfRows;
                    int maxColumn;

                    SearchColumnWithMaxNumberOfRows(n, allFigures, out maxNumberOfRows, out maxColumn,
                        out modelFigures);

                    var numbercolumns = NumberOfColumnsOfFiguresList(modelFigures);

                    if (numbercolumns > maxNumberOfColumnsInAllModels)
                        maxNumberOfColumnsInAllModels = numbercolumns;

                    for (int i = 1; i < numbercolumns + 1; i++)
                    {
                        double coordY =
                            GetCoordYOfFirstFigureInColumn(maxNumberOfRows, i, modelFigures) +
                            maxYinModel;

                        List<PetriNetNode> thisColumn = modelFigures.Where(t => t.Column == i).ToList();
                        if (thisColumn.Count != 0)
                        {
                            if (thisColumn[0] is VPlace)
                                thisColumn[0].CoordY = coordY + 10;
                            else thisColumn[0].CoordY = coordY;
                        }
                        int numberOfArcsInColumn = 0;

                        for (int k = 1; k < thisColumn.Count; k++)
                        {
                            thisColumn[k].CoordY = thisColumn[k - 1].CoordY + 70;
                        }

                        foreach (PetriNetNode figure in thisColumn)
                        {
                            numberOfArcsInColumn =
                                figure.ThisArcs.Count(t => (t.To.Column > t.From.Column && figure == t.From) ||
                                                           (t.From.Column > t.To.Column && figure == t.To));

                            figure.SpaceCoefficient = spaceBetweenColumnsCoefficient;
                        }

                        spaceBetweenColumnsCoefficient 
                            = GetSpaceBetweenColumnsCoefficientMagically(numberOfArcsInColumn);
                    }

                    maxYinModel = modelFigures.Select(t => t.CoordY).Concat(new[] { maxYinModel }).Max();
                    maxYinModel += 60; //todo magic number
                    spaceBetweenColumnsCoefficient = 1;
                }

                //todo вот тут не норм, подумать как для разных моделей свои раастояния рассчитывать
                for (int p = 1; p < maxNumberOfColumnsInAllModels + 1; p++)
                {
                    foreach (PetriNetNode figure in allFigures)
                    {
                        if (figure.Column == p)
                        {
                            //TODO здесь нужно как-то вообще убрать эту временную переменную
                            PetriNetNode tempF = PetriNetNode.Create();
                            foreach (PetriNetNode f in allFigures)
                            {
                                if (f.Column == figure.Column - 1)
                                {
                                    tempF = f;
                                    break;
                                }
                            }
                            double lyambda;
                            if (tempF is VPlace || tempF is VTransition)
                                lyambda = tempF.CoordX - 10;
                            else lyambda = -60;

                            figure.CoordX = 10 + lyambda + 60 * figure.SpaceCoefficient;
                        }
                        control.MoveFigure(figure); //todo это ужас, конечно. надо нормально декомпозировать и убрать
                    }
                }
            }
        }

        public void ArrangeMulticlusterGraph(int numberOfClusters, List<PetriNetNode> figuresInOneCluster, List<PetriNetNode> figuresInAllClusters, IList<VArc> arcs)
        {
            double maxPreviousY = 0;
            for (int i = 1; i < numberOfClusters + 1; i++)
            {
                figuresInOneCluster.Clear();
                figuresInOneCluster.AddRange(figuresInAllClusters.Where(t => t.ModelNumber == i));
                ForceBasedAlgorithm(figuresInOneCluster, arcs);
                double max = figuresInOneCluster.Select(t => t.CoordY).Concat(new double[] { 0 }).Max();
                //todo magic number
                max += 50;
                foreach (PetriNetNode figure in figuresInOneCluster)
                {
                    figure.CoordY += maxPreviousY;
                }
                maxPreviousY += max;
            }
        }

        public void DepthFirstSearch(PetriNetNode figure, int modelNum)
        {
            foreach (VArc a in figure.ThisArcs)
            {
                PetriNetNode f = figure == a.To ? a.From : a.To;
                f.ModelNumber = figure.ModelNumber;
                f.IsChecked = true;
                foreach (VArc arc in f.ThisArcs)
                {
                    PetriNetNode fig = f == arc.From ? arc.To : arc.From;
                    if (fig.IsChecked == false)
                    {
                        fig.IsChecked = true;
                        fig.ModelNumber = modelNum;
                        DepthFirstSearch(fig, modelNum);
                    }
                }
            }
        }

        // ---------------------------------- private part ----------------------------------

        private static readonly Random ran = new Random();

        private void ForceBasedAlgorithm(List<PetriNetNode> figures, IList<VArc> arcs)
        {
            double oldX, oldY, newX, newY;
            const double tolerance = 0.00001;
            foreach (PetriNetNode figure in figures)
            {
                figure.CoordX = 200 + ran.NextDouble() * 300;
                figure.CoordY = 100 + ran.NextDouble() * 200;
            }

            do
            {
                for (int i = 0; i < figures.Count; i++)
                {
                    figures[i].NetForceX = figures[i].NetForceY = 0;
                    for (int j = 0; j < figures.Count; j++)
                    {
                        if (i == j) continue;
                        double squaredDistance = Math.Pow(figures[i].CoordX - figures[j].CoordX, 2) +
                            Math.Pow(figures[i].CoordY - figures[j].CoordY, 2);

                        // counting the repulsion between two vertices 
                        figures[i].NetForceX += 200 * (figures[i].CoordX - figures[j].CoordX) / squaredDistance;
                        figures[i].NetForceY += 200 * (figures[i].CoordY - figures[j].CoordY) / squaredDistance;
                    }

                    foreach (VArc arc in arcs)
                    {
                        if (arc.From == figures[i])
                        {
                            figures[i].NetForceX += 0.06 * (arc.To.CoordX - figures[i].CoordX);
                            figures[i].NetForceY += 0.06 * (arc.To.CoordY - figures[i].CoordY);
                        }
                        else if (arc.To == figures[i])
                        {
                            figures[i].NetForceX += 0.06 * (arc.From.CoordX - figures[i].CoordX);
                            figures[i].NetForceY += 0.06 * (arc.From.CoordY - figures[i].CoordY);
                        }
                    }

                    figures[i].VelocityX = (figures[i].VelocityX + figures[i].NetForceX) * 0.85;
                    figures[i].VelocityY = (figures[i].VelocityY + figures[i].NetForceY) * 0.85;
                }

                oldX = figures[0].CoordX;
                oldY = figures[0].CoordY;
                foreach (PetriNetNode figure in figures)
                {
                    figure.CoordX += figure.VelocityX;
                    figure.CoordY += figure.VelocityY;
                }
                newX = figures[0].CoordX;
                newY = figures[0].CoordY;
            } while (Math.Abs(oldX - newX) > tolerance || Math.Abs(oldY - newY) > tolerance);

            double minX = figures[0].CoordX, minY = figures[0].CoordY;
            for (int i = 1; i < figures.Count; i++)
            {
                if (figures[i].CoordX < minX)
                    minX = figures[i].CoordX;
                if (figures[i].CoordY < minY)
                    minY = figures[i].CoordY;
            }

            foreach (PetriNetNode figure in figures)
            {
                figure.CoordX -= minX - 5;
                figure.CoordY -= minY - 5;
            }
        }

        private double GetSpaceBetweenColumnsCoefficientMagically(int numberOfArcsInColumn)
        {
            if (numberOfArcsInColumn < 6)
                return 1;
            else if (numberOfArcsInColumn >= 6 && numberOfArcsInColumn < 11)
                return 2;
            else if (numberOfArcsInColumn >= 11 && numberOfArcsInColumn < 31)
                return 4;
            else if (numberOfArcsInColumn >= 31 && numberOfArcsInColumn < 51)
                return 6;
            else if (numberOfArcsInColumn >= 51 && numberOfArcsInColumn < 101)
                return 10;
            else if (numberOfArcsInColumn >= 101 && numberOfArcsInColumn < 501)
                return 20;
            else if (numberOfArcsInColumn >= 501 && numberOfArcsInColumn < 1001)
                return 25;
            else return 30;
        }

        private double GetCoordYOfFirstFigureInColumn(int maxNumberOfRows, int thisColumn, List<PetriNetNode> thisModel)
        {
            var thisNumberOfRows = thisModel.Count(figure => figure.Column == thisColumn);
            double y = 10 + (70 * maxNumberOfRows - 20) / 2 - (70 * thisNumberOfRows - 20) / 2;
            return y;
        }

        private int NumberOfColumnsOfFiguresList(List<PetriNetNode> figuresList)
        {
            return figuresList.Select(figure => figure.Column).Concat(new[] { 1 }).Max();
        }

        private void SearchColumnWithMaxNumberOfRows(int model, List<PetriNetNode> allFigures,
            out int maxNumber,
            out int maxLevel,
            out List<PetriNetNode> thisModelFigures)
        {
            int maxColumn = 0;
            int maxNumberOfRows = 0;
            int numberOfCheckedFigures = 0;
            List<PetriNetNode> thisModel = allFigures.Where(t => t.ModelNumber == model).ToList();
            thisModelFigures = thisModel;
            foreach (PetriNetNode figure in thisModel)
            {
                if (figure.Column == 1)
                {
                    maxNumberOfRows += 1;
                    maxColumn = 1;
                }
            }
            if (maxColumn == 0)
            {
                foreach (PetriNetNode figure in thisModel)
                    if (figure.Column == 2)
                    {
                        maxNumberOfRows += 1;
                        maxColumn = 2;
                    }
            }

            int numberOfColumns = NumberOfColumnsOfFiguresList(thisModel);
            while (numberOfCheckedFigures != thisModel.Count)
            {
                for (int i = 1; i < numberOfColumns + 1; i++)
                {
                    int thisNumberOfRows = 0;
                    foreach (PetriNetNode figure in thisModel)
                    {
                        if (figure.Column == i)
                        {
                            thisNumberOfRows += 1;
                            numberOfCheckedFigures += 1;
                        }
                    }
                    if (thisNumberOfRows > maxNumberOfRows)
                    {
                        maxNumberOfRows = thisNumberOfRows;
                        maxColumn = i;
                    }
                }
            }
            maxNumber = maxNumberOfRows;
            maxLevel = maxColumn;
        }

        private List<PetriNetNode> SearchFiguresWithoutFrom(List<PetriNetNode> figures)
        {
            List<PetriNetNode> startFigures = new List<PetriNetNode>();
            foreach (PetriNetNode figure in figures)
            {
                if (figure.ThisArcs.Count == 0)
                {
                    startFigures.Add(figure);
                }
                else
                {
                    bool hasIngoingArcs = figure.ThisArcs.Any(arc => arc.To == figure);
                    if (hasIngoingArcs == false)
                        startFigures.Add(figure);
                }
            }
            if (startFigures.Count == 0) startFigures.Add(figures[0]);
            return startFigures;
        }

        private void SetColumnsForStartFigures(List<PetriNetNode> figures)
        {
            List<PetriNetNode> start1 = SearchFiguresWithoutFrom(figures);
            if (start1.Count != 0)
            {
                foreach (PetriNetNode t in start1)
                {
                    if (t is VPlace)
                        t.Column = 1;
                    else
                        t.Column = 2;
                }
            }
        }


        private void NextColumn(List<PetriNetNode> figures)
        {
            int numberOfColumnedFigures = 0;
            int level = 1;
            while (figures.Count != numberOfColumnedFigures)
            {
                List<PetriNetNode> thisLevel = figures.Where(t => t.Column == level).ToList();
                foreach (PetriNetNode figureOnLevel in thisLevel)
                {
                    foreach (VArc arc in figureOnLevel.ThisArcs)
                    {
                        PetriNetNode figure = arc.To != figureOnLevel ? arc.To : arc.From;
                        if (figure.Column == 0)
                            figure.Column = level + 1;
                    }
                }
                level++;
                if (level > figures.Count)
                {
                    for (int i = 1; i < figures.Count; i++)
                    {
                        if (figures[i].Column == 0)
                        {
                            if (figures[i] is VPlace)
                                figures[i].Column = 1;
                            else figures[i].Column = 2;
                            level = 1;
                            break;
                        }
                    }
                }
                numberOfColumnedFigures = figures.Count(figure => figure.Column != 0);
            }
        }
    }
}
