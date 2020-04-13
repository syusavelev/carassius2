﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core;
using Core.Model;
using PNEditorReduction.Model;

namespace PNEditorReduction
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

        public VPetriNet MyClone()
        {
            VPetriNet temp = VPetriNet.Create();

            temp.places = new ShadowCopyList<VPlace, Place>(places.getViewList().Count, places.getmodelList().Count);
            foreach (VPlace p in places.getViewList())
                temp.places.getViewList().Add(p.MyClone());
            foreach (Place p in places.getmodelList())
                temp.places.getmodelList().Add((Place)p.MyClone());

            temp.transitions = new ShadowCopyList<VTransition, Transition>(transitions.getViewList().Count, transitions.getmodelList().Count);
            foreach (VTransition p in transitions.getViewList())
                temp.transitions.getViewList().Add(p.MyClone());
            foreach (Transition p in transitions.getmodelList())
                temp.transitions.getmodelList().Add((Transition)p.MyClone());

            temp.arcs = new ShadowCopyList<VArc, Arc>(arcs.getViewList().Count, arcs.getmodelList().Count);
            foreach (VArc p in arcs.getViewList())
                temp.arcs.getViewList().Add(p.MyClone());
            foreach (Arc p in arcs.getmodelList())
                temp.arcs.getmodelList().Add(p.MyClone());

            return temp;
        }
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
