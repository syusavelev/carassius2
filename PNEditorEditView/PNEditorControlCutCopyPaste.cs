using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using Core;

namespace PNEditorEditView
{
    /// <summary>
    /// Author: Natalia Nikitina / Alexey Mitsyuk
    /// Cut, Copy and Paste Functions
    /// PAIS Lab, 2014
    /// </summary>
    public partial class PNEditorControl 
    {
        public static List<PetriNetNode> cuttedOrCopiedFigures = new List<PetriNetNode>();
        List<VArc> cuttedOrCopiedArcs = new List<VArc>();
        enum cutOrCopyMode { cut, copy };
        cutOrCopyMode cutOrCopy;
        bool nothingToPaste;

        private static bool ShouldBeDeleted(VArc arc)
        {
            if (!(cuttedOrCopiedFigures.Contains(arc.From)) ||
                   !(cuttedOrCopiedFigures.Contains(arc.To)))
                return true;
            else return false;
        }

        public void AddThisArcsToListOfCuttedArcs(List<PetriNetNode> selected, List<VArc> cuttedArcs)
        {
            foreach (PetriNetNode figure in selected)
            {
                foreach (VArc arc in figure.ThisArcs)
                    if (!cuttedArcs.Contains(arc))
                        cuttedArcs.Add(arc);
            }
        }
        [MenuItemHandler("edit/cut",25)]
        public void CutMenuItem_Click()
        {
            btnCut_Click(null,null);
        }
        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            _numberOfPastes = 0;
            if (_selectedFigures.Count != 0)
            {
                nothingToPaste = false;
                cutOrCopy = cutOrCopyMode.cut;

                cuttedOrCopiedArcs.Clear();
                cuttedOrCopiedFigures.Clear();

                cuttedOrCopiedFigures.AddRange(_selectedFigures);
                cuttedOrCopiedArcs.AddRange(_selectedArcs);

                List<PetriNetNode> cuttedF = new List<PetriNetNode>();
                List<VArc> cuttedA = new List<VArc>();
                cuttedF.AddRange(cuttedOrCopiedFigures);
                cuttedA.AddRange(cuttedOrCopiedArcs);

                List<VArc> impl = new List<VArc>();
                AddThisArcsToListOfCuttedArcs(cuttedOrCopiedFigures, impl);

                List<PetriNetNode> cutF = new List<PetriNetNode>();
                List<VArc> cutA = new List<VArc>();
                cutF.AddRange(cuttedOrCopiedFigures);
                cutA.AddRange(impl);

                UnselectFigures();//(selectedFigures, selectedArcs);

                List<PetriNetNode> old = new List<PetriNetNode>();
                foreach (PetriNetNode figure in cutF)
                {
                    PetriNetNode f = PetriNetNode.Create();
                    f.CoordX = figure.CoordX;
                    f.CoordY = figure.CoordY;
                    f.Id = figure.Id;
                    old.Add(f);
                }

                CutFigures(cutF, cutA);

                CutCommand newCommand = new CutCommand(cuttedF, old, cuttedA, impl);
                Command.ExecutedCommands.Push(newCommand);
                Command.CanceledCommands.Clear();

                HideAllProperties();
            }
            else nothingToPaste = true;

            TurnOnSelectMode();
            btnCut.Focusable = false;
        }

        public void CutFigures(List<PetriNetNode> cuttedFigures, List<VArc> cuttedArcs)
        {
            DeleteArcs(cuttedArcs);

            if (cuttedFigures.Count != 0)
            {
                foreach (PetriNetNode figure in cuttedFigures)
                {
                    RemoveNode(figure);
                }
            }

            _selectedFigure = null;
            _selectedArc = null;
        }
        [MenuItemHandler("edit/copy",26)]
        public void CopyMenuItem_Click()
        {
            btnCopy_Click(null,null);
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFigures.Count != 0)
            {
                nothingToPaste = false;
                cutOrCopy = cutOrCopyMode.copy;

                cuttedOrCopiedArcs.Clear();
                cuttedOrCopiedFigures.Clear();

                cuttedOrCopiedFigures.AddRange(_selectedFigures);
                cuttedOrCopiedArcs.AddRange(_selectedArcs);
            }
            else nothingToPaste = true;

