using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using Core;
using Microsoft.Win32;
using PNEditorEditView.Util;

namespace PNEditorEditView
{
    /// <summary>
    /// Author: Natalia Nikitina / Alexey Mitsyuk
    /// Main menu
    /// PAIS Lab, 2014
    /// </summary>
    public partial class PNEditorControl
    {
        public static string currentFileName;
        public static int numberOfModel;
        [MenuItemHandler("file/save as",25)]
        public void MenuSaveAs_Click()
        {
            btnSelect.IsEnabled = true;
            SaveFileDialog SFdialog = new SaveFileDialog();
            SetFilterToSaveFileDialog(SFdialog);
            if (SFdialog.ShowDialog() == true)
            {
                SaveModel(SFdialog.FileName, true);
                menuSave.IsEnabled = true;
                MainController.Self.SetMenuItemEnabled(Core.Util.PathOfMethod(MenuSave_Click),true);
                numberOfModel++;
            }
            currentFileName = SFdialog.FileName;
            isSomethingChanged = false;
        }
        [MenuItemHandler("file/export/as image", 26)]
        public void MenuExportToPicture_Click()
        {
            btnSaveToPng_Click(null, null);
        }


        private void SaveModel(string fileName, bool saveLayout)
        {
            var s = System.IO.Path.GetFileNameWithoutExtension(fileName);
            XmlDocument doc = null;
            doc = Pnml.SavePnml(s, Net.arcs, saveLayout);

            doc.Save(fileName);
            //doc.Save(currentFileName);
        }

