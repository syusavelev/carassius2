using System.Windows;

namespace PNEditorSimulateView
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
            lblID.Visibility = Visibility.Visible;
            lblIDValue.Visibility = Visibility.Visible;

            lblName.Visibility = Visibility.Visible;
            tbName.Visibility = Visibility.Visible;
            btnOkName.Visibility = Visibility.Visible;

            lblIngoingArcs.Visibility = Visibility.Visible;
            textBoxIngoinArcs.Visibility = Visibility.Visible;

            lblOutgoinArcs.Visibility = Visibility.Visible;
            textBoxOutgoingArcs.Visibility = Visibility.Visible;

            lblIDValue.Content = selected.Id;
            tbName.Text = selected.Label;
            VPlace place = selected as VPlace;
            if (place != null)
            {
                ShowOnlyPlaceProperties();
                tbTokenNumber.Text = place.NumberOfTokens.ToString();
            }
            else
            //HideOnlyPlaceProperties();
            {
                VTransition transition = selected as VTransition;
                ShowOnlyTransitionProperties();
                tbPriority.Text = transition.Priority.ToString();
            }

            textBoxIngoinArcs.Clear();
            textBoxOutgoingArcs.Clear();
            foreach (VArc a in selected.ThisArcs)
            {
                VArc arc = a;
                if (a.To != selected)
                    textBoxOutgoingArcs.Text += arc.To.Id + "\n";
                else
                    textBoxIngoinArcs.Text += arc.From.Id + "\n";
            }
        }

        public void HideProperties()
        {
            lblID.Visibility = Visibility.Collapsed;
            lblIDValue.Visibility = Visibility.Collapsed;

            lblName.Visibility = Visibility.Collapsed;
            tbName.Visibility = Visibility.Collapsed;
            btnOkName.Visibility = Visibility.Collapsed;

            lblIngoingArcs.Visibility = Visibility.Collapsed;
            textBoxIngoinArcs.Visibility = Visibility.Collapsed;

            lblOutgoinArcs.Visibility = Visibility.Collapsed;
            textBoxOutgoingArcs.Visibility = Visibility.Collapsed;

            HideOnlyPlaceProperties();
        }

        public void HidePetriNetProperties()
        {
            btnOkName.Visibility = Visibility.Collapsed;
            btnTokenNumberChangeFieldMinus.Visibility = Visibility.Collapsed;
            btnTokenNumberChangeFieldPlus.Visibility = Visibility.Collapsed;
            btnTokenNumberChangeFieldOk.Visibility = Visibility.Collapsed;
            btnPriorityChangeFieldMinus.Visibility = Visibility.Collapsed;
            btnPriorityChangeFieldPlus.Visibility = Visibility.Collapsed;
            btnPriorityChangeFieldOk.Visibility = Visibility.Collapsed;
            tbName.Visibility = Visibility.Collapsed;
            tbTokenNumber.Visibility = Visibility.Collapsed;
            lblName.Visibility = Visibility.Collapsed;
            lblNumOfTokens.Visibility = Visibility.Collapsed;
            tbPriority.Visibility = Visibility.Collapsed;
            lblPriority.Visibility = Visibility.Collapsed;
            HideOnlyPlaceProperties();
            HideOnlyTransitionProperties();
        }

        public void ShowPetriNetProperties()
        {
            btnOkName.Visibility = Visibility.Visible;
            btnTokenNumberChangeFieldMinus.Visibility = Visibility.Visible;
            btnTokenNumberChangeFieldPlus.Visibility = Visibility.Visible;
            btnTokenNumberChangeFieldOk.Visibility = Visibility.Visible;
            btnPriorityChangeFieldMinus.Visibility = Visibility.Visible;
            btnPriorityChangeFieldPlus.Visibility = Visibility.Visible;
            btnPriorityChangeFieldOk.Visibility = Visibility.Visible;
            tbName.Visibility = Visibility.Visible;
            tbTokenNumber.Visibility = Visibility.Visible;
            lblName.Visibility = Visibility.Visible;
            lblNumOfTokens.Visibility = Visibility.Visible;
            tbPriority.Visibility = Visibility.Visible;
            lblPriority.Visibility = Visibility.Visible;
        }

        public void HideOnlyTransitionProperties()
        {
            lblPriority.Visibility = Visibility.Collapsed;
            tbPriority.Visibility = Visibility.Collapsed;
            btnPriorityChangeFieldMinus.Visibility = Visibility.Collapsed;
            btnPriorityChangeFieldPlus.Visibility = Visibility.Collapsed;
            btnPriorityChangeFieldOk.Visibility = Visibility.Collapsed;
        }

        public void ShowOnlyTransitionProperties()
        {
            lblPriority.Visibility = Visibility.Visible;
            tbPriority.Visibility = Visibility.Visible;
            btnPriorityChangeFieldMinus.Visibility = Visibility.Visible;
            btnPriorityChangeFieldPlus.Visibility = Visibility.Visible;
            btnPriorityChangeFieldOk.Visibility = Visibility.Visible;
        }

        public void HideOnlyPlaceProperties()
        {
            lblNumOfTokens.Visibility = Visibility.Collapsed;
            tbTokenNumber.Visibility = Visibility.Collapsed;
            btnTokenNumberChangeFieldMinus.Visibility = Visibility.Collapsed;
            btnTokenNumberChangeFieldPlus.Visibility = Visibility.Collapsed;
            btnTokenNumberChangeFieldOk.Visibility = Visibility.Collapsed;
        }

        public void ShowOnlyPlaceProperties()
        {
            lblNumOfTokens.Visibility = Visibility.Visible;
            tbTokenNumber.Visibility = Visibility.Visible;
            btnTokenNumberChangeFieldMinus.Visibility = Visibility.Visible;
            btnTokenNumberChangeFieldPlus.Visibility = Visibility.Visible;
            btnTokenNumberChangeFieldOk.Visibility = Visibility.Visible;
        }

        public void ShowArcProperties(VArc selected)
        {
            lblID.Visibility = Visibility.Visible;
            lblIDValue.Visibility = Visibility.Visible;
            lblIDValue.Content = selected.Id;

            lblSource.Visibility = Visibility.Visible;
            lblSourceValue.Visibility = Visibility.Visible;
            lblTarget.Visibility = Visibility.Visible;
            lblTargetValue.Visibility = Visibility.Visible;

            lblSourceValue.Content = selected.From.Id;
            lblTargetValue.Content = selected.To.Id;

            lblWeight.Visibility = Visibility.Visible;
            tbArcWeight.Visibility = Visibility.Visible;

            lblWeight.Content = "Weight: ";
            tbArcWeight.Text = selected.Weight;

            btnWeightOK.Visibility = Visibility.Visible;
        }

        public void HideArcProperties()
        {
            lblSource.Visibility = Visibility.Collapsed;
            lblSourceValue.Visibility = Visibility.Collapsed;
            lblTarget.Visibility = Visibility.Collapsed;
            lblTargetValue.Visibility = Visibility.Collapsed;

            lblWeight.Visibility = Visibility.Collapsed;
            tbArcWeight.Visibility = Visibility.Collapsed;
            btnWeightOK.Visibility = Visibility.Collapsed;
        }

        public void HideAllProperties()
        {
            HideProperties();
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
                HideProperties();
                _selectedArc = _selectedArcs[0];
                ShowArcProperties(_selectedArc);
            }
            else if (_selectedArcs.Count + _selectedFigures.Count > 1)
            {
                HideProperties();
                HideArcProperties();
                ShowSelectedProperties();
                RefreshSelectedList();
            }
            else
            {
                HideProperties();
                HideArcProperties();
                HideSelectedProperties();
            }
           
        }
    }
}