            TurnOnSelectMode();
            btnCopy.Focusable = false;
        }

        public List<VArc> PasteFiguresAndArcs(List<PetriNetNode> figuresToPaste, List<VArc> arcsToPaste)
        {
            //SetOfFigures.Figures.AddRange(figuresToPaste);

            foreach (VArc arc in arcsToPaste)
            {
                if (cutOrCopy == cutOrCopyMode.copy)
                    arc.DetectIdMatches(Net.arcs);
                Net.arcs.Add(arc);
            }
            foreach (PetriNetNode figure in figuresToPaste)//SetOfFigures.Figures)
            {
                //if (figuresToPaste.Contains(figure))
                {
                    DrawFigure(figure);
                    figure.IsSelect = true;
                    MakeSelected(figure);
                }
            }
            List<VArc> toDelete = new List<VArc>();
            foreach (VArc arc in Net.arcs)
            {
                if (arcsToPaste.Contains(arc))
                {
                    if (figuresToPaste.Contains(arc.From) &&
                        figuresToPaste.Contains(arc.To))
                    {
                        _selectedArcs.Add(arc);
                        arc.IsSelect = true;

                        DisplayArc(arc);
                        ColorArrow(arc);

                        btnShowHideLabels.IsEnabled = true;
                    }
                    else toDelete.Add(arc);
                }
            }
            return toDelete;
        }

        readonly Dictionary<PetriNetNode, PetriNetNode> _copies = new Dictionary<PetriNetNode, PetriNetNode>();
        int _numberOfPastes;
        [MenuItemHandler("edit/paste",27)]
        public void PasteMenuItem_Click()
        {
            btnPaste_Click(null,null);
        }
        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            btnUndo.IsEnabled = true;
            _copies.Clear();
            if (cuttedOrCopiedFigures.Count == 0) nothingToPaste = true;
            if (nothingToPaste == false)
            {
                double minX = cuttedOrCopiedFigures[0].CoordX, minY = cuttedOrCopiedFigures[0].CoordY;

                foreach (PetriNetNode figure in cuttedOrCopiedFigures)
                {
                    if (figure.CoordX < minX)
                        minX = figure.CoordX;
                    if (figure.CoordY < minY)
                        minY = figure.CoordY;
                }

                if (cutOrCopy == cutOrCopyMode.copy)
                {
                    var copiedF = new List<PetriNetNode>();
                    var copiedA = new List<VArc>();

                    UnselectFigures();//(cuttedOrCopiedFigures, cuttedOrCopiedArcs);

                    foreach (PetriNetNode copiedFigure in cuttedOrCopiedFigures)
                    {
                        var x = copiedFigure.CoordX - minX + ScrollViewerForMainModelCanvas.HorizontalOffset + 40;
                        var y = copiedFigure.CoordY - minY + ScrollViewerForMainModelCanvas.VerticalOffset + 40;

                        PetriNetNode newFigure;
                        if (copiedFigure is VPlace)
                        {
                            newFigure = VPlace.Create(x, y);
                            Net.places.Add((VPlace)newFigure);
                        }
                        else
                        {
                            newFigure = VTransition.Create(x, y);
                            Net.transitions.Add((VTransition) newFigure);
                        }
                        //SetOfFigures.Figures.Add(newFigure);
                        copiedF.Add(newFigure);

                        newFigure.Label = copiedFigure.Label;
                        VPlace place = newFigure as VPlace;
                        if (place != null)
                            place.NumberOfTokens = (copiedFigure as VPlace).NumberOfTokens;
                        newFigure.IsSelect = true;

                        DrawFigure(newFigure);
                        _copies.Add(copiedFigure, newFigure);
                        MakeSelected(newFigure);
                    }


                    foreach (PetriNetNode fig in cuttedOrCopiedFigures)
                    {
                        foreach (var arc in fig.ThisArcs)
                        {
                            if (!cuttedOrCopiedArcs.Contains(arc)) continue;

                            PetriNetNode fromF;
                            _copies.TryGetValue(arc.From, out fromF);
                            PetriNetNode toF;
                            _copies.TryGetValue(arc.To, out toF);
                            var arcs = from ar in Net.arcs where ar.From == fromF && ar.To == toF select ar;
                                    
                            if (arcs.Any()) continue;

                            var newArc = new VArc(fromF, toF)
                            {
                                IsDirected = arc.IsDirected,
                                Weight = arc.Weight
                            };
                            newArc.AddToThisArcsLists();
                            DrawArc(newArc);
                                    
                            if (newArc.IsDirected)
                            {
                                var lineVisible = GetKeyByValueForArcs(newArc, DictionaryForArcs);
                                DrawArrowHeads(lineVisible);
                                GetKeyByValueForArcs(newArc, DictionaryForArrowHeads1).MouseDown += MouseArcDown;
                                GetKeyByValueForArcs(newArc, DictionaryForArrowHeads2).MouseDown += MouseArcDown;
                            }

                            newArc.IsSelect = true;
                            _selectedArcs.Add(newArc);
                            ColorArrow(newArc);
                            Net.arcs.Add(newArc);
                            copiedA.Add(newArc);
                        }
                    }
                    var newCommand = new PasteCommand(copiedF, copiedA);
                    Command.ExecutedCommands.Push(newCommand);
                }
                else if (cutOrCopy == cutOrCopyMode.cut && _numberOfPastes == 0)
                {
                    _numberOfPastes++;
                    foreach (var figure in cuttedOrCopiedFigures)
                    {
                        foreach (var arc in figure.ThisArcs)
                        {
                            if (!cuttedOrCopiedArcs.Contains(arc))
                            {
                                cuttedOrCopiedArcs.Add(arc);
                            }
                        }
                        figure.CoordX += -minX + ScrollViewerForMainModelCanvas.HorizontalOffset;
                        figure.CoordY += -minY + ScrollViewerForMainModelCanvas.VerticalOffset;
                    }

                    var toDelete = PasteFiguresAndArcs(cuttedOrCopiedFigures, cuttedOrCopiedArcs);
                    foreach (var arc in toDelete)
                    {
                        cuttedOrCopiedArcs.Remove(arc);
                        arc.IsSelect = false;
                        _selectedArcs.Remove(arc);
                        Net.arcs.Remove(arc);
                    }

                    var figures = new List<PetriNetNode>();
                    var arcs1 = new List<VArc>();
                    figures.AddRange(cuttedOrCopiedFigures);
                    arcs1.AddRange(cuttedOrCopiedArcs);

                    var newCommand = new PasteCommand(figures, arcs1);
                    Command.ExecutedCommands.Push(newCommand);
                }
                Command.CanceledCommands.Clear();
            }
            _figuresBeforeDrag = CopyListOfFigures(Net.Nodes);
            ReassignSelectedProperties();
            TurnOnSelectMode();
            btnPaste.Focusable = false;
        }
    }
}
