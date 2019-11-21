using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PNEditorEditView;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace PNEditorTests
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        //public void containsPair_test1()
        //{
        //    AddSyncRelationWindow window = new AddSyncRelationWindow();
        //    string[] source1 = { "fsm1", "tr1" };
        //    string[] target1 = { "fsm2", "tr5" };

        //    string[] source2 = { "fsm1", "tr24" };
        //    string[] target2 = { "fsm2", "tr25" };

        //    PNEditorControl.SyncMode = PNEditorControl.SyncModeEnum.Sync;
        //    PNEditorControl.fsms = new Fsms();
        //    PNEditorControl.fsms.sync_first.Add(source1);
        //    PNEditorControl.fsms.sync_first.Add(source2);
        //    PNEditorControl.fsms.sync_second.Add(target1);
        //    PNEditorControl.fsms.sync_second.Add(target2);

        //    bool actual = window.ContainsPair(source1, target1);

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void containsPair_test2()
        //{
        //    AddSyncRelationWindow window = new AddSyncRelationWindow();
        //    string[] source1 = { "fsm1", "tr1" };
        //    string[] target1 = { "fsm2", "tr5" };

        //    string[] source2 = { "fsm1", "tr24" };
        //    string[] target2 = { "fsm2", "tr25" };
        //    string[] target3 = { "fsm2", "tr34"};

        //    PNEditorControl.SyncMode = PNEditorControl.SyncModeEnum.Sync;
        //    PNEditorControl.fsms = new Fsms();
        //    PNEditorControl.fsms.sync_first.Add(source1);
        //    PNEditorControl.fsms.sync_first.Add(source2);
        //    PNEditorControl.fsms.sync_second.Add(target1);
        //    PNEditorControl.fsms.sync_second.Add(target2);

        //    bool actual = window.ContainsPair(source1, target3);

        //    Assert.AreEqual(false, actual);
        //}

        //void clearLists()
        //{
        //    PNEditorControl.Net.places.Clear();
        //    //Place.places.Clear();
        //    //Transition.transitions.Clear();
        //    PNEditorControl.Net.transitions.Clear();
        //    Arc.arcs.Clear();
        //    PetriNetNode.figures.Clear();
        //}

        //[TestMethod]
        //public void deleteArcs_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = new Place(0, 0);
        //    Transition transition = new Transition(100, 100);
        //    Arc arc = control.AddArc(place, transition);
            
        //    Shape line = control.GetKeyByValueForArcs(arc, control.DictionaryForArcs);

        //    List<Arc> arcs = new List<Arc>();
        //    arcs.Add(arc);

        //    control.DeleteArcs(arcs);

        //    bool cond1 = !Arc.arcs.Contains(arc);
        //    bool cond2 = !arc.IsSelect;
        //    bool cond3 = !control.DictionaryForArcs.ContainsKey(line);
        //    bool cond4 = !arc.From.ThisArcs.Contains(arc);
        //    bool cond5 = !arc.To.ThisArcs.Contains(arc);
        //    bool actual = cond1 && cond2 && cond3 && cond4 && cond5;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void makeSelected_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    place.IsSelect = true;

        //    control.MakeSelected(place);

        //    bool cond1 = control.SelectedFigures.Contains(place);
        //    Shape ellipseForColor =
        //        control.GetKeyByValueForFigures(place) as Shape;
        //    bool cond2 = ellipseForColor.Stroke == Brushes.Coral;
        //    Assert.AreEqual(true, cond1 && cond2);
        //}

        //[TestMethod]
        //public void saveNewFigures_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    Transition transition = control.AddTransition(100, 100);
        //    List<PetriNetNode> nodes = control.SaveNewFigures(PetriNetNode.figures);
        //    bool cond1, cond2, cond3, cond4;
        //    cond1 = cond2 = cond3 = cond4 = false;
        //    foreach (PetriNetNode node in nodes)
        //    {
        //        if (node is Place)
        //        {
        //            cond1 = node != place;
        //            cond2 = node.Id == place.Id && node.CoordX == place.CoordX
        //                && node.CoordY == place.CoordY;
        //        }
        //        else
        //        {
        //            cond3 = node != transition;
        //            cond4 = node.Id == transition.Id && node.CoordX == transition.CoordX
        //                && node.CoordY == transition.CoordY;
        //        }
        //    }

        //    bool actual = cond1 && cond2 && cond3 && cond4;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void addArc_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    Transition transition = control.AddTransition(100, 100);

        //    Arc arc = control.AddArc(place, transition);

        //    bool cond1 = place.ThisArcs.Contains(arc);
        //    bool cond2 = transition.ThisArcs.Contains(arc);
        //    bool cond3 = control.DictionaryForArcs.ContainsValue(arc);
        //    bool actual = cond1 && cond2 && cond3;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void visualizeArc_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    Transition transition = control.AddTransition(100, 100);

        //    Random ran = new Random();
        //    int i = ran.Next(0, 2);
        //    Arc arc;
        //    bool actual;

        //    if (i == 0)
        //        arc = new Arc(place, place);
        //    else
        //        arc = new Arc(place, transition);

        //    control.VisualizeArc(arc);

        //    if (i == 0)
        //        actual = control.GetKeyByValueForArcs(arc, control.DictionaryForArcs) is System.Windows.Shapes.Path;
        //    else
        //        actual = control.GetKeyByValueForArcs(arc, control.DictionaryForArcs) is Line;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void setCoordinatesOfLine_test1()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    Transition transition = control.AddTransition(100, 100);

        //    Arc arc = control.AddArc(place, transition);

        //    Line line = control.GetKeyByValueForArcs(arc, control.DictionaryForArcs) as Line;
        //    control.SetCoordinatesOfLine(line, arc);

        //    bool cond1 = line.X1 == place.CoordX + 30;
        //    bool cond2 = line.Y1 == place.CoordY + 15;
        //    bool cond3 = line.X2 == transition.CoordX;
        //    bool cond4 = line.Y2 == transition.CoordY + 25;
        //    bool actual = cond1 && cond2 && cond3 && cond4;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void setCoordinatesOfLine_test2()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(100, 100);
        //    Transition transition = control.AddTransition(0, 0);

        //    Arc arc = control.AddArc(place, transition);

        //    Line line = control.GetKeyByValueForArcs(arc, control.DictionaryForArcs) as Line;
        //    control.SetCoordinatesOfLine(line, arc);

        //    bool cond1 = line.X1 == place.CoordX;
        //    bool cond2 = line.Y1 == place.CoordY + 15;
        //    bool cond3 = line.X2 == transition.CoordX + 20;
        //    bool cond4 = line.Y2 == transition.CoordY + 25;
        //    bool actual = cond1 && cond2 && cond3 && cond4;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void addPLace_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    bool cond1 = Place.places.Contains(place);
        //    bool cond2 = PetriNetNode.figures.Contains(place);
        //    bool cond3 = control.AllFiguresObjectReferences.ContainsValue(place);
        //    bool actual = cond1 && cond2 && cond3;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void addTransition_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Transition transition = control.AddTransition(0, 0);
        //    //bool cond1 = Transition.transitions.Contains(transition);
        //    bool cond1 = PNEditorControl.Net.transitions.Contains(transition);
        //    bool cond2 = PetriNetNode.figures.Contains(transition);
        //    bool cond3 = control.AllFiguresObjectReferences.ContainsValue(transition);
        //    bool actual = cond1 && cond2 && cond3;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void stopInMesh_test_place()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    double actualX, actualY;
        //    control.StopInMesh(10, 10, "place", out actualX, out actualY);

        //    Assert.AreEqual(25, actualX);
        //    Assert.AreEqual(35, actualY);
        //}

        //[TestMethod]
        //public void stopInMesh_test_transition1()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    double actualX, actualY;
        //    control.StopInMesh(60, 40, "transition", out actualX, out actualY);

        //    Assert.AreEqual(80, actualX);
        //    Assert.AreEqual(35, actualY);
        //}

        //[TestMethod]
        //public void stopInMesh_test_transition2()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    double actualX, actualY;
        //    control.StopInMesh(50, 100, "transition", out actualX, out actualY);

        //    Assert.AreEqual(20, actualX);
        //    Assert.AreEqual(105, actualY);
        //}

        //[TestMethod]
        //public void changeLabel_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);
        //    control.AddLabel(place, place.Id);

        //    Label label;
        //    control.DictionaryForLabels.TryGetValue(place, out label);

        //    control.ChangeLabel(place, place.Id + "!");

        //    bool cond1 = !control.DictionaryForLabels.ContainsValue(label);
        //    bool cond2 = !control.AllLabels.Contains(label);

        //    Label newLabel;
        //    control.DictionaryForLabels.TryGetValue(place, out newLabel);

        //    bool cond3 = newLabel.Content.ToString() == place.Id + "!";

        //    bool actual = cond1 && cond2 && cond3;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void addLabel_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(0, 0);

        //    control.AddLabel(place, place.Id);

        //    Label label;
        //    control.DictionaryForLabels.TryGetValue(place, out label);

        //    bool cond1 = control.DictionaryForLabels.ContainsValue(label);
        //    bool cond2 = control.AllLabels.Contains(label);
        //    bool cond3 = label.Content.ToString() == place.Id;
        //    bool actual = cond1 && cond2 && cond3;

        //    Assert.AreEqual(true, actual);
        //}

        //[TestMethod]
        //public void setCoordinatesByMesh_test_place()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = control.AddPlace(10, 10);

        //    control.SetCoordinatesByMesh(place);

        //    double actualX = place.CoordX;
        //    double actualY = place.CoordY;

        //    Assert.AreEqual(10, actualX);
        //    Assert.AreEqual(20, actualY);
        //}

        //[TestMethod]
        //public void setCoordinatesByMesh_test_transition()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Transition transition = control.AddTransition(60, 40);

        //    control.SetCoordinatesByMesh(transition);

        //    double actualX = transition.CoordX;
        //    double actualY = transition.CoordY;

        //    Assert.AreEqual(70, actualX);
        //    Assert.AreEqual(10, actualY);
        //}

        //[TestMethod]
        //public void colorArrow_test()
        //{
        //    clearLists();
        //    PNEditorControl control = new PNEditorControl();
        //    Place place = new Place(0, 0);
        //    Transition transition = new Transition(100, 100);
            
        //    Arc arc = control.AddArc(place, transition);
        //    arc.IsSelect = true;

        //    control.ColorArrow(arc);

        //    bool actual = control.GetKeyByValueForArcs(arc, control.DictionaryForArcs).Stroke == Brushes.Coral;

        //    Assert.AreEqual(true, actual);
        //}








    }
}