        private void SetFilterToSaveFileDialog(SaveFileDialog SFdialog)
        {
           
                    SFdialog.Filter = "PNML files(*.pnml)|*.pnml";
        }

        
        [MenuItemHandler("file/open",22)]
        public void MenuOpen_Click()
        {
            EnableAddButtons();
            btnSelect.IsEnabled = true;
            MessageBoxResult result;

            if (currentFileName == null)
            {
                result = MessageBox.Show("Do you want to save the current file?",
                    "Save..", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveFileDialog SFdialog = new SaveFileDialog();
                    SetFilterToSaveFileDialog(SFdialog);

                    if (SFdialog.ShowDialog() == true)
                    {
                        SaveModel(SFdialog.FileName, true);
                        menuSave.IsEnabled = true;
                        MainController.Self.SetMenuItemEnabled(Core.Util.PathOfMethod(MenuSave_Click), true);

                    }
                }
            }
            else
            {
                result = MessageBox.Show("Do you want to save changes in " +
                    System.IO.Path.GetFileName(currentFileName) + "?", "Save..", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    SaveModel(currentFileName, true);
            }

            currentFileName = null;

            if (result == MessageBoxResult.Cancel) return;


            var OFdialog = new OpenFileDialog
            {
                Filter = "PNML files(*.pnml)|*.pnml"
            };

            if (OFdialog.ShowDialog() == true)
            {
                //clear the model
                DeleteArcs(Net.arcs);
                while (Net.Nodes.Count != 0)
                    RemoveNode(Net.Nodes[0]);
                //Net.arcs.Clear();
                ClearCanvasWithoutLoss();
                HideAllProperties();
                _selectedFigures.Clear();
                _selectedArcs.Clear();
                _selectedArc = null;
                _selectedFigure = null;
                var net = new XmlDocument();
                var exception = false;
                try
                {
                    net.Load(OFdialog.FileName);
                    var s = System.IO.Path.GetFileNameWithoutExtension(OFdialog.FileName);
                    ShowMainWindowTitleDelegate("Carassius - Petri Net Editor | " + s); //OFdialog.FileName);
                }
                catch (XmlException)
                {
                    MessageBox.Show("The file contains data, that don't match with format of PNML");
                    exception = true;
                }
                if (exception == false)
                {

                    bool flag = false;

                    if (net.LastChild.Name == "pnml")
                    {
                        Net.arcs.AddRange(Pnml.OpenPnml(net, out flag));

                        foreach (var figure in Net.Nodes)
                        {
                            foreach (var arc in Net.arcs)
                                if (arc.From == figure || arc.To == figure)
                                    figure.ThisArcs.Add(arc);
                        }



                        btnAddPlace.Visibility = Visibility.Visible;
                        btnAddTransition.Visibility = Visibility.Visible;
                        btnAddArc.Visibility = Visibility.Visible;

                        btnNonOrientedArc.Visibility = Visibility.Collapsed;
                        btnAddToken.Visibility = Visibility.Visible;
                        pnlTopToolPanel.Visibility = Visibility.Visible;

                        btnArrangeModel.IsEnabled = true;
                    }
                    //todo тут
                    ClearCommandStacks();


                    if (flag == false)
                    {
                        currentFileName = OFdialog.FileName;
                        menuSave.IsEnabled = true;
                        MainController.Self.SetMenuItemEnabled(Core.Util.PathOfMethod(MenuSave_Click), true);

                        foreach (PetriNetNode figure in Net.Nodes)
                        {
                            DrawFigure(figure);
                        }
                        foreach (VArc arc in Net.arcs)
                        {
                            DisplayArc(arc);
                        }
                        VisUtil.ResizeCanvas(Net.Nodes, MainControl, MainModelCanvas);
                    }

                }
            }
            if (_thisScale != 1)
            {
                _scaleTransform.ScaleX = 1;
                _scaleTransform.ScaleY = 1;
                _thisScale = 1;
                MainModelCanvas.LayoutTransform = _scaleTransform;
                MainModelCanvas.UpdateLayout();
            }
            btnGrid.IsEnabled = true;
            btnShowHideLabels.IsEnabled = true;
            hideLabels = false;
            TurnOnSelectMode();
        }

        [MenuItemHandler("file/new")]
        public void Handler()
        {
            MenuNew_Click(null,null);
        }
        public void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.IsEnabled = true;
            IsCancelPressed = false;
            EnableAddButtons();
            var result = MessageBoxResult.None;

            if (isSomethingChanged)
            {
                if (currentFileName == null)
                {
                    result = MessageBox.Show("Do you want to save the current file?",
                        "Save..", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var SFdialog = new SaveFileDialog();
                        SetFilterToSaveFileDialog(SFdialog);

                        if (SFdialog.ShowDialog() == true)
                        {
                            SaveModel(SFdialog.FileName, true);
                            menuSave.IsEnabled = true;
                            MainController.Self.SetMenuItemEnabled(Core.Util.PathOfMethod(MenuSave_Click), true);

                        }
                        currentFileName = SFdialog.FileName;
                    }
                }
                else
                {
                    result = MessageBox.Show("Do you want to save changes in "
                        + System.IO.Path.GetFileName(currentFileName) + "?", "Save..", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        SaveModel(currentFileName, true);
                    }
                }
            }
            if (result == MessageBoxResult.No || result == MessageBoxResult.Yes || isSomethingChanged == false)
            {
                if (IsWindowClosing == false)
                {
                    DeleteArcs(Net.arcs);
                    while (Net.Nodes.Count != 0)
                        RemoveNode(Net.Nodes[0]);

                    //Net.arcs.Clear();
                    //SetOfFigures.Figures.Clear();
                    ClearCanvasWithoutLoss();
                    _selectedFigures.Clear();
                    _selectedArcs.Clear();
                    _selectedArc = null;
                    _selectedFigure = null;

                    VisUtil.ResizeCanvas(Net.Nodes, MainControl, MainModelCanvas);
                    btnUndo.IsEnabled = true;
                    HideAllProperties();
                    if (_thisScale != 1)
                    {
                        _scaleTransform.ScaleX = 1;
                        _scaleTransform.ScaleY = 1;
                        _thisScale = 1;
                        MainModelCanvas.LayoutTransform = _scaleTransform;
                        MainModelCanvas.UpdateLayout();
                    }
                    btnGrid.IsEnabled = true;

                    currentFileName = null;
                    menuSave.IsEnabled = false;
                    MainController.Self.SetMenuItemEnabled(Core.Util.PathOfMethod(MenuSave_Click), false);

                }
                ShowMainWindowTitleDelegate("Carassius - Petri Net Editor");
            }
            else IsCancelPressed = true;
            TurnOnSelectMode();
        }
        [MenuItemHandler("file/save",24,false)]
        public void MenuSave_Click()
        {
            btnSelect.IsEnabled = true;
            SaveModel(currentFileName, true);
            isSomethingChanged = false;
        }

        private void MenuSaveWithoutLayout_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.IsEnabled = true;
            SaveModel(currentFileName, false);
            isSomethingChanged = false;
        }

        private void MenuSaveAsWithoutLayout_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.IsEnabled = true;
            SaveFileDialog SFdialog = new SaveFileDialog();
            SetFilterToSaveFileDialog(SFdialog);
            if (SFdialog.ShowDialog() == true)
            {
                SaveModel(SFdialog.FileName, false);
                menuSave.IsEnabled = true;
                MainController.Self.SetMenuItemEnabled(Core.Util.PathOfMethod(MenuSave_Click), true);

                numberOfModel++;
            }
            currentFileName = SFdialog.FileName;
            isSomethingChanged = false;
        }
    }
}
