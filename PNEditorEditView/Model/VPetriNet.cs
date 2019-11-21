using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core;
using Core.Model;
using PNEditorEditView.Model;

namespace PNEditorEditView
{
    /// <summary>
    /// Author: Natalia Nikitina
    /// Represent a petri net
    /// PAIS Lab, 2015
    /// </summary>
    public class VPetriNet : IModel
    {
        public ShadowCopyList<VPlace, Place> places;
        public ShadowCopyList<VTransition, Transition> transitions;
        public ShadowCopyList<VArc, Arc> arcs;

        public List<PetriNetNode> nodes;

        public List<PetriNetNode> Nodes
        {
            //TODO: why getting creates each time new list. Why to store it
            //TODO: move to method
            get
            {
                nodes = new List<PetriNetNode>();
                nodes.AddRange(places);
                nodes.AddRange(transitions);
                return nodes;
            }
        }

        public void RedrawModel()
        {

        }

        public void ShowCorrespondingProperties()
        {

        }

        public void RemoveNode(Node node)
        {
            VPlace place = node as VPlace;
            if (place != null)
                PNEditorControl.Net.places.Remove(place);
            else
                PNEditorControl.Net.transitions.Remove(node as VTransition); 
        }

        public void DeleteArcs(IList<VArc> arcs)
        {
            foreach (VArc arc in arcs.ToList())
            {
                PNEditorControl.Net.arcs.Remove(arc);
                arc.From.ThisArcs.Remove(arc);
                arc.To.ThisArcs.Remove(arc);
                // arc.WeightLabel = null;
            }
            arcs.Clear();
        }

        //----------------------------------------------------------------------

        public static VPetriNet Create()
        {
            return new VPetriNet();
        }

        //----------------------------------------------------------------------

        private VPetriNet()
        {
            places = new ShadowCopyList<VPlace, Place>(MainController.Self.Net.Places);
            transitions = new ShadowCopyList<VTransition, Transition>(MainController.Self.Net.Transitions);
            arcs = new ShadowCopyList<VArc, Arc>(MainController.Self.Net.Arcs);
        }

        //private VPetriNet(List<VPlace> places, List<VTransition> transitions, List<VArc> arcs)
        //{
        //    this.places = new List<VPlace>(places);
        //    this.transitions = new List<VTransition>(transitions);
        //    this.arcs = new List<VArc>(arcs);
        //    foreach (var arc in arcs)
        //    {
        //        Debug.Assert(!ArcSourceAndTargetOfSameType(arc), "Arc source and target must be of different types");
        //    }
        //}

        private bool ArcSourceAndTargetOfSameType(VArc arc)
        {
            return (arc.From is VPlace && arc.To is VPlace) || (arc.From is VTransition && arc.To is VTransition);
        }

    }
}
