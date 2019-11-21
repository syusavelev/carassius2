using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PNEditorEditView
{
    /// <summary>
    /// Author: Natalia Nikitina
    /// Work with properties here
    /// PAIS Lab, 2014
    /// </summary>
    public partial class PNEditorControl
    {
        public void ShowProperties(PetriNetNode selected)
        {
            foreach (VArc a in selected.ThisArcs)
            {
                VArc arc = a;
                if (a.To != selected)
                    textBoxOutgoingArcs.Text += arc.To.Id + "\n";
                else
                    textBoxIngoinArcs.Text += arc.From.Id + "\n";
            }
        }





        public void ShowArcProperties(VArc selected)
        {
           

            lblSource.Visibility = Visibility.Visible;
            lblSourceValue.Visibility = Visibility.Visible;
            lblTarget.Visibility = Visibility.Visible;
            lblTargetValue.Visibility = Visibility.Visible;

            lblSourceValue.Content = selected.From.Id;
            lblTargetValue.Content = selected.To.Id;

        }

        public void HideArcProperties()
        {
            lblSource.Visibility = Visibility.Collapsed;
            lblSourceValue.Visibility = Visibility.Collapsed;
            lblTarget.Visibility = Visibility.Collapsed;
            lblTargetValue.Visibility = Visibility.Collapsed;

        }

        public void HideAllProperties()
        {
            HideArcProperties();
            HideSelectedProperties();
        }

        public void HideSelectedProperties()
        {
            lblSelected.Visibility = Visibility.Collapsed;
            textboxForSelectedList.Visibility = Visibility.Collapsed;
        }

        public void ShowSelectedProperties()
        {
            lblSelected.Visibility = Visibility.Visible;
            textboxForSelectedList.Visibility = Visibility.Visible;
        }

        public void RefreshSelectedList()
        {
            textboxForSelectedList.Clear();
            foreach (PetriNetNode figure in _selectedFigures)
                textboxForSelectedList.Text += figure.Id + "\n";
            foreach (VArc arc in _selectedArcs)
                textboxForSelectedList.Text += arc.Id + "\n";
        }

        public void ReassignSelectedProperties()
        {
            var allSelectedObjects = new List<object>();
            allSelectedObjects.AddRange(_selectedFigures);
            allSelectedObjects.AddRange(_selectedArcs);
            if (allSelectedObjects.Count != 1)
            {
                foreach (var propertyEditor in _propertyEditors)
                {
                    propertyEditor.SetItem(null);
                }
            }
            else
            {
                var item = allSelectedObjects.First();
                foreach (var propertyEditor in _propertyEditors)
                {
                    propertyEditor.SetItem(item);
                }
            }
            //old monstrous code

            if (_selectedFigures.Count == 1 && _selectedArcs.Count == 0)
            {
                HideSelectedProperties();
                HideArcProperties();
                _selectedFigure = _selectedFigures[0];
                ShowProperties(_selectedFigure);
            }
            else if (_selectedArcs.Count == 1 && _selectedFigures.Count == 0)
            {
                HideSelectedProperties();
                _selectedArc = _selectedArcs[0];
                ShowArcProperties(_selectedArc);
            }
            else if (_selectedArcs.Count + _selectedFigures.Count > 1)
            {
                HideArcProperties();
                ShowSelectedProperties();
                RefreshSelectedList();
            }
            else
            {
                HideArcProperties();
                HideSelectedProperties();
            }
            //if (btnSimulationMode.IsEnabled == false)
            //{
            //    lblName.Visibility = Visibility.Collapsed;
            //    tbName.Visibility = Visibility.Collapsed;
            //    btnOkName.Visibility = Visibility.Collapsed;
            //}
        }
    }
